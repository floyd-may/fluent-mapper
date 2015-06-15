using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FluentMapping
{
    public sealed class SourceValue<TSource> : IValue<TSource>
    {
        private readonly PropertyInfo _propInfo;

        public SourceValue(PropertyInfo propInfo)
        {
            _propInfo = propInfo;
            PropertyName = propInfo.Name;

            ValueType = propInfo.PropertyType;
        }

        public string PropertyName { get; private set; }

        public Type ValueType { get; private set; }

        public override string ToString()
        {
            return Description;
        }

        public string Description
        {
            get
            {
                return string.Format(
                    "{0} {1}.{2}",
                    ValueType.Name,
                    typeof(TSource).Name,
                    PropertyName
                    );
            }
        }

        public Expression CreateGetter()
        {
            var paramExpr = Expression.Parameter(typeof (TSource));

            var getterExpr = Expression.Property(paramExpr, _propInfo);

            return Expression.Lambda(getterExpr, paramExpr);
        }
    }
}