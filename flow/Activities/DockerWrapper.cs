namespace Flow.Activities
{
    using System;
    using System.Activities;
    using System.Activities.Statements;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Core;

    [Designer("System.Activities.Core.Presentation.SequenceDesigner, System.Activities.Core.Presentation, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
    public class DockerWrapper : NativeActivity
    {
        private readonly Sequence _sequence = new Sequence();
        private IDisposable _token = Disposable.Empty;

        public DockerWrapper()
        {
            DisplayName = "Docker";
        }

        [RequiredArgument]
        [Category("Basic")]
        [Description("Docker image name")]
        public InArgument<DockerImage> Image { get; set; }

        [Category("Basic")]
        [Description("Docker container platform")]
        public Flow.OperatingSystem Platform { get; set; }

        // Automatically remove the container
        [Category("Advanced")]
        [DisplayName("Auto-remove")]
        [Description("Automatically remove the container after each activity")]
        public bool AutomaticallyRemove { get; set; } = true;

        // Pull image before running
        [Category("Advanced")]
        [Description("Pull image before running")]
        public DockerPullType Pull { get; set; } = DockerPullType.Missing;

        [Category("Advanced")]
        [Description("Bind mount volumes")]
        public InArgument<Enumerable<DockerVolume>> Volumes { get; set; }

        [Category("Advanced")]
        [DisplayName("Docker arguments")]
        [Description("Additional docker \"run\" command line arguments")]
        public InArgument<Enumerable<CommandLineArgument>> DockerArguments { get; set; }

        [Category("Advanced")]
        [DisplayName("Environment Variables")]
        [Description("Docker environment variables")]
        public InArgument<Enumerable<EnvironmentVariable>> DockerEnvironmentVariables { get; set; }

        [Browsable(false)]
        public Collection<Activity> Activities => _sequence.Activities;

        [Browsable(false)]
        public Collection<Variable> Variables => _sequence.Variables;

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            metadata.RequireExtension<IDockerWrapperService>();
            metadata.AddImplementationChild(_sequence);
        }

        protected override void Execute(NativeActivityContext context)
        {
            var dockerWrapper = context.GetExtension<IDockerWrapperService>();
            _token = dockerWrapper.Using(
                new DockerWrapperInfo(
                    context.GetValue(Image),
                    context.GetValue(Volumes),
                    context.GetValue(DockerArguments),
                    context.GetValue(DockerEnvironmentVariables),
                    Platform,
                    AutomaticallyRemove,
                    Pull));
            context.ScheduleActivity(_sequence, OnCompleted, OnFaulted);
        }

        private void OnCompleted(NativeActivityContext ctx, ActivityInstance instance) =>
            _token.Dispose();

        private void OnFaulted(NativeActivityFaultContext ctx, Exception error, ActivityInstance instance) => 
            _token.Dispose();
    }
}
