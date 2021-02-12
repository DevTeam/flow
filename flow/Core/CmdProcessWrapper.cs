namespace Flow.Core
{
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class CmdProcessWrapper: IInitializableProcessWrapper<Path>
    {
        private Path _cmdPath;

        public CmdProcessWrapper(
            [Tag(TempFile)] Path tempFilePath) =>
            _cmdPath = tempFilePath + ".cmd";

        public void Initialize(Path cmdPath) => _cmdPath = cmdPath;

        public ProcessInfo Wrap(ProcessInfo processInfo)
        {
            return processInfo;
        }
    }
}
