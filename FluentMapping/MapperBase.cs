using System;

namespace FluentMapping
{
    public abstract class MapperBase<TTarget, TSource> : IMapper<TTarget, TSource> 
        where TTarget : class
        where TSource : class
    {
        private Lazy<IMapper<TTarget, TSource>> _lzMapper;

        protected MapperBase()
        {
            _lzMapper = new Lazy<IMapper<TTarget, TSource>>(() => Configure(FluentMapper.ThatMaps<TTarget>().From<TSource>()).Create());
        }

        public TTarget Map(TSource source)
        {
            return _lzMapper.Value.Map(source);
        }

        protected abstract TypeMappingSpec<TTarget, TSource> Configure(TypeMappingSpec<TTarget, TSource> mappingSpec);
    }
}