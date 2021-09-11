using Bit0.CrunchLog.Sdk.Config;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Bit0.CrunchLog.Cli.Commands
{
    internal sealed class VersionCommand : Command<EmptyCommandSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] EmptyCommandSettings settings)
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
            grid.AddRow("[wheat1]Core Libraries[/]");
            grid.AddRow($"Bit0.CrunchLog", $"{GetProductVersion(typeof(CrunchLog))} ({GetTargetFramework(typeof(CrunchLog))})");
            grid.AddRow($"Bit0.CrunchLog.Sdk", $"{GetProductVersion(typeof(CrunchConfig))} ({GetTargetFramework(typeof(CrunchConfig))})");
            grid.AddRow($"Bit0.CrunchLog.Cli", $"{GetProductVersion(typeof(Program))} ({GetTargetFramework(typeof(Program))})");
            grid.AddRow("");
            grid.AddRow("[wheat1]Environment[/]");
            grid.AddRow($"Operating System", RuntimeInformation.OSDescription);
            grid.AddRow($".NET", RuntimeInformation.FrameworkDescription.Replace(".NET ", ""));
            grid.AddRow($"RID", RuntimeInformation.RuntimeIdentifier);
            grid.AddRow($"[bold]Working Directory[/]", Environment.CurrentDirectory);
            AnsiConsole.Render(grid);

            return 0;
        }

        private static string GetTargetFramework(Type type)
        {
            return type.Assembly.GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>()?.FrameworkName ?? "Unknown";
        }

        private static string GetProductVersion(Type type)
        {
            return FileVersionInfo.GetVersionInfo(type.Assembly.Location).ProductVersion;
        }
    }
}
