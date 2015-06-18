using System;

namespace FluentMapping
{
    public sealed class EmptyObjectAssembler : IAssembler
    {
        public IMapper<TTarget, TSource> Assemble<TSource, TTarget>(
                Func<TSource, TTarget> constructor, 
                Func<TTarget, TSource, TTarget> mappingFunc
            ) 
            where TSource : class 
            where TTarget : class
        {
            return new SimpleMapper<TTarget, TSource>(src =>
            {
                var result = constructor(src);
                if (src == null)
                    return result;

                return mappingFunc(result, src);
            });
        }

        public IContextualMapper<TTarget, TSource, TContext> 
            Assemble<TTarget, TSource, TContext>(
                Func<TSource, TContext, TTarget> constructor, 
                Func<TTarget, TSource, TContext, TTarget> mappingFunc
            ) 
            where TTarget : class 
            where TSource : class
        {
            return new ContextualMapper<TTarget, TSource, TContext>((src, ctx) =>
            {
                var result = constructor(src, ctx);
                if (src == null)
                    return result;

                return mappingFunc(result, src, ctx);
            });
        }
    }
}