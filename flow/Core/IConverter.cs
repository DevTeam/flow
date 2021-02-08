namespace Flow.Core
{
    internal interface IConverter<in TSource, out TDestination>
    {
        TDestination Convert(TSource source);
    }
}
