namespace Flow
{
    using System;
    using System.Collections.Generic;
    using Core;
    using IoC;

    public readonly struct DockerImage : IFromText<DockerImage>
    {
        [NotNull] public readonly string Name;

        public DockerImage([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));
            Name = name;
        }

        public static implicit operator DockerImage([NotNull] string imageName)
        {
            if (imageName == null) throw new ArgumentNullException(nameof(imageName));
            using (var enumerator = imageName.GetEnumerator())
            {
                return new DockerImage(enumerator.Parse());
            }
        }

        public override string ToString() => Name;

        DockerImage IFromText<DockerImage>.Parse(IEnumerator<char> text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            return new DockerImage(text.Parse());
        }
    }
}
