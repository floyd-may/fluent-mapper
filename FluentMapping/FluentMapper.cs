namespace FluentMapping
{
    public static class FluentMapper
    {
        public static TargetTypeSpec<TTarget> ThatMaps<TTarget>()
        {
            return new TargetTypeSpec<TTarget>();
        }
    }

    public sealed class TargetTypeSpec<T>
    {
        public TypeMappingSpec<T, TSource> From<TSource>()
        {
            return new TypeMappingSpec<T, TSource>();
        }
    }
}