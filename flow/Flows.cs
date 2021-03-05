namespace Flow
{
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using Core;
    using IoC;
#if !NET48
    using System.Activities.XamlIntegration;
#endif

    public static class Flows
    {
        [NotNull] public static IDictionary<string, object> Run([NotNull] string activityName, [NotNull] IDictionary<string, object> inputs, TimeSpan timeout, Verbosity verbosity = Verbosity.Normal)
        {
            if (activityName == null) throw new ArgumentNullException(nameof(activityName));
            if (inputs == null) throw new ArgumentNullException(nameof(inputs));

            using (var compositionRoot = Container.Create().Using(new Configuration(verbosity)).BuildUp<FlowEntry>())
            {
                return compositionRoot.Instance.Run(activityName, inputs, timeout);
            }
        }

        [NotNull] public static IDictionary<string, object> Run([NotNull] this Activity activity, [NotNull] IDictionary<string, object> inputs, TimeSpan timeout, Verbosity verbosity = Verbosity.Normal)
        {
            if (activity == null) throw new ArgumentNullException(nameof(activity));
            if (inputs == null) throw new ArgumentNullException(nameof(inputs));

            using (var compositionRoot = Container.Create().Using(new Configuration(verbosity)).BuildUp<FlowEntry>())
            {
                return compositionRoot.Instance.Run(activity, inputs, timeout);
            }
        }
    }
}