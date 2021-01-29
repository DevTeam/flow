#if NET48
using System.Activities;
#else
using System.Activities.XamlIntegration;
using System.Linq;
using System.Reflection;
#endif

namespace flow
{
    public static class Program
    {
        public static void Main(string[] args)
        {
#if NET48
            WorkflowInvoker.Invoke(new Build());
#else
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("Build.xaml")));
            var helloWorldActivity = ActivityXamlServices.Load(stream);
            System.Activities.WorkflowInvoker.Invoke(helloWorldActivity);
#endif
        }
    }
}
