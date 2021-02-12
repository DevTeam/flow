namespace Flow.Core
{
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ShProcessWrapper: IInitializableProcessWrapper<Path>
    {
        private Path _shPath;

        public ShProcessWrapper(
            [Tag(TempFile)] Path tempFilePath) =>
            _shPath = tempFilePath + ".sh";

        public void Initialize(Path shPath) => _shPath = shPath;

        public ProcessInfo Wrap(ProcessInfo processInfo)
        {
            return processInfo;
        }
    }
}
