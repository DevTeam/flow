﻿namespace Flow.Activities
{
    using System;
    using System.Activities;
    using System.ComponentModel;
    using Core;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed class CommandLine : CodeActivity<ExitCode>
    {
        [RequiredArgument]
        [Category("Basic")]
        [DisplayName("Executable File")]
        public InArgument<Path> Executable { get; set; }

        [Category("Basic")]
        [DisplayName("Working Directory")]
        public InArgument<Path> WorkingDirectory { get; set; }

        [Category("Basic")]
        [DisplayName("Arguments")]
        public InArgument<Enumerable<CommandLineArgument>> Arguments { get; set; }

        [Category("Advanced")]
        [DisplayName("Environment Variables")]
        public InArgument<Enumerable<EnvironmentVariable>> EnvironmentVariables { get; set; }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            metadata.RequireExtension<ICommandLineService>();
        }

        protected override ExitCode Execute(CodeActivityContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return context.GetExtension<ICommandLineService>().Execute(
                new ProcessInfo(
                context.GetValue(Executable),
                context.GetValue(WorkingDirectory),
                context.GetValue(Arguments),
                context.GetValue(EnvironmentVariables))
            );
        }
    }
}
