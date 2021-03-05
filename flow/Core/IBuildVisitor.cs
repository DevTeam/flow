namespace Flow.Core
{
    internal interface IBuildVisitor
    {
        void Visit(BuildError error);

        void Visit(BuildWarning warning);
    }
}
