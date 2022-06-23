﻿using Bit0.CrunchLog.Cli.Commands;
using Bit0.CrunchLog.Cli.Extensions;
using Bit0.CrunchLog.Sdk.Services.ConfigService;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bit0.CrunchLog.Cli
{
    public class Program
    {
        private static Int32 Main(String[] args)
        {
            var registrar = new ServiceCollection()
                .AddSerilog()
                .AddTransient<ConfigService>()
                .Build();

            var app = new CommandApp<VersionCommand>(registrar)
                .Setup()
                .AddCommands();

            try
            {
                AnsiConsole.WriteLine();
                return app.Run(args);
            }
            catch (Exception ex)
            {
                return app.HandleException(ex);
            }
        }
    }

    public enum ExitCodes
    {
        Success,
        Error
    }
}
