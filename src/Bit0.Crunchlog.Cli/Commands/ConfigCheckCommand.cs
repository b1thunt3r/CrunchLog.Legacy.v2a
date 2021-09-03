using Bit0.CrunchLog.Cli.Commands.Settings;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace Bit0.CrunchLog.Cli.Commands
{
    public class ConfigCheckCommand : Command<AppSettings>
    {
        private readonly ILogger<ConfigCheckCommand> _logger;

        public ConfigCheckCommand(ILogger<ConfigCheckCommand> logger)
        {
            _logger = logger;
        }

        public override Int32 Execute([NotNull] CommandContext context, [NotNull] AppSettings settings)
        {
            AnsiConsole.MarkupLine(settings.ConfigFile.FullName);

            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
