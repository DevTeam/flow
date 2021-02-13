namespace Flow
{
    using System;
    using System.Collections.Generic;
    using Core;
    using IoC;

    public readonly struct DockerVolume : IFromText<DockerVolume>
    {
        public readonly Path HostPath;
        public readonly Path ContainerPath;

        public DockerVolume(Path hostPath, Path containerPath)
        {
            HostPath = hostPath;
            ContainerPath = containerPath;
        }

        public static implicit operator DockerVolume([NotNull] string variable)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            using (var enumerator = variable.GetEnumerator())
            {
                var (name, value) = enumerator.ParseTuple(':');
                return new DockerVolume(name, value);
            }
        }

        public override string ToString() => $"{HostPath}:{ContainerPath}";

        DockerVolume IFromText<DockerVolume>.Parse(IEnumerator<char> text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            var (hostPath, containerPath) = text.ParseTuple(':');
            return new DockerVolume(hostPath, containerPath);
        }
    }
}