namespace Flow.Activities
{
    using System;
    using System.Activities;
    using System.ComponentModel;
    using Core;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Build : CodeActivity<BuildResult>
    {
        [Category("Advanced")]
        [DisplayName("Arguments")]
        [Description("Additional arguments")]
        public InArgument<Enumerable<CommandLineArgument>> Arguments { get; set; }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            metadata.RequireExtension<IDotnetBuildService>();
        }

        protected override BuildResult Execute(CodeActivityContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return context.GetExtension<IDotnetBuildService>().Execute(new DotnetBuildInfo(
                context.GetValue(Arguments)));
        }
    }
}
