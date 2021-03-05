namespace Flow.Core
{
    using JetBrains.TeamCity.ServiceMessages;

    internal interface IBuildLogFlow
    {
        void Initialize(int tabs);

        IBuildLogFlow CreateChild();

        bool ProcessMessage(IServiceMessageProcessor processor, IBuildVisitor buildVisitor, IServiceMessage message);
    }
}
