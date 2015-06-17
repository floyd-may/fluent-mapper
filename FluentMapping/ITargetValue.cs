using System.Linq.Expressions;

namespace FluentMapping
{
    public interface ITargetValue<TTarget> : IValue<TTarget>
    {
        Expression CreateSetter(Expression sourceExpression);

        bool IsSupersededBy(LambdaExpression expression);
    }
}