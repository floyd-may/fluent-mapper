using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentMapping.Internal;

namespace FluentMapping
{
    public static class ContextualTypeMappingSpecExtensions
    {
        public static ContextualTypeMappingSpec<TTarget, TSource, TContext>
            IgnoringSourceProperty<TTarget, TSource, TProperty, TContext>(
                this ContextualTypeMappingSpec<TTarget, TSource, TContext> spec,
                Expression<Func<TSource, TProperty>> propertyExpression)
            where TTarget : class
            where TSource : class
        {
            return new ContextualTypeMappingSpec<TTarget, TSource, TContext>(
                spec.InnerSpec.IgnoringSourceProperty(propertyExpression),
                spec.ContextualMappings,
                spec.Constructor);
        }

        public static ContextualTypeMappingSpec<TTarget, TSource, TContext>
            IgnoringTargetProperty<TTarget, TSource, TProperty, TContext>(
                this ContextualTypeMappingSpec<TTarget, TSource, TContext> spec,
                Expression<Func<TTarget, TProperty>> propertyExpression)
            where TTarget : class
            where TSource : class
        {
            return new ContextualTypeMappingSpec<TTarget, TSource, TContext>(
                spec.InnerSpec.IgnoringTargetProperty(propertyExpression),
                spec.ContextualMappings,
                spec.Constructor);
        }

        public static ContextualTypeMappingSpec<TTarget, TSource, TContext>
            IgnoringSourceProperty<TTarget, TSource, TContext>(
                this ContextualTypeMappingSpec<TTarget, TSource, TContext> spec,
                string propertyName)
            where TTarget : class
            where TSource : class
        {
            return new ContextualTypeMappingSpec<TTarget, TSource, TContext>(
                spec.InnerSpec.IgnoringSourceProperty(propertyName),
                spec.ContextualMappings,
                spec.Constructor);
        }

        public static ContextualTypeMappingSpec<TTarget, TSource, TContext>
            IgnoringTargetProperty<TTarget, TSource, TContext>(
                this ContextualTypeMappingSpec<TTarget, TSource, TContext> spec,
                string propertyName)
            where TTarget : class
            where TSource : class
        {
            return new ContextualTypeMappingSpec<TTarget, TSource, TContext>(
                spec.InnerSpec.IgnoringTargetProperty(propertyName),
                spec.ContextualMappings,
                spec.Constructor
                );
        }

        public static ContextualTypeMappingSpec<TTarget, TSource, TContext>
            WithCustomMap<TTarget, TSource, TContext>(
            this ContextualTypeMappingSpec<TTarget, TSource, TContext> spec,
            Expression<Func<TTarget, TSource, TContext, TTarget>> expression)
            where TTarget : class
            where TSource : class
        {
            return new ContextualTypeMappingSpec<TTarget, TSource, TContext>(
                spec.InnerSpec
                    .IgnoringNestedSourceProperty(expression)
                    .WithTargetValues(spec.InnerSpec.TargetValues.Where(x => !x.IsSupersededBy(expression))),
                new List<Expression>(spec.ContextualMappings) { expression },
                spec.Constructor
                );
        }

        public static ContextualTypeMappingSpec<TTarget, TSource, TContext>
            WithConstructor<TTarget, TSource, TContext>(
            this ContextualTypeMappingSpec<TTarget, TSource, TContext> spec,
            Expression<Func<TContext, TTarget>> constructorExpression)
            where TTarget : class
            where TSource : class
        {
            return new ContextualTypeMappingSpec<TTarget, TSource, TContext>(
                spec.InnerSpec,
                spec.ContextualMappings,
                constructorExpression);
        }
    }
}