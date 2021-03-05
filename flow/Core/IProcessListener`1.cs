namespace Flow.Core
{
    internal interface IProcessListener<out T>: IProcessListener
    {
        T Result { get; }
    }
}
