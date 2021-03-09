namespace Flow.Core
{
    using JetBrains.TeamCity.ServiceMessages;

    internal interface IBuildLogFlow
    {
        void Initialize(int tabs);

        IBuildLogFlow CreateChild();

        bool ProcessMessage(IMessageProcessor processor, IBuildVisitor buildVisitor, IServiceMessage message);
    }
}
