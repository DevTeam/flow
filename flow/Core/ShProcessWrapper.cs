namespace Flow.Core
{
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ShProcessWrapper: IProcessWrapper
    {
        private Path _shPath;

        public ShProcessWrapper(
            [Tag(TempFile)] Path tempFilePath) =>
            _shPath = tempFilePath + ".sh";

        public ProcessInfo Wrap(ProcessInfo processInfo)
        {
            return processInfo;
        }
    }
}