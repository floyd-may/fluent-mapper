using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FluentMapping
{
    public sealed class TargetValue<TTarget> : ITargetValue<TTarget>
    {
        private readonly PropertyInfo _propInfo;

        public TargetValue(PropertyInfo propInfo)
        {
            _propInfo = propInfo;
            PropertyName = propInfo.Name;

            ValueType = propInfo.PropertyType;

        }

        public string PropertyName { get; private set; }

        public Type ValueType { get; private set; }
        public string Description {
            get
            {
                return string.Format(
                    "{0} {1}.{2}",
                    ValueType.Name,
                    typeof(TTarget).Name,
                    PropertyName
                    );
            }
        }

        public Expression CreateSetter(Expression sourceExpression)
        {
            var sourceLambda = ((LambdaExpression) sourceExpression);

            var targetParamExpr = Expression.Parameter(typeof(TTarget));
            var sourceParamExpr = sourceLambda.Parameters[0];

            var setterExpr = Expression.Call(targetParamExpr, _propInfo.GetSetMethod(), sourceLambda.Body);

            var blockExpr = Expression.Block(setterExpr, targetParamExpr);

            return Expression.Lambda(blockExpr, targetParamExpr, sourceParamExpr);
        }

        public override string ToString()
        {
            return Description;
        }
    }
}