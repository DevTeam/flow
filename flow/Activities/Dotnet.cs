namespace Flow.Activities
{
    using System;
    using System.Activities;
    using System.Activities.Statements;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Core;

    [Designer("System.Activities.Core.Presentation.SequenceDesigner, System.Activities.Core.Presentation, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
    public class Dotnet : NativeActivity
    {
        private readonly Sequence _sequence = new Sequence();
        private IDisposable _token = Disposable.Empty;

        [Category("Basic")]
        [DisplayName("Solution Directory")]
        public InArgument<Path> SolutionDirectory { get; set; }

        [Category("Advanced")]
        [DisplayName("Dotnet")]
        [Description("Dotnet executable path")]
        public InArgument<Path> DotnetExecutable { get; set; }

        [Category("Advanced")]
        [DisplayName("Environment Variables")]
        public InArgument<Enumerable<EnvironmentVariable>> EnvironmentVariables { get; set; }

        [Browsable(false)]
        public Collection<Activity> Activities => _sequence.Activities;

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            metadata.RequireExtension<IDotnetWrapperService>();
            metadata.AddImplementationChild(_sequence);
        }

        protected override void Execute(NativeActivityContext context)
        {
            var dotnetWrapper = context.GetExtension<IDotnetWrapperService>();
            var solutionDirectory = context.GetValue(SolutionDirectory);
            if (solutionDirectory.IsEmpty)
            {
                solutionDirectory = new Path("../../..");
            }

            _token = dotnetWrapper.Using(
                new DotnetWrapperInfo(
                    solutionDirectory,
                    context.GetValue(DotnetExecutable),
                    context.GetValue(EnvironmentVariables)));
            context.ScheduleActivity(_sequence, OnCompleted, OnFaulted);
        }

        private void OnCompleted(NativeActivityContext ctx, ActivityInstance instance) =>
            _token.Dispose();

        private void OnFaulted(NativeActivityFaultContext ctx, Exception error, ActivityInstance instance) => 
            _token.Dispose();
    }
}
