namespace Flow.Services
{
    internal interface IConverter<in TSource, out TDestination>
    {
        TDestination Convert(TSource source);
    }
}
