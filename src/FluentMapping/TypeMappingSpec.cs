using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FluentMapping
{
    public sealed partial class TypeMappingSpec<TTarget, TSource>
        where TTarget : class
        where TSource : class
    {
        public IEnumerable<SourceValue<TSource>> SourceValues { get; private set; }
        public IEnumerable<ITargetValue<TTarget>> TargetValues { get; private set; }
        public IEnumerable<Expression> CustomMappings { get; private set; }
        public Func<TSource, TTarget> ConstructorFunc { get; private set; }
        public IAssembler Assembler { get; private set; }

        public TypeMappingSpec() : this(GetDefaultTargetValues(), GetDefaultSourceValues(), new Expression[0], null, null)
        {
        }

        public TypeMappingSpec(
            ITargetValue<TTarget>[] targetValues, 
            SourceValue<TSource>[] sourceValues, 
            Expression[] customMappings,
            Func<TSource, TTarget> constructorFunc,
            IAssembler assembler
            )
        {
            TargetValues = targetValues;
            SourceValues = sourceValues;
            CustomMappings = customMappings ?? Enumerable.Empty<Expression>();
            ConstructorFunc = constructorFunc;
            Assembler = assembler ?? new DefaultAssembler();
        }

        public IMapper<TTarget, TSource> Create()
        {
            ValidateMapping();

            var constructor = GetConstructor();

            var mapperFunc = GetMapperFunc();

            return Assembler.Assemble(constructor, mapperFunc);
        }

        public Func<TTarget, TSource, TTarget> GetMapperFunc()
        {
            ValidateMapping();

            var targetParam = Expression.Parameter(typeof (TTarget));
            var sourceParam = Expression.Parameter(typeof (TSource));

            var setterActions = TargetValues.OrderBy(x => x.PropertyName)
                .Zip(SourceValues.OrderBy(x => x.PropertyName), (tgt, src) => tgt.CreateSetter(src.CreateGetter()))
                .Concat(CustomMappings)
                .Select(x => EnsureReturnsTarget(x))
                .ToArray()
                ;

            if (!setterActions.Any())
                return (tgt, src) => tgt;

            var accumulatedLambda = Expression.Invoke(setterActions.First(), targetParam, sourceParam);

            foreach (var setterExpr in setterActions.Skip(1))
            {
                accumulatedLambda = Expression.Invoke(setterExpr, accumulatedLambda, sourceParam);
            }

            var mapperFunc = Expression.Lambda<Func<TTarget, TSource, TTarget>>(accumulatedLambda, targetParam, sourceParam);

            return mapperFunc.Compile();
        }

        public Func<TSource, TTarget> GetConstructor()
        {
            if(ConstructorFunc != null)
                return ConstructorFunc;

            var ctorInfo = typeof (TTarget).GetConstructor(new Type [0]);

            if(ctorInfo == null)
                throw new InvalidOperationException(string.Format("Unable to find constructor for type '{0}'.", typeof(TTarget).Name));

            return Expression.Lambda<Func<TSource, TTarget>>(
                Expression.New(ctorInfo),
                Expression.Parameter(typeof(TSource))
                ).Compile();
        }

        public ContextualTypeMappingSpec<TTarget, TSource, TContext> UsingContext<TContext>()
        {
            return new ContextualTypeMappingSpec<TTarget, TSource, TContext>(this);
        }

        public SetterSpec<TTarget, TSource, TProperty> ThatSets<TProperty>(
            Expression<Func<TTarget, TProperty>> propertyExpression)
        {
            return new SetterSpec<TTarget, TSource, TProperty>(this, propertyExpression);
        }

        public IEnumerable<ITargetValue<TTarget>> GetUnmappedTargetValues()
        {
            var sourcesByName = SourceValues.ToDictionary(x => x.PropertyName);

            return TargetValues.Where(x => !sourcesByName.ContainsKey(x.PropertyName));
        }

        public IEnumerable<SourceValue<TSource>> GetUnmappedSourceValues()
        {
            var targetValuesByName = TargetValues.ToDictionary(x => x.PropertyName);

            return SourceValues.Where(x => !targetValuesByName.ContainsKey(x.PropertyName));
        }

        private void ValidateMapping()
        {
            var targetNames = TargetValues.Select(x => x.PropertyName);
            var sourceNames = SourceValues.Select(x => x.PropertyName);

            var unmatchedTargets = targetNames.Except(sourceNames);

            foreach (var targetProperty in GetUnmappedTargetValues())
            {
                ThrowUnmatchedTarget(targetProperty);
            }

            var unmatchedSources = sourceNames.Except(targetNames);

            foreach (var sourceProperty in unmatchedSources)
            {
                ThrowUnmatchedSource(SourceValues.First(x => x.PropertyName == sourceProperty));
            }

            var mismatchedTypes = SourceValues.OrderBy(x => x.PropertyName)
                .Zip(TargetValues.OrderBy(x => x.PropertyName), (src, tgt) => new
                {
                    src,
                    tgt
                })
                .Where(x => !x.tgt.ValueType.IsAssignableFrom(x.src.ValueType));

            foreach (var mismatch in mismatchedTypes)
            {
                var msg = string.Format(
                    "Cannot map [{0}] from [{1}].",
                    mismatch.tgt.Description,
                    mismatch.src.Description
                    );
                throw new Exception(msg);
            }

        }

        private static void ThrowUnmatchedTarget(ITargetValue<TTarget> value)
        {
            var message = string.Format("Target {0} is unmatched.",
                value.Description);
            throw new Exception(message);
        }

        private static void ThrowUnmatchedSource(SourceValue<TSource> value)
        {
            var message = string.Format("Source {0} is unmatched.",
                value.Description);
            throw new Exception(message);
        }

        private static SourceValue<TSource>[] GetDefaultSourceValues()
        {
            return GetProperties(typeof(TSource))
                .Select(x => new SourceValue<TSource>(x))
                .ToArray();
        }

        private static ITargetValue<TTarget>[] GetDefaultTargetValues()
        {
            // ReSharper disable once CoVariantArrayConversion
            return typeof (TTarget).GetProperties()
                .Where(x => x.CanWrite && x.GetSetMethod() != null && x.GetSetMethod().IsPublic)
                .Select(x => new TargetValue<TTarget>(x))
                .ToArray();
        }

        private static LambdaExpression EnsureReturnsTarget(Expression e)
        {
            var lambda = e as LambdaExpression;

            if (lambda.ReturnType == typeof (TTarget))
                return lambda;

            var targetParam = lambda.Parameters[0];
            var sourceParam = lambda.Parameters[1];

            var block = Expression.Block(lambda.Body, targetParam);

            return Expression.Lambda(block, targetParam, sourceParam);
        }

        private static PropertyInfo[] GetProperties(Type type)
        {
            if (!type.IsInterface)
                return type.GetProperties();

            return (new Type[] { type })
                   .Concat(type.GetInterfaces())
                   .SelectMany(i => i.GetProperties()).ToArray();
        }
    }
}