namespace Flow.Core
{
    internal interface IInitializableProcessWrapper<in TState>: IProcessWrapper
    {
        void Initialize(TState state);
    }
}
