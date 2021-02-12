﻿namespace Flow.Activities
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

        [RequiredArgument]
        public InArgument<DockerImage> Image { get; set; }

        [Browsable(false)]
        public Collection<Activity> Activities => _sequence.Activities;

        [Browsable(false)]
        public Collection<Variable> Variables => _sequence.Variables;

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            metadata.AddImplementationChild(_sequence);
        }

        protected override void Execute(NativeActivityContext context)
        {
            var dockerWrapper = context.GetExtension<IDockerWrapperService>();
            _token = dockerWrapper.Using(new DockerWrapperInfo(context.GetValue(Image)));
            context.ScheduleActivity(_sequence, OnCompleted, OnFaulted);
        }

        private void OnCompleted(NativeActivityContext ctx, ActivityInstance instance) =>
            _token.Dispose();

        private void OnFaulted(NativeActivityFaultContext ctx, Exception error, ActivityInstance instance) => 
            _token.Dispose();
    }
}