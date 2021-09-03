using Bit0.CrunchLog.Cli.Commands.Settings;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace Bit0.CrunchLog.Cli.Commands
{
    public class ConfigCheckCommand : Command<EmptySettings>
    {
        private readonly ILogger<ConfigCheckCommand> _logger;

        public ConfigCheckCommand(ILogger<ConfigCheckCommand> logger)
        {
            _logger = logger;
        }

        public override Int32 Execute([NotNull] CommandContext context, [NotNull] EmptySettings settings)
        {
            _logger.LogTrace("Trace");
            _logger.LogDebug("Debug");
            _logger.LogInformation("Info");
            _logger.LogWarning("Warning");
            _logger.LogError("Error");
            _logger.LogCritical("Critical");

            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                //var a = new { b = "t", c = DateTime.Now };
                _logger.LogError(new EventId(1000), ex, ex.Message);
                //_logger.LogError(ex, ex.Message + " {@ex} on {Created:s}", ex, DateTime.Now);

                throw;
            }
        }
    }
}
