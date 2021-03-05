namespace Flow.Core
{
    using IoC;

    internal interface IServiceMessageProcessor
    {
        bool ProcessServiceMessages([CanBeNull] string text);
    }
}
