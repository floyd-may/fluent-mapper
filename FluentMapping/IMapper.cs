namespace FluentMapping
{
    public interface IMapper<out TTarget, in TSource>
    {
        TTarget Map(TSource source);
    }
}