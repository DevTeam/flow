namespace Flow.Core
{
    internal readonly struct DockerWrapperInfo
    {
        public readonly DockerImage Image;

        public DockerWrapperInfo(DockerImage image)
        {
            Image = image;
        }
    }
}
