using Bit0.CrunchLog.Cli.Logging;
using Bit0.Serilog.Sinks.SpectreConsole;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Spectre.Cli.Extensions.DependencyInjection;

namespace Bit0.CrunchLog.Cli.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static DependencyInjectionRegistrar Build(this IServiceCollection serviceCollection)
        {
            return new DependencyInjectionRegistrar(serviceCollection);
        }

        public static IServiceCollection AddSerilog(this IServiceCollection serviceCollection)
        {
            const string outputTemplateFile = "[{Timestamp:s} {Level:w4}] {Message:lj} {NewLine}{Exception}";

            serviceCollection.AddLogging(configure =>
                    configure.AddSerilog(new LoggerConfiguration()
                        .MinimumLevel.ControlledBy(LogInterceptor.LogLevel)
                        .WriteTo.SpectreConsole()
                        .Enrich.With<LoggingEnricher>()
                        .WriteTo.Map(LoggingEnricher.LogFilePathPropertyName,
                            (logFilePath, wt) =>
                            {
                                if (!string.IsNullOrWhiteSpace(logFilePath))
                                {
                                    wt.File(path: logFilePath,
                                        outputTemplate: outputTemplateFile
                                    );
                                }
                            }, 1)
                        .CreateLogger()
                    )
                );

            return serviceCollection;
        }
    }
}
