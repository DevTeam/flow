namespace Flow.Core
{
    using JetBrains.TeamCity.ServiceMessages;

    internal interface IBuildLogFlow
    {
        IBuildLogFlow CreateChild();

        bool ProcessMessage(IMessageProcessor processor, IBuildVisitor buildVisitor, IServiceMessage message);
    }
}
