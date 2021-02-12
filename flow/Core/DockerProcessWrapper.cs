namespace Flow.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DockerProcessWrapper: IInitializableProcessWrapper<DockerWrapperInfo>
    {
        private bool _initialized;
        private DockerWrapperInfo _wrapperInfo;

        public void Initialize(DockerWrapperInfo wrapperInfo)
        {
            _wrapperInfo = wrapperInfo;
            _initialized = true;
        }

        public ProcessInfo Wrap(ProcessInfo processInfo)
        {
            if (!_initialized) throw new InvalidOperationException("Not initialized");
            return new ProcessInfo(
                "docker.exe",
                processInfo.WorkingDirectory,
                GetArgs(processInfo),
                processInfo.Variables);
        }

        private IEnumerable<CommandLineArgument> GetArgs(ProcessInfo processInfo)
        {
            yield return "run";
            yield return "-it";
            yield return "--rm";
            var paths = processInfo
                .Arguments
                .Where(i => i.Type == CommandLineArgumentType.Path)
                .Select(i => System.IO.Path.GetDirectoryName(i.Value))
                .Distinct();

            foreach (var path in paths)
            {
                yield return "-v";
                yield return $"{path}:{path}";
            }
            
            yield return _wrapperInfo.Image.Name;
            yield return processInfo.Executable.Value;
            foreach (var arg in processInfo.Arguments)
            {
                yield return arg;
            }
        }
    }
}
