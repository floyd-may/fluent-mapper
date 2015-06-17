using System;

namespace FluentMapping
{
    public class NullSourceAssembler : IAssembler
    {
        public IMapper<TTarget, TSource> Assemble<TSource, TTarget>(Func<TSource, TTarget> constructor, Func<TTarget, TSource, TTarget> mappingFunc)
            where TTarget : class
            where TSource : class
        {
            return new SimpleMapper<TTarget, TSource>(src =>
            {
                if (src == null)
                    return null;

                return mappingFunc(constructor(src), src);
            });
        }

        public IContextualMapper<TTarget, TSource, TContext> Assemble<TTarget, TSource, TContext>(Func<TSource, TContext, TTarget> constructor, Func<TTarget, TSource, TContext, TTarget> mappingFunc) where TTarget : class where TSource : class
        {
            return new ContextualMapper<TTarget, TSource, TContext>((src, ctx) =>
            {
                if (src == null)
                    return null;

                return mappingFunc(constructor(src, ctx), src, ctx);
            });
        }
    }
}