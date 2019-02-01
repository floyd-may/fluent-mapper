using System;

namespace FluentMapping
{
    public interface IAssembler
    {
        IMapper<TTarget, TSource> Assemble<TSource, TTarget>(
            Func<TSource, TTarget> constructor,
            Func<TTarget, TSource, TTarget> mappingFunc)
            where TTarget : class
            where TSource : class;

        IContextualMapper<TTarget, TSource, TContext> Assemble<TTarget, TSource, TContext>(
            Func<TSource, TContext, TTarget> constructor,
            Func<TTarget, TSource, TContext, TTarget> mappingFunc)
            where TTarget : class
            where TSource : class;
    }
}