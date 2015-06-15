using System;

namespace FluentMapping
{
    internal sealed class SimpleContextualMapper<TTarget, TSource, TContext> : IContextualMapper<TTarget, TSource, TContext>
    {
        private readonly IMapper<TTarget, TSource> _childMapper;
        private readonly Action<TTarget, TSource, TContext> _contextMappingAction;

        public SimpleContextualMapper(IMapper<TTarget, TSource> childMapper) :
            this(childMapper, delegate { })
        {
            
        }

        public SimpleContextualMapper(IMapper<TTarget, TSource> childMapper, Action<TTarget, TSource, TContext> contextMappingAction)
        {
            _childMapper = childMapper;
            _contextMappingAction = contextMappingAction;
        }

        public TTarget Map(TSource source, TContext context)
        {
            var target = _childMapper.Map(source);
            _contextMappingAction(target, source, context);
            return target;
        }

        public IMapper<TTarget, TSource> BindContext(TContext context)
        {
            return new Mapper(_childMapper, context, _contextMappingAction);
        }

        private sealed class Mapper : IMapper<TTarget, TSource>
        {
            private readonly IMapper<TTarget, TSource> _mapper;
            private readonly TContext _context;
            private readonly Action<TTarget, TSource, TContext> _contextMappingAction;

            public Mapper(IMapper<TTarget, TSource> mapper, TContext context, Action<TTarget, TSource, TContext> contextMappingAction)
            {
                _mapper = mapper;
                _context = context;
                _contextMappingAction = contextMappingAction;
            }

            public TTarget Map(TSource source)
            {
                var target = _mapper.Map(source);
                _contextMappingAction(target, source, _context);

                return target;
            }
        }
    }
}