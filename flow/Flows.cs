namespace Flow
{
    using System;
    using System.Activities;
    using System.Activities.Hosting;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using IoC;
    using Services;

#if !NET48
    using System.Activities.XamlIntegration;
#endif

    public static class Flows
    {
        private static readonly MethodInfo ResolveMethodInfo = typeof(FluentResolve).GetMethods().First(i => i.Name == nameof(FluentResolve.Resolve) && i.IsPublic && i.IsStatic && i.ReturnParameter?.ParameterType.IsGenericParameter == true && i.GetParameters().Length == 2 && i.GetParameters()[0].ParameterType == typeof(IContainer) && i.GetParameters()[1].ParameterType == typeof(Type));
        private static readonly MethodInfo AddExtensionMethodInfo = typeof(WorkflowInstanceExtensionManager).GetMethods().First(i => i.Name == nameof(WorkflowInstanceExtensionManager.Add) && i.IsPublic && !i.IsStatic && i.IsGenericMethod && i.GetParameters().Length == 1 && i.GetParameters()[0].ParameterType.IsGenericType && i.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Func<>));

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

            var invoker = new WorkflowInvoker(activity);
            using (invoker.ConfigureExtensions())
            {
                return invoker.Invoke(timeout);
            }
        }

        private static IDisposable ConfigureExtensions([NotNull] this WorkflowInvoker invoker)
        {
            if (invoker == null) throw new ArgumentNullException(nameof(invoker));

            var container = Container.Create().Using<Configuration>();
            // Configure extensions by bindings from the container

            var extensionKeys =
                from keyGroup in container
                from key in keyGroup
                where key.Tag == null || key.Tag == Key.AnyTag
                where key.Type.GetCustomAttribute<PublicAttribute>() != null
                select key;

            var containerConst = Expression.Constant(container);
            foreach (var key in extensionKeys)
            {
                var typeConst = Expression.Constant(key.Type);
                var factory = Expression.Lambda(Expression.Call(null, ResolveMethodInfo.MakeGenericMethod(key.Type), containerConst, typeConst)).Compile();
                var addMethod = AddExtensionMethodInfo.MakeGenericMethod(key.Type);
                addMethod.Invoke(invoker.Extensions, new object[] { factory });
            }

            return container;
        }
    }
}
