using Bit0.CrunchLog.Cli.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
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

            const String outputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:w4}] {Message:lj} {NewLine}";
            const String outputTemplateFile = outputTemplate + "{Exception}";

            serviceCollection.AddLogging(configure =>
                    configure.AddSerilog(new LoggerConfiguration()
                        .MinimumLevel.ControlledBy(LogInterceptor.LogLevel)
                        .WriteTo.Console(outputTemplate: outputTemplate, theme: AnsiConsoleTheme.Code)
                        .Enrich.With<LoggingEnricher>()
                        .WriteTo.Map(LoggingEnricher.LogFilePathPropertyName,
                            (logFilePath, wt) =>
                            {
                                if (!String.IsNullOrWhiteSpace(logFilePath))
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
