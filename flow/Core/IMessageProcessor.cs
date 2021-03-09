namespace Flow.Core
{
    using IoC;

    internal interface IMessageProcessor
    {
        bool ProcessServiceMessages([CanBeNull] string text, [NotNull] IBuildVisitor buildVisitor);
    }
}
