namespace Flow.Services
{
    using IoC;

    internal interface IEnvironment
    {
        Path WorkingDirectory { get; }
    }
}
