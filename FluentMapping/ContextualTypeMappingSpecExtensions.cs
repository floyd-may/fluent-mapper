using System;
using System.Linq.Expressions;

namespace FluentMapping
{
    public static class ContextualTypeMappingSpecExtensions
    {
        public static ContextualTypeMappingSpec<TTarget, TSource, TContext>
            IgnoringSourceProperty<TTarget, TSource, TProperty, TContext>(
                this ContextualTypeMappingSpec<TTarget, TSource, TContext> spec,
                Expression<Func<TSource, TProperty>> propertyExpression)
        {
            return new ContextualTypeMappingSpec<TTarget, TSource, TContext>(
                spec.InnerSpec.IgnoringSourceProperty(propertyExpression),
                spec.ContextualMappings);
        }

        public static ContextualTypeMappingSpec<TTarget, TSource, TContext>
            IgnoringTargetProperty<TTarget, TSource, TProperty, TContext>(
                this ContextualTypeMappingSpec<TTarget, TSource, TContext> spec,
                Expression<Func<TTarget, TProperty>> propertyExpression)
        {
            return new ContextualTypeMappingSpec<TTarget, TSource, TContext>(
                spec.InnerSpec.IgnoringTargetProperty(propertyExpression), 
                spec.ContextualMappings);
        }

        public static ContextualTypeMappingSpec<TTarget, TSource, TContext>
            IgnoringSourceProperty<TTarget, TSource, TContext>(
                this ContextualTypeMappingSpec<TTarget, TSource, TContext> spec,
                string propertyName)
        {
            return new ContextualTypeMappingSpec<TTarget, TSource, TContext>(
                spec.InnerSpec.IgnoringSourceProperty(propertyName),
                spec.ContextualMappings);
        }

        public static ContextualTypeMappingSpec<TTarget, TSource, TContext>
            IgnoringTargetProperty<TTarget, TSource, TContext>(
                this ContextualTypeMappingSpec<TTarget, TSource, TContext> spec,
                string propertyName)
        {
            return new ContextualTypeMappingSpec<TTarget, TSource, TContext>(
                spec.InnerSpec.IgnoringTargetProperty(propertyName),
                spec.ContextualMappings
                );
        } 
    }
}