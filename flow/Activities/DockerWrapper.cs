namespace Flow.Activities
{
    using System.Activities;
    using System.Activities.Statements;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Markup;

    [ContentProperty("Activities")]
    [Designer("System.Activities.Core.Presentation.SequenceDesigner, System.Activities.Core.Presentation, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
    public class DockerWrapper : NativeActivity
    {
        private readonly Sequence _sequence = new();

        [Browsable(false)]
        public Collection<Activity> Activities => _sequence.Activities;

        [Browsable(false)]
        public Collection<Variable> Variables => _sequence.Variables;

        protected override void CacheMetadata(NativeActivityMetadata metadata) =>
            metadata.AddImplementationChild(_sequence);

        protected override void Execute(NativeActivityContext context) => 
            context.ScheduleActivity(_sequence);
    }
}
