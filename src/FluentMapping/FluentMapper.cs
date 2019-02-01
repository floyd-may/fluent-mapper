namespace FluentMapping
{
    public static class FluentMapper
    {
        public static TargetTypeSpec<TTarget> ThatMaps<TTarget>()
            where TTarget : class
        {
            return new TargetTypeSpec<TTarget>();
        }
    }

    public sealed class TargetTypeSpec<T>
        where T : class
    {
        public TypeMappingSpec<T, TSource> From<TSource>()
            where TSource : class
        {
            return new TypeMappingSpec<T, TSource>();
        }
    }
}