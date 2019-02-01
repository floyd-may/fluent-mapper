using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentMapping.Internal;

namespace FluentMapping
{
    public sealed class ExpandIntoNestedTargetSpec<TTarget, TSource, TTargetProperty, TNestedMappingTarget>
        where TTarget : class
        where TSource : class
        where TTargetProperty : class
        where TNestedMappingTarget : class
    {
        public TypeMappingSpec<TTarget, TSource> Spec { get; private set; }
        public Expression<Func<TTarget, TTargetProperty>> TargetPropertyExpression { get; private set; }
        public TypeMappingSpec<TNestedMappingTarget, TSource> NestedSpec { get; private set; }
        public Func<TNestedMappingTarget, TTargetProperty> TargetMapFunction { get; private set; }

        public ExpandIntoNestedTargetSpec(
            TypeMappingSpec<TTarget, TSource> spec,
            Expression<Func<TTarget, TTargetProperty>> targetPropertyExpression)
            : this(spec, targetPropertyExpression, FluentMapper.ThatMaps<TNestedMappingTarget>().From<TSource>(), null)
        {
            if(!typeof(TTargetProperty).IsAssignableFrom(typeof(TNestedMappingTarget)))
                throw new ArgumentException(string.Format("Must provide a mapping function from {0} to {1}.", typeof(TTargetProperty).Name, typeof(TNestedMappingTarget).Name));
        }

        private ExpandIntoNestedTargetSpec(
            TypeMappingSpec<TTarget, TSource> spec,
            Expression<Func<TTarget, TTargetProperty>> targetPropertyExpression,
            TypeMappingSpec<TNestedMappingTarget, TSource> nestedSpec,
            Func<TNestedMappingTarget, TTargetProperty> targetMapFunc
            )
        {
            Spec = spec;
            TargetPropertyExpression = targetPropertyExpression;
            NestedSpec = nestedSpec;

            if (null == targetMapFunc)
            {
                var parm = Expression.Parameter(typeof (TNestedMappingTarget));
                targetMapFunc = Expression.Lambda<Func<TNestedMappingTarget, TTargetProperty>>(parm, parm).Compile();
            }

            TargetMapFunction = targetMapFunc;
        }

        public ExpandIntoNestedTargetSpec<TTarget, TSource, TTargetProperty, TNestedMappingTarget> WithConstructor(Func<TNestedMappingTarget> constructor)
        {
            return new ExpandIntoNestedTargetSpec<TTarget, TSource, TTargetProperty, TNestedMappingTarget>(
                Spec,
                TargetPropertyExpression,
                NestedSpec.WithConstructor(constructor),
                TargetMapFunction
                );
        }

        public TypeMappingSpec<TTarget, TSource> UsingDefaultMappings()
        {
            return UsingMapping(x => x);
        }

        public TypeMappingSpec<TTarget, TSource> UsingMapping(
            Func<TypeMappingSpec<TNestedMappingTarget, TSource>, TypeMappingSpec<TNestedMappingTarget, TSource>> nestedSpecTransform)
        {
            var previouslyUnmatchedSourceNames =
                new HashSet<string>(
                    NestedSpec.GetUnmappedSourceValues().Select(x => x.PropertyName)
                    );

            var finalNestedSpec = nestedSpecTransform(NestedSpec);

            var unmatchedSourceNames = new HashSet<string>(
                finalNestedSpec.GetUnmappedSourceValues()
                    .Select(x => x.PropertyName)
                );
            var customMappedSourceNames = previouslyUnmatchedSourceNames
                .Except(unmatchedSourceNames);

            finalNestedSpec = finalNestedSpec
                .WithSourceValues(
                    finalNestedSpec.SourceValues
                        .Where(x => !unmatchedSourceNames.Contains(x.PropertyName)));

            var nestedMapper = finalNestedSpec.Create();

            var nestedSourceValueNames = new HashSet<string>(
                finalNestedSpec.SourceValues.Select(x => x.PropertyName));

            nestedSourceValueNames.UnionWith(customMappedSourceNames);

            return Spec
                .ThatSets(TargetPropertyExpression).From(src => TargetMapFunction(nestedMapper.Map(src)))
                .WithSourceValues(
                    Spec.SourceValues
                        .Where(x => !nestedSourceValueNames.Contains(x.PropertyName)));
        }

        public ExpandIntoNestedTargetSpec<TTarget, TSource, TTargetProperty, TIntermediary> 
            Via<TIntermediary>(Func<TIntermediary> constructor, Func<TIntermediary, TTargetProperty> buildFunc) where TIntermediary : class
        {
            return new ExpandIntoNestedTargetSpec<TTarget, TSource, TTargetProperty, TIntermediary>(
                Spec,
                TargetPropertyExpression,
                FluentMapper.ThatMaps<TIntermediary>().From<TSource>()
                    .WithConstructor(constructor),
                buildFunc
                );
        }
    }
}