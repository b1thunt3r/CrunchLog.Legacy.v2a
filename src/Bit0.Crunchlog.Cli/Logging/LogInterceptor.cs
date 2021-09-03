using Bit0.CrunchLog.Cli.Commands.Settings;
using Serilog.Core;
using Serilog.Events;
using Spectre.Console.Cli;

namespace Bit0.CrunchLog.Cli.Logging
{
    internal class LogInterceptor : ICommandInterceptor
    {
        public static readonly LoggingLevelSwitch LogLevel = new(LogEventLevel.Error);

        public void Intercept(CommandContext context, CommandSettings settings)
        {
            if (settings is CommandSettingsBase logSettings)
            {
                LoggingEnricher.Path = logSettings.LogFile;
                LogLevel.MinimumLevel = logSettings.Verbosity;
            }
        }
    }
}
