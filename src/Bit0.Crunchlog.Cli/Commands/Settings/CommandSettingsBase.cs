using Serilog.Events;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Globalization;

namespace Bit0.CrunchLog.Cli.Commands.Settings
{
    public abstract class CommandSettingsBase : CommandSettings
    {
        [CommandOption("--logFile")]
        [Description("Path and file name for logging")]
        public String LogFile { get; set; }

        [CommandOption("--verbosity | -v")]
        [Description("Minimum level for logging\r\nAllowed values are v[dim]erbose[/], d[dim]ebug[/], w[dim]arning[/], e[dim]rror[/], f[dim]atal[/] or n[dim]one[/].")]
        [TypeConverter(typeof(VerbosityConverter))]
        [DefaultValue(LogEventLevel.Error)]
        public LogEventLevel Verbosity { get; init; }
    }

    public sealed class EmptySettings : CommandSettingsBase
    { }

    internal sealed class VerbosityConverter : TypeConverter
    {
        private readonly Dictionary<String, LogEventLevel> _lookup;

        public VerbosityConverter()
        {
            _lookup = new Dictionary<String, LogEventLevel>(StringComparer.OrdinalIgnoreCase)
            {
                {"0", LogEventLevel.Verbose},
                {"t", LogEventLevel.Verbose},
                {"trace", LogEventLevel.Verbose},
                {"v", LogEventLevel.Verbose},
                {"verbose", LogEventLevel.Verbose},
                {"full", LogEventLevel.Verbose},

                {"1", LogEventLevel.Debug},
                {"d", LogEventLevel.Debug},
                {"debug", LogEventLevel.Debug},

                {"2", LogEventLevel.Information},
                {"i", LogEventLevel.Information},
                {"info", LogEventLevel.Information},
                {"information", LogEventLevel.Information},

                {"3", LogEventLevel.Warning},
                {"w", LogEventLevel.Warning},
                {"warn", LogEventLevel.Warning},
                {"warning", LogEventLevel.Warning},

                {"4", LogEventLevel.Error},
                {"e", LogEventLevel.Error},
                {"error", LogEventLevel.Error},

                {"5", LogEventLevel.Fatal},
                {"c", LogEventLevel.Fatal},
                {"critical", LogEventLevel.Fatal},
                {"f", LogEventLevel.Fatal},
                {"fatal", LogEventLevel.Fatal},

                {"6", (LogEventLevel) 6},
                {"n", (LogEventLevel) 6},
                {"none", (LogEventLevel) 6},
                {"s", (LogEventLevel) 6},
                {"silent", (LogEventLevel) 6},
                {"q", (LogEventLevel) 6},
                {"quite", (LogEventLevel) 6},
            };
        }

        public override Object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value)
        {
            if (value is String stringValue)
            {
                var result = _lookup.TryGetValue(stringValue, out var verbosity);
                if (!result)
                {
                    throw new InvalidOperationException($"The value '{value}' is not a valid verbosity.");
                }
                return verbosity;
            }
            throw new NotSupportedException("Can't convert value to verbosity.");
        }
    }
}
