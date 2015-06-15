using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentMapping.Internal;

namespace FluentMapping
{
    public static class TypeMappingSpecExtensions
    {
        public static TypeMappingSpec<TTarget, TSource> IgnoringTargetProperty<TTarget, TSource, TProperty>(this TypeMappingSpec<TTarget, TSource> spec, Expression<Func<TTarget, TProperty>> propertyExpression)
        {
            var propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;

            return spec.IgnoringTargetProperty(propertyName);
        }

        public static TypeMappingSpec<TTarget, TSource> IgnoringTargetProperty<TTarget, TSource>(this TypeMappingSpec<TTarget, TSource> spec,
            string propertyName)
        {
            return spec.WithTargetValues(spec.TargetValues.Where(x => x.PropertyName != propertyName));
        }

        public static TypeMappingSpec<TTarget, TSource> IgnoringSourceProperty<TTarget, TSource, TProperty>(this TypeMappingSpec<TTarget, TSource> spec, Expression<Func<TSource, TProperty>> propertyExpression)
        {
            var propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;

            return spec.IgnoringSourceProperty(propertyName);
        }

        public static TypeMappingSpec<TTarget, TSource> IgnoringSourceProperty<TTarget, TSource>(this TypeMappingSpec<TTarget, TSource> spec,
            string propertyName)
        {
            return spec.WithSourceValues(spec.SourceValues.Where(x => x.PropertyName != propertyName));
        }

        public static TypeMappingSpec<TTarget, TSource> WithTargetAsBuilder<TTarget, TSource>(this TypeMappingSpec<TTarget, TSource> spec)
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

        public static TypeMappingSpec<TTarget, TSource> WithConstructor<TTarget, TSource>(
            this TypeMappingSpec<TTarget, TSource> spec, Func<TTarget> constructorFunc)
        {
            return new TypeMappingSpec<TTarget, TSource>(
                spec.TargetValues.ToArray(),
                spec.SourceValues.ToArray(),
                spec.CustomMappings.ToArray(),
                constructorFunc
                );
        }

        public static TypeMappingSpec<TTarget, TSource> WithCustomMap<TTarget, TSource>(
            this TypeMappingSpec<TTarget, TSource> spec,
            Expression<Action<TTarget, TSource>> customMappingExpression
            )
        {
            return spec
                .IgnoringNestedSourceProperty(customMappingExpression)
                .WithCustomMapper(customMappingExpression);
        }
    }
}