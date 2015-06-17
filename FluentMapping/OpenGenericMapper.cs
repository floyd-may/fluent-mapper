using System;

namespace FluentMapping
{
    public class OpenGenericMapper<TTarget, TSource> : IMapper<TTarget, TSource>
        where TTarget : class
        where TSource : class
    {
        private readonly Lazy<IMapper<TTarget, TSource>> _childMapper; 

        public OpenGenericMapper()
        {
            _childMapper = new Lazy<IMapper<TTarget, TSource>>(() => FluentMapper.ThatMaps<TTarget>().From<TSource>().Create());
        }

        public TTarget Map(TSource source)
        {
            return _childMapper.Value.Map(source);
        }
    }
}