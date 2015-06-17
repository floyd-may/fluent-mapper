using System;

namespace FluentMapping
{
    internal sealed class SimpleContextualMapper<TTarget, TSource, TContext> : IContextualMapper<TTarget, TSource, TContext>
    {
        private readonly Func<TSource, TContext, TTarget> _constructor;
        private readonly Func<TTarget, TSource, TTarget> _mappingFunc;
        private readonly Func<TTarget, TSource, TContext, TTarget> _contextMappingAction;

        public SimpleContextualMapper(
            Func<TSource, TContext, TTarget> constructor,
            Func<TTarget, TSource, TTarget> mappingFunc, 
            Func<TTarget, TSource, TContext, TTarget> contextMappingAction)
        {
            _constructor = constructor;
            _mappingFunc = mappingFunc;
            _contextMappingAction = contextMappingAction;
        }

        public TTarget Map(TSource source, TContext context)
        {
            var target = _mappingFunc(_constructor(source, context), source);
            
            return _contextMappingAction(target, source, context);
        }

        public IMapper<TTarget, TSource> BindContext(TContext context)
        {
            return new SimpleMapper<TTarget, TSource>(src => Map(src, context));
        }
    }

    internal sealed class ContextualMapper<TTarget, TSource, TContext> : IContextualMapper<TTarget, TSource, TContext>
    {
        private readonly Func<TSource, TContext, TTarget> _factory;

        public ContextualMapper(Func<TSource, TContext, TTarget> factory)
        {
            _factory = factory;
        }

        public TTarget Map(TSource source, TContext context)
        {
            return _factory(source, context);
        }

        public IMapper<TTarget, TSource> BindContext(TContext context)
        {
            return new SimpleMapper<TTarget, TSource>(src => _factory(src, context));
        }
    }
}