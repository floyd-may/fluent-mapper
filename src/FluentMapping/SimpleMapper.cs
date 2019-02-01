using System;

namespace FluentMapping
{
    internal sealed class SimpleMapper<TTarget, TSource> : IMapper<TTarget, TSource>
    {
        private readonly Func<TSource, TTarget> _factory;

        public SimpleMapper(Func<TSource, TTarget> factory)
        {
            _factory = factory;
        }

        public TTarget Map(TSource source)
        {
            return _factory(source);
        }
    }
}