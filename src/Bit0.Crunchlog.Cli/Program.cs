using Bit0.CrunchLog.Cli.Commands;
using Bit0.CrunchLog.Cli.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Spectre.Cli.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bit0.CrunchLog.Cli
{
    public class Program
    {
        private static Int32 Main(String[] args)
        {
            const String outputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:w4}] {Message:lj} {NewLine}";
            const String outputTemplateFile = outputTemplate + "{Exception}";

            var serviceCollection = new ServiceCollection()
                .AddLogging(configure =>
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

            using var registrar = new DependencyInjectionRegistrar(serviceCollection);

            var app = new CommandApp<VersionCommand>(registrar);
            app.Configure(config =>
            {
                config.SetApplicationName("crunch");
                config.PropagateExceptions();
                config.SetInterceptor(new LogInterceptor());

                config.AddCommand<VersionCommand>("version")
                    .WithAlias("ver")
                    .IsHidden();

                config.AddBranch("config", configBranch =>
                {
                    configBranch.AddCommand<ConfigCheckCommand>("check");
                });
            });

            try
            {
                return app.Run(args);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($":cross_mark: {ex.Message}\r\n");

                if (LogInterceptor.LogLevel.MinimumLevel < LogEventLevel.Error)
                {
                    AnsiConsole.WriteException(ex, new ExceptionSettings
                    {
                        Format = ExceptionFormats.ShortenMethods | ExceptionFormats.ShortenTypes,
                        Style = new ExceptionStyle
                        {
                            Exception = new Style().Foreground(Color.Yellow),
                            Message = new Style().Foreground(Color.White),
                            Dimmed = new Style().Foreground(Color.Grey),
                            NonEmphasized = new Style().Foreground(Color.Silver),
                            Parenthesis = new Style().Foreground(Color.Grey),
                            Method = new Style().Foreground(Color.Yellow),
                            ParameterName = new Style().Foreground(Color.Yellow),
                            ParameterType = new Style().Foreground(Color.White),
                            Path = new Style().Foreground(Color.Cornsilk1),
                            LineNumber = new Style().Foreground(Color.Cornsilk1)
                        }
                    });
                }

                return -1;
            }
        }
    }
}
