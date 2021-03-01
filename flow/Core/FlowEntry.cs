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
    internal class FlowEntry
    {
        private readonly IContainer _container;
        [NotNull] private readonly IStages _stages;
        private static readonly MethodInfo ResolveMethodInfo = typeof(FluentResolve).GetMethods().First(i => i.Name == nameof(FluentResolve.Resolve) && i.IsPublic && i.IsStatic && i.ReturnParameter?.ParameterType.IsGenericParameter == true && i.GetParameters().Length == 2 && i.GetParameters()[0].ParameterType == typeof(IContainer) && i.GetParameters()[1].ParameterType == typeof(Type));
        private static readonly MethodInfo AddExtensionMethodInfo = typeof(WorkflowInstanceExtensionManager).GetMethods().First(i => i.Name == nameof(WorkflowInstanceExtensionManager.Add) && i.IsPublic && !i.IsStatic && i.IsGenericMethod && i.GetParameters().Length == 1 && i.GetParameters()[0].ParameterType.IsGenericType && i.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Func<>));

        public FlowEntry(
            [NotNull] IContainer container,
            [NotNull] IStages stages)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _stages = stages;
        }

        public IDictionary<string, object> Run([NotNull] Activity activity, TimeSpan timeout)
        {
            if (activity == null) throw new ArgumentNullException(nameof(activity));
            _stages.Before();

            var invoker = new WorkflowInvoker(activity);
            ConfigureExtensions(invoker);
            
            var results = invoker.Invoke(timeout);
            
            _stages.After(results);
            return results;
        }

        private void ConfigureExtensions([NotNull] WorkflowInvoker invoker)
        {
            if (invoker == null) throw new ArgumentNullException(nameof(invoker));

            // Configure extensions by bindings from the container
            var extensionKeys =
                from keyGroup in _container
                from key in keyGroup
                where key.Tag == null || key.Tag == Key.AnyTag
                where key.Type.GetCustomAttribute<PublicAttribute>() != null
                select key;

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
