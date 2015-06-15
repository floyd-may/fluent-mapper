using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FluentMapping
{
    public class BuilderTargetValue<TTarget> : ITargetValue<TTarget>
    {
        private readonly MethodInfo _method;

        public BuilderTargetValue(MethodInfo method)
        {
            _method = method;
        }

        public string PropertyName { get { return _method.Name.Substring(4); } }
        public Type ValueType { get { return _method.GetParameters().First().ParameterType; } }
        public string Description
        {
            get
            {
                return string.Format(
                    "{0}.{1}({2})",
                    typeof(TTarget).Name,
                    _method.Name,
                    ValueType.Name
                    );
            }
        }
        
        public Expression CreateSetter(Expression sourceExpression)
        {
            var sourceLambda = ((LambdaExpression)sourceExpression);

            var targetParamExpr = Expression.Parameter(typeof(TTarget));
            var sourceParamExpr = sourceLambda.Parameters[0];

            var callExpr = Expression.Call(targetParamExpr, _method, sourceLambda.Body);

            return Expression.Lambda(callExpr, targetParamExpr, sourceParamExpr);
        }

        public override string ToString()
        {
            return Description;
        }
    }
}