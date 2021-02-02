namespace Flow.Core
{
    using System.Collections.Generic;
    using IoC;
    using Services;
    using static IoC.Lifetime;

    internal class Configuration: IConfiguration
    {
        public IEnumerable<IToken> Apply(IMutableContainer container)
        {
            yield return container
                .Bind<IStdOut>().As(Singleton).To<StdOut>()
                .Bind<IStdErr>().As(Singleton).To<StdErr>();
        }
    }
}
