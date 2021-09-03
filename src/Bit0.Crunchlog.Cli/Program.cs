using Bit0.CrunchLog.Cli.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Cli.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Bit0.CrunchLog.Cli
{
    public class Program
    {
        private static Int32 Main(String[] args)
        {
            var serviceCollection = new ServiceCollection()
                .AddLogging(configure =>
                    configure.AddSimpleConsole(opts =>
                        {
                            opts.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
                        })
                );

            using var registrar = new DependencyInjectionRegistrar(serviceCollection);

            var app = new CommandApp<VersionCommand>(registrar);
            app.Configure(config =>
            {

                config.SetApplicationName("crunch");
#if DEBUG
                config.PropagateExceptions();
#endif

                config.AddCommand<VersionCommand>("version")
                    .WithAlias("ver")
                    .IsHidden();
            });

            try
            {
                return app.Run(args);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
