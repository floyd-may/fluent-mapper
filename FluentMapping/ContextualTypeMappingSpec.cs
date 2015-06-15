using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FluentMapping
{
    public sealed class ContextualTypeMappingSpec<TTarget, TSource, TContext>
    {
        public ContextualTypeMappingSpec(TypeMappingSpec<TTarget, TSource> innerSpec) : this(innerSpec, null)
        {
            
        }

        public ContextualTypeMappingSpec(TypeMappingSpec<TTarget, TSource> innerSpec, IEnumerable<Expression> contextualMappings)
        {
            InnerSpec = innerSpec;
            ContextualMappings = contextualMappings ?? new Expression[0];
        }

        public TypeMappingSpec<TTarget, TSource> InnerSpec { get; private set; }
        public IEnumerable<Expression> ContextualMappings { get; private set; }

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

            var invocations = ContextualMappings
                .Select(x => Expression.Invoke(x, targetParam, sourceParam, contextParam));

            var blockExpr = Expression.Block((IEnumerable<Expression>) invocations);

            var mappingLambda = Expression.Lambda<Action<TTarget, TSource, TContext>>(blockExpr, targetParam, sourceParam, contextParam);

            return new SimpleContextualMapper<TTarget, TSource, TContext>(InnerSpec.Create(), mappingLambda.Compile());
        }
    }


}