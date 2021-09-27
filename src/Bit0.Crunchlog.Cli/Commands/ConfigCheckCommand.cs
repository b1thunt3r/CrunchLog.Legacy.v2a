using Bit0.CrunchLog.Cli.Commands.Settings;
using Bit0.CrunchLog.Sdk.Services.ConfigService;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace Bit0.CrunchLog.Cli.Commands
{
    public class ConfigCheckCommand : Command<AppSettings>
    {
        private readonly ILogger<ConfigCheckCommand> _logger;
        private readonly ConfigService _configService;

        public ConfigCheckCommand(ILogger<ConfigCheckCommand> logger, ConfigService configService)
        {
            _logger = logger;
            _configService = configService;
        }

        public override System.Int32 Execute([NotNull] CommandContext context, [NotNull] AppSettings settings)
        {
            AnsiConsole.MarkupLine(settings.ConfigFile.FullName);

            _configService.ParseFile(settings.ConfigFile);

            AnsiConsole.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Default)
            .Start("[yellow]Initializing warp drive[/]", ctx =>
            {
                // Initialize
                Thread.Sleep(3000);
                WriteLogMessage("Starting gravimetric field displacement manifold");
                Thread.Sleep(1000);
                WriteLogMessage("Warming up deuterium chamber");
                Thread.Sleep(2000);
                WriteLogMessage("Generating antideuterium");

                // Warp nacelles
                Thread.Sleep(3000);
                ctx.Spinner(Spinner.Known.BouncingBar);
                ctx.Status("[bold blue]Unfolding warp nacelles[/]");
                WriteLogMessage("Unfolding left warp nacelle");
                Thread.Sleep(2000);
                WriteLogMessage("Left warp nacelle [green]online[/]");
                WriteLogMessage("Unfolding right warp nacelle");
                Thread.Sleep(1000);
                WriteLogMessage("Right warp nacelle [green]online[/]");

                // Warp bubble
                Thread.Sleep(3000);
                ctx.Spinner(Spinner.Known.Star2);
                ctx.Status("[bold blue]Generating warp bubble[/]");
                Thread.Sleep(3000);
                ctx.Spinner(Spinner.Known.Star);
                ctx.Status("[bold blue]Stabilizing warp bubble[/]");

                // Safety
                ctx.Spinner(Spinner.Known.Monkey);
                ctx.Status("[bold blue]Performing safety checks[/]");
                WriteLogMessage("Enabling interior dampening");
                Thread.Sleep(2000);
                WriteLogMessage("Interior dampening [green]enabled[/]");

                // Warp!
                Thread.Sleep(3000);
                ctx.Spinner(Spinner.Known.Moon);
                WriteLogMessage("Preparing for warp");
                Thread.Sleep(1000);
                for (var warp = 1; warp < 10; warp++)
                {
                    ctx.Status($"[bold blue]Warp {warp}[/]");
                    Thread.Sleep(500);
                }
            });

            // Done
            AnsiConsole.MarkupLine("[bold green]Crusing at Warp 9.8[/]");

            return (System.Int32)ExitCodes.Error;
        }

        private void WriteLogMessage(System.String message)
        {
            _logger.LogDebug(message);
        }
    }
}
