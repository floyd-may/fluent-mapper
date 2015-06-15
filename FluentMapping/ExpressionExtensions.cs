using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FluentMapping
{
    public static class ExpressionExtensions
    {
        public static Expression<Action<TTarget, TSource, TContext>> ToSetCall<TTarget, TSource, TProperty, TContext>(
            this Expression<Func<TTarget, TProperty>> tgtGetExpr, Expression<Func<TSource, TContext, TProperty>> getExpr)
        {
            var sourceParam = Expression.Parameter(typeof(TSource));
            var targetParam = Expression.Parameter(typeof(TTarget));
            var contextParam = Expression.Parameter(typeof(TContext));

            var targetPropertyInfo = ((MemberExpression)tgtGetExpr.Body).Member as PropertyInfo;

            var getInvokeExpr = Expression.Invoke(getExpr, sourceParam, contextParam);

            var setterExpr = Expression.Call(targetParam, targetPropertyInfo.GetSetMethod(), getInvokeExpr);

            var setterActionExpr = Expression.Lambda<Action<TTarget, TSource, TContext>>(setterExpr, targetParam,
                sourceParam, contextParam);

            return setterActionExpr;
        }

        public static Expression<Action<TTarget, TSource>> ToSetCall<TTarget, TSource, TProperty>(
            this Expression<Func<TTarget, TProperty>> tgtGetExpr, Expression<Func<TSource, TProperty>> getExpr)
        {
            var sourceParam = Expression.Parameter(typeof(TSource));
            var targetParam = Expression.Parameter(typeof(TTarget));

            var targetPropertyInfo = ((MemberExpression)tgtGetExpr.Body).Member as PropertyInfo;

            var getInvokeExpr = Expression.Invoke(getExpr, sourceParam);

            var setterExpr = Expression.Call(targetParam, targetPropertyInfo.GetSetMethod(), getInvokeExpr);

            var setterActionExpr = Expression.Lambda<Action<TTarget, TSource>>(setterExpr, targetParam,
                sourceParam);

            return setterActionExpr;
        }
    }
}