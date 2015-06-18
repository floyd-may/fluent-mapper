using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentMapping.Internal;

namespace FluentMapping
{
    public static class TypeMappingSpecExtensions
    {
        public static TypeMappingSpec<TTarget, TSource> WithTargetAsBuilder<TTarget, TSource>(this TypeMappingSpec<TTarget, TSource> spec)
            where TTarget : class
            where TSource : class
        {
            MethodInfo[] methods;
            if (typeof (TTarget).IsInterface)
                methods = new[] {typeof (TTarget)}.Concat(typeof (TTarget).GetInterfaces())
                    .SelectMany(x => x.GetMethods())
                    .ToArray();
            else
                methods = typeof (TTarget).GetMethods();

            var builderTargetValues = methods
                .Where(x => x.Name.StartsWith("With"))
                .Where(x => x.ReturnType == typeof (TTarget))
                .Where(x => x.GetParameters().Length == 1)
                .Select(x => new BuilderTargetValue<TTarget>(x))
                ;

            return spec.WithTargetValues(spec.TargetValues.Concat(builderTargetValues));
        }

        public static TypeMappingSpec<TTarget, TSource> WithCustomMap<TTarget, TSource>(
            this TypeMappingSpec<TTarget, TSource> spec,
            Expression<Action<TTarget, TSource>> customMappingExpression
            )
            where TTarget : class
            where TSource : class
        {
            return spec
                .IgnoringNestedSourceProperty(customMappingExpression)
                .WithTargetValues(spec.TargetValues.Where(v => !v.IsSupersededBy(customMappingExpression)))
                .WithCustomMapper(customMappingExpression);
        }

        public static NullSourceBehavior<TTarget, TSource> WithNullSource<TTarget, TSource>(
            this TypeMappingSpec<TTarget, TSource> spec)
            where TTarget : class
            where TSource : class
        {
            return new NullSourceBehavior<TTarget, TSource>(spec);
        }
    }
}