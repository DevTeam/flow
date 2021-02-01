namespace Flow.Core
{
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
#if !NET48
    using System.Activities.XamlIntegration;
#endif

    public static class Activities
    {
        public static IDictionary<string, object> Run(string activityName)
        {
            var assembly = Assembly.GetCallingAssembly();
            var activities =
                from type in assembly.DefinedTypes
                where type.Name.Equals(activityName, StringComparison.CurrentCultureIgnoreCase)
                where !type.IsAbstract
                where typeof(Activity).IsAssignableFrom(type)
                where type.DeclaredConstructors.Any(ctor => ctor.IsPublic && !ctor.GetParameters().Any())
                select (Activity)Activator.CreateInstance(type);

#if !NET48
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

            return WorkflowInvoker.Invoke(activity);
        }
    }
}
