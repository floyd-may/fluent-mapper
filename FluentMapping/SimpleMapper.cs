using System;

namespace FluentMapping
{
    internal sealed class SimpleMapper<TTarget, TSource> : IMapper<TTarget, TSource>
    {
        private readonly Func<TTarget> _factory;
        private readonly Func<TTarget, TSource, TTarget> _mappingAction;

        public SimpleMapper(Func<TTarget> factory, Func<TTarget, TSource, TTarget> mappingAction)
        {
            _factory = factory;
            _mappingAction = mappingAction;
        }

        public TTarget Map(TSource source)
        {
            var target = _factory();

            return _mappingAction(target, source);
        }
    }
}