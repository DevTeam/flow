namespace Flow
{
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Core;
    using IoC;
#if !NET48
    using System.Activities.XamlIntegration;
#endif

    public static class Flows
    {
        public static IDictionary<string, object> Run([NotNull] string activityName, TimeSpan timeout)
        {
            if (activityName == null) throw new ArgumentNullException(nameof(activityName));

            var assembly = Assembly.GetCallingAssembly();

            // Get activity by name from declared types
            var activities =
                from type in assembly.DefinedTypes
                where type.Name.Equals(activityName, StringComparison.CurrentCultureIgnoreCase)
                where !type.IsAbstract
                where typeof(Activity).IsAssignableFrom(type)
                where type.DeclaredConstructors.Any(ctor => ctor.IsPublic && !ctor.GetParameters().Any())
                select (Activity)Activator.CreateInstance(type);

#if !NET48
                // Get xaml activity by name from resources
                activities = activities.Concat(
                    from resourceName in assembly.GetManifestResourceNames()
                    where resourceName.EndsWith($"{activityName}.xaml", StringComparison.InvariantCultureIgnoreCase)
                    let stream = assembly.GetManifestResourceStream(resourceName)
                    select ActivityXamlServices.Load(stream));
#endif

            var activity = activities.FirstOrDefault();
            if (activity == null)
            {
                throw new ArgumentException(nameof(activityName), $"Cannot find \"{activityName}\".");
            }

            return Run(activity, timeout);
        }

        public static IDictionary<string, object> Run([NotNull] this Activity activity, TimeSpan timeout)
        {
            if (activity == null) throw new ArgumentNullException(nameof(activity));
            using (var compositionRoot = Container.Create().Using<Configuration>().BuildUp<FlowEntry>())
            {
                return compositionRoot.Instance.Run(activity, timeout);
            }
        }
    }
}
