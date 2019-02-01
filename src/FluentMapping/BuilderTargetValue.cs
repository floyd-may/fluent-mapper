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

        public bool IsSupersededBy(LambdaExpression expression)
        {
            var visitor = new BuilderMethodVisitor(_method);
            visitor.Visit(expression);


            return visitor.MethodIsPresent;
        }

        public override string ToString()
        {
            return Description;
        }

        private sealed class BuilderMethodVisitor : ExpressionVisitor
        {
            private readonly MethodInfo _method;

            public BuilderMethodVisitor(MethodInfo method)
            {
                _method = method;
            }

            public bool MethodIsPresent { get; private set; }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                var nodeParams = node.Method.GetParameters();

                var match = (node.Method.DeclaringType == _method.DeclaringType)
                            && (node.Method.Name == _method.Name)
                            && (node.Method.ReturnType == _method.ReturnType)
                            && (nodeParams.Length == 1)
                            && (nodeParams.First().ParameterType == _method.GetParameters().First().ParameterType)
                    ;
                MethodIsPresent = MethodIsPresent || match;

                return base.VisitMethodCall(node);
            }
        }
    }
}