using System;
using System.Linq;
using System.Linq.Expressions;
using FluentMapping.Internal;

namespace FluentMapping
{
    public partial class TypeMappingSpec<TTarget, TSource>
    {
        public TypeMappingSpec<TTarget, TSource> IgnoringTargetProperty<TProperty>(Expression<Func<TTarget, TProperty>> propertyExpression)
        {
            var propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;

            return this.IgnoringTargetProperty(propertyName);
        }

        public TypeMappingSpec<TTarget, TSource> IgnoringTargetProperty(string propertyName)
        {
            return this.WithTargetValues(this.TargetValues.Where(x => x.PropertyName != propertyName));
        }

        public TypeMappingSpec<TTarget, TSource> IgnoringSourceProperty<TProperty>(Expression<Func<TSource, TProperty>> propertyExpression)
        {
            var propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;

            return this.IgnoringSourceProperty(propertyName);
        }

        public TypeMappingSpec<TTarget, TSource> IgnoringSourceProperty(string propertyName)
        {
            return this.WithSourceValues(this.SourceValues.Where(x => x.PropertyName != propertyName));
        }
    }
}