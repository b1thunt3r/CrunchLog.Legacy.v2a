using Bit0.CrunchLog.Cli.Commands;
using Bit0.CrunchLog.Cli.Logging;
using Serilog.Events;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bit0.CrunchLog.Cli.Extensions
{
    public static class CommandAppExtensions
    {
        public static CommandApp<TCommand> Setup<TCommand>(this CommandApp<TCommand> app) where TCommand : class, ICommand
        {
            app.Configure(config =>
            {
                config.SetApplicationName("crunch");
                config.PropagateExceptions();
                config.SetInterceptor(new LogInterceptor());
            });

            return app;
        }

        public static CommandApp<TCommand> AddCommands<TCommand>(this CommandApp<TCommand> app) where TCommand : class, ICommand
        {
            app.Configure(config =>
            {
                config.AddCommand<VersionCommand>("version")
                    .WithAlias("ver")
                    .IsHidden();

                config.AddBranch("config", configBranch =>
                {
                    configBranch.AddCommand<ConfigCheckCommand>("check");
                });
            });

            return app;
        }

        public static Int32 HandleException<TCommand>(this CommandApp<TCommand> app, Exception ex) where TCommand : class, ICommand
        {
            AnsiConsole.MarkupLine($":cross_mark: {ex.Message}\r\n");

            if (LogInterceptor.LogLevel.MinimumLevel < LogEventLevel.Error)
            {
                AnsiConsole.WriteException(ex, app.GetExceptionSettings());
            }

            return (Int32)ExitCodes.Error;
        }

        public static ExceptionSettings GetExceptionSettings<TCommand>(this CommandApp<TCommand> app) where TCommand : class, ICommand
        {
            return new ExceptionSettings
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
            };
        }
    }
}
