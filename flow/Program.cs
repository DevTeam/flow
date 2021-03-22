#if DEBUG
namespace Flow
{
    public static class Program
    {
        public static int Main(string[] args) => Flows.Run(args);
    }
}
#endif