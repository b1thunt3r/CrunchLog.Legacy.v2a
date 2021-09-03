using Bit0.CrunchLog.Sdk.Config;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Bit0.CrunchLog.Cli.Commands
{
    internal sealed class VersionCommand : Command<EmptyCommandSettings>
    {
        public override Int32 Execute([NotNull] CommandContext context, [NotNull] EmptyCommandSettings settings)
        {
            AnsiConsole.Markup("[green]CrunchLog[/]");
#if DEBUG
            AnsiConsole.Markup(" [red]DEBUG BUILD[/]");
#endif
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[dim]Use[/] crunch --help [dim]to see available commands[/]");
            AnsiConsole.WriteLine();

            var grid = new Grid();
            grid.AddColumn(new GridColumn().NoWrap());
            grid.AddColumn(new GridColumn());
            grid.AddRow("[yellow]Core Libraries[/]");
            grid.AddRow($"Bit0.CrunchLog", $"{FileVersionInfo.GetVersionInfo(typeof(CrunchLog).Assembly.Location).ProductVersion}");
            grid.AddRow($"Bit0.CrunchLog.Sdk", $"{FileVersionInfo.GetVersionInfo(typeof(CrunchConfig).Assembly.Location).ProductVersion}");
            grid.AddRow($"Bit0.CrunchLog.Cli", $"{FileVersionInfo.GetVersionInfo(typeof(Program).Assembly.Location).ProductVersion}");
            grid.AddRow("");
            grid.AddRow("[yellow]Environment[/]");
            grid.AddRow($".NET", RuntimeInformation.FrameworkDescription.Replace(".NET ", ""));
            grid.AddRow($"RID", RuntimeInformation.RuntimeIdentifier);
            grid.AddRow($"[bold]Working Directory[/]", Environment.CurrentDirectory);
            AnsiConsole.Render(grid);

            return 0;
        }
    }
}
