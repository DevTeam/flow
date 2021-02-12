namespace Flow.Activities
{
    using System;
    using System.Activities;
    using Core;

    public sealed class CommandLine : CodeActivity<ExitCode>
    {
        [RequiredArgument]
        public InArgument<Path> Executable { get; set; }

        public InArgument<Path> WorkingDirectory { get; set; }

        public InArgument<Enumerable<CommandLineArgument>> Arguments { get; set; }

        public InArgument<Enumerable<EnvironmentVariable>> Variables { get; set; }

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
                context.GetValue(Variables)
                )
            );
        }
    }
}
