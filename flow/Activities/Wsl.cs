﻿namespace Flow.Activities
{
    using System;
    using System.Activities;
    using System.Activities.Statements;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Core;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [Designer("System.Activities.Core.Presentation.SequenceDesigner, System.Activities.Core.Presentation, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
    public class Wsl : NativeActivity
    {
        private readonly Sequence _sequence = new Sequence();
        private IDisposable _token = Disposable.Empty;

        [Browsable(false)]
        public Collection<Activity> Activities => _sequence.Activities;

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            metadata.RequireExtension<IDockerWrapperService>();
            metadata.AddImplementationChild(_sequence);
        }

        protected override void Execute(NativeActivityContext context)
        {
            var wslWrapper = context.GetExtension<IWslWrapperService>();
            _token = wslWrapper.Using();
            context.ScheduleActivity(_sequence, OnCompleted, OnFaulted);
        }

        private void OnCompleted(NativeActivityContext ctx, ActivityInstance instance) =>
            _token.Dispose();

        private void OnFaulted(NativeActivityFaultContext ctx, Exception error, ActivityInstance instance) => 
            _token.Dispose();
    }
}
