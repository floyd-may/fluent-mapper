using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentMapping.Internal;

namespace FluentMapping
{
    public sealed class ContextualSetterSpec<TTarget, TSource, TProperty, TContext>
        where TTarget : class
        where TSource : class
    {
        private readonly Expression<Func<TTarget, TProperty>> _propertyExpression;

        public ContextualSetterSpec(
            ContextualTypeMappingSpec<TTarget, TSource, TContext> mappingSpec, 
            Expression<Func<TTarget, TProperty>> propertyExpression)
        {
            _propertyExpression = propertyExpression;
            MappingSpec = mappingSpec;
        }

        public ContextualTypeMappingSpec<TTarget, TSource, TContext> MappingSpec { get; private set; }

        public ContextualTypeMappingSpec<TTarget, TSource, TContext> From(
            Expression<Func<TSource, TContext, TProperty>> getExpression)
        {
            var innerSpec = MappingSpec.InnerSpec.IgnoringNestedSourceProperty(getExpression);

            var setterActionExpr = _propertyExpression.ToSetCall(getExpression);

            return new ContextualTypeMappingSpec<TTarget, TSource, TContext>(
                innerSpec, 
                new List<Expression>(MappingSpec.ContextualMappings){ setterActionExpr },
                MappingSpec.Constructor
                );
        }

        
    }
}