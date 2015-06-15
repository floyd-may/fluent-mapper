namespace FluentMapping
{
    public interface IContextualMapper<out TTarget, in TSource, in TContext>
    {
        TTarget Map(TSource source, TContext context);
        IMapper<TTarget, TSource> BindContext(TContext context);
    }
}