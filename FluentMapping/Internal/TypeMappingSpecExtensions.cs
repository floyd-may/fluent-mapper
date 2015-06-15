using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FluentMapping.Internal
{
    internal static class TypeMappingSpecExtensions
    {
        internal static TypeMappingSpec<TTarget, TSource> 
            WithSourceValues<TTarget, TSource>(
                this TypeMappingSpec<TTarget, TSource> spec, 
                IEnumerable<SourceValue<TSource>> sourceValues)
        {
            return new TypeMappingSpec<TTarget, TSource>(
                spec.TargetValues.ToArray(),
                sourceValues.ToArray(),
                spec.CustomMappings.ToArray(),
                spec.ConstructorFunc
                );
        }

        internal static TypeMappingSpec<TTarget, TSource> 
            WithTargetValues<TTarget, TSource>(
                this TypeMappingSpec<TTarget, TSource> spec,
                IEnumerable<ITargetValue<TTarget>> targetValues)
        {
            return new TypeMappingSpec<TTarget, TSource>(
                targetValues.ToArray(),
                spec.SourceValues.ToArray(),
                spec.CustomMappings.ToArray(),
                spec.ConstructorFunc
                );
        }

        internal static TypeMappingSpec<TTarget, TSource>
            WithCustomMapper<TTarget, TSource>(this TypeMappingSpec<TTarget, TSource> spec, Expression customMapper)
        {
            return new TypeMappingSpec<TTarget, TSource>(
                spec.TargetValues.ToArray(),
                spec.SourceValues.ToArray(),
                new List<Expression>(spec.CustomMappings) { customMapper }.ToArray(),
                spec.ConstructorFunc
                );
        }

        internal static TypeMappingSpec<TTarget, TSource>
            IgnoringNestedSourceProperty<TTarget, TSource>(
                this TypeMappingSpec<TTarget, TSource> spec,
                Expression sourceExpression)
        {
            var visitor = new MemberVisitor<TSource>();
            visitor.Visit(sourceExpression);

            foreach (var expression in visitor.MemberExpressions)
            {
                spec = spec.IgnoringSourceProperty(expression.Member.Name);
            }

            return spec;
        }

        private class MemberVisitor<T> : ExpressionVisitor
        {
            private readonly List<MemberExpression> _expressions = new List<MemberExpression>(); 

            public IEnumerable<MemberExpression> MemberExpressions {
                get { return _expressions; }
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                var propertyInfo = node.Member as PropertyInfo;

                var isMatch =
                    (propertyInfo != null) &&
                    (propertyInfo.DeclaringType != null) &&
                    (
                        propertyInfo.DeclaringType == typeof(T) ||
                        propertyInfo.DeclaringType.IsAssignableFrom(typeof(T))
                    )
                    ;

                if (isMatch)
                {
                    _expressions.Add(node);
                }

                return base.VisitMember(node);
            }
        }
    }
}