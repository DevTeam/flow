namespace Flow.Core
{
    using System;
    using System.Activities;
    using System.Activities.Hosting;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using IoC;
#if !NET48
    using System.Activities.XamlIntegration;
#endif

    // ReSharper disable once ClassNeverInstantiated.Global
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class FlowEntry
    {
        [NotNull] private readonly IContainer _container;
        [NotNull] private readonly ISettingsSetup _settingsSetup;
        [NotNull] private readonly ISettings _settings;
        [NotNull] private readonly ILog<FlowEntry> _log;
        [NotNull] private readonly IStages _stages;
        private static readonly MethodInfo ResolveMethodInfo = typeof(FluentResolve).GetMethods().First(i => i.Name == nameof(FluentResolve.Resolve) && i.IsPublic && i.IsStatic && i.ReturnParameter?.ParameterType.IsGenericParameter == true && i.GetParameters().Length == 2 && i.GetParameters()[0].ParameterType == typeof(IContainer) && i.GetParameters()[1].ParameterType == typeof(Type));
        private static readonly MethodInfo AddExtensionMethodInfo = typeof(WorkflowInstanceExtensionManager).GetMethods().First(i => i.Name == nameof(WorkflowInstanceExtensionManager.Add) && i.IsPublic && !i.IsStatic && i.IsGenericMethod && i.GetParameters().Length == 1 && i.GetParameters()[0].ParameterType.IsGenericType && i.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Func<>));

        public FlowEntry(
            [NotNull] IContainer container,
            [NotNull] ISettingsSetup settingsSetup,
            [NotNull] ISettings settings,
            [NotNull] ILog<FlowEntry> log,
            [NotNull] IStages stages)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _settingsSetup = settingsSetup ?? throw new ArgumentNullException(nameof(settingsSetup));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _stages = stages;
        }

        public int Run([NotNull] IEnumerable<string> args)
        {
            if (!_settingsSetup.Setup(args))
            {
                return 1;
            }

            Run(_settings.ActivityName, _settings.Inputs, _settings.Timeout, _settings.Verbosity);
            return 0;
        }

        public IDictionary<string, object> Run([NotNull] string activityName, [NotNull] IDictionary<string, object> inputs, TimeSpan timeout, Verbosity verbosity)
        {
            if (activityName == null) throw new ArgumentNullException(nameof(activityName));
            if (inputs == null) throw new ArgumentNullException(nameof(inputs));

            _log.Info(() => new []{ new Text("Activity name: ", Color.Header), new Text(activityName, Color.Header) });

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

            return Run(activity, inputs, timeout, verbosity);
        }

        public IDictionary<string, object> Run([NotNull] Activity activity, [NotNull] IDictionary<string, object> inputs, TimeSpan timeout, Verbosity verbosity)
        {
            if (activity == null) throw new ArgumentNullException(nameof(activity));
            if (inputs == null) throw new ArgumentNullException(nameof(inputs));

            _settingsSetup.Setup(verbosity);
            _log.Info(() => new[] { new Text("Activity: ", Color.Header), new Text(activity.DisplayName, Color.Header) });
            _log.Trace(() =>
                {
                    var text = new List<Text>
                    {
                        "Activity type: ",
                        activity.GetType().FullName,
                        Text.NewLine,
                        "Timeout: ",
                        timeout.ToString("G"),
                        Text.NewLine,
                        "Inputs: ",
                    };

                    if (inputs.Count > 0)
                    {
                        text.Add("Inputs: ");
                        text.Add(Text.NewLine);
                        foreach (var input in inputs)
                        {
                            text.Add(Text.Tab);
                            text.Add($"{input.Key}={input.Value}");
                            text.Add(Text.NewLine);
                        }
                    }

                    return text.ToArray();
                }
            );

            _stages.Before();

            var invoker = new WorkflowInvoker(activity);
            ConfigureExtensions(invoker);
            
            var results = invoker.Invoke(inputs, timeout);
            
            _stages.After(results);
            return results;
        }

        private void ConfigureExtensions([NotNull] WorkflowInvoker invoker)
        {
            if (invoker == null) throw new ArgumentNullException(nameof(invoker));

            // Configure extensions by bindings from the container
            var extensionKeys = (
                from keyGroup in _container
                from key in keyGroup
                where key.Tag == null || key.Tag == Key.AnyTag
                where key.Type.GetCustomAttribute<PublicAttribute>() != null
                select key)
                .ToArray();

            _log.Trace(() =>
                {
                    var text = new List<Text>();
                    
                    if (extensionKeys.Length > 0)
                    {
                        text.Add("Extensions: ");
                        text.Add(Text.NewLine);
                        foreach (var key in extensionKeys)
                        {
                            text.Add(Text.Tab);
                            text.Add(key.ToString());
                            text.Add(Text.NewLine);
                        }
                    }

                    return text.ToArray();
                }
            );

            var containerConst = Expression.Constant(_container);
            foreach (var key in extensionKeys)
            {
                var typeConst = Expression.Constant(key.Type);
                var factory = Expression.Lambda(Expression.Call(null, ResolveMethodInfo.MakeGenericMethod(key.Type), containerConst, typeConst)).Compile();
                var addMethod = AddExtensionMethodInfo.MakeGenericMethod(key.Type);
                addMethod.Invoke(invoker.Extensions, new object[] { factory });
            }
        }
    }
}
