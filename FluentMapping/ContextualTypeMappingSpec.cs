using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FluentMapping
{
    public sealed class ContextualTypeMappingSpec<TTarget, TSource, TContext>
        where TTarget : class
        where TSource : class
    {
        public ContextualTypeMappingSpec(TypeMappingSpec<TTarget, TSource> innerSpec) : this(innerSpec, null, null)
        {
            
        }

        public ContextualTypeMappingSpec(TypeMappingSpec<TTarget, TSource> innerSpec, IEnumerable<Expression> contextualMappings, Expression constructor)
        {
            InnerSpec = innerSpec;
            ContextualMappings = contextualMappings ?? new Expression[0];
            Constructor = constructor;
        }

        public TypeMappingSpec<TTarget, TSource> InnerSpec { get; private set; }
        public IEnumerable<Expression> ContextualMappings { get; private set; }
        public Expression Constructor { get; private set; }

        public ContextualSetterSpec<TTarget, TSource, TProperty, TContext> ThatSets<TProperty>(Expression<Func<TTarget, TProperty>> propertyExpression)
        {
            return new ContextualSetterSpec<TTarget, TSource, TProperty, TContext>(
                this.IgnoringTargetProperty(propertyExpression),
                propertyExpression
                );
        }

        public IContextualMapper<TTarget, TSource, TContext> Create()
        {
            var targetParam = Expression.Parameter(typeof (TTarget));
            var sourceParam = Expression.Parameter(typeof (TSource));
            var contextParam = Expression.Parameter(typeof (TContext));

            var accumulatedLambda = Expression.Invoke(ContextualMappings.First(), targetParam, sourceParam, contextParam);

            foreach (var setterExpr in ContextualMappings.Skip(1))
            {
                accumulatedLambda = Expression.Invoke(setterExpr, accumulatedLambda, sourceParam, contextParam);
            }

            var mappingLambda = Expression.Lambda<Func<TTarget, TSource, TContext, TTarget>>(
                    accumulatedLambda, 
                    targetParam, 
                    sourceParam, 
                    contextParam)
                .Compile();

            var constructor = GetConstructor();
            var innerMapper = InnerSpec.GetMapperFunc();

            return InnerSpec.Assembler.Assemble(constructor,
                (tgt, src, ctx) => mappingLambda(innerMapper(tgt, src), src, ctx));
        }

        private Func<TSource, TContext, TTarget> GetConstructor()
        {
            if (null == this.Constructor)
            {
                var ctor = InnerSpec.GetConstructor();
                return (src, ctx) => ctor(src);
            }

            var ctxParam = Expression.Parameter(typeof (TContext));
            var srcParam = Expression.Parameter(typeof(TSource));

            var invokeExpr = Expression.Invoke(Constructor, ctxParam);

            return Expression.Lambda<Func<TSource, TContext, TTarget>>(invokeExpr, srcParam, ctxParam).Compile();
        }
    }


}