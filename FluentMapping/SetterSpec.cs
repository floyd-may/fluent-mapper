using System;
using System.Linq.Expressions;
using FluentMapping.Internal;

namespace FluentMapping
{
    public sealed class SetterSpec<TTarget, TSource, TProperty>
    {
        public SetterSpec(TypeMappingSpec<TTarget, TSource> spec, Expression<Func<TTarget, TProperty>> tgtPropExpression)
        {
            Spec = spec;
            TgtPropExpression = tgtPropExpression;
        }

        public TypeMappingSpec<TTarget, TSource> Spec { get; private set; }
        public Expression<Func<TTarget, TProperty>> TgtPropExpression { get; private set; }

        public TypeMappingSpec<TTarget, TSource> From(Expression<Func<TSource, TProperty>> getExpression)
        {
            var setter = TgtPropExpression.ToSetCall(getExpression);

            return Spec
                .IgnoringTargetProperty(TgtPropExpression)
                .IgnoringNestedSourceProperty(getExpression)
                .WithCustomMapper(setter);
        }
    }
}