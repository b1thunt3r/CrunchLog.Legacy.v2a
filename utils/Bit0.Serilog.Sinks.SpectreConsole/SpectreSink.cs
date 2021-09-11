using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Spectre.Console;
using System;

namespace Bit0.Serilog.Sinks.SpectreConsole
{
    public class SpectreSink : ILogEventSink
    {
        private readonly Func<DateTimeOffset, LogEventLevel, string, Exception, string> _outputFormat;
        private readonly IFormatProvider _formatProvider;

        public SpectreSink(Func<DateTimeOffset, LogEventLevel, string, Exception, string> outputFormat, IFormatProvider formatProvider)
        {
            _outputFormat = outputFormat ?? ((timestamp, level, message, _) =>
                 {
                     return $"[grey][[[/][grey62]{timestamp:s}[/] {level.FormatLevel()}[grey]]][/] {message}";
                 });

            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);
            AnsiConsole.MarkupLine(_outputFormat(logEvent.Timestamp, logEvent.Level, message, logEvent.Exception));
        }
    }

    public static class SpectreSinkSinkExtensions
    {
        public static LoggerConfiguration SpectreConsole(
                  this LoggerSinkConfiguration loggerConfiguration,
                  Func<DateTimeOffset, LogEventLevel, string, Exception, string> outputFormat = null,
                  IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new SpectreSink(outputFormat, formatProvider));
        }

        public static string FormatLevel(this LogEventLevel level, int width = 4)
        {
            string[][] textMap =
            {
                new[] { "v", "vb", "vrb", "verb" },
                new[] { "d", "de", "dbg", "dbug" },
                new[] { "i", "in", "inf", "info" },
                new[] { "w", "wn", "wrn", "warn" },
                new[] { "e", "er", "err", "eror" },
                new[] { "f", "fa", "ftl", "fatl" },
            };

            string[] colorMap =
            {
                "[dim]{0}[/]",
                "[green]{0}[/]",
                "[deepskyblue1]{0}[/]",
                "[yellow]{0}[/]",
                "[red]{0}[/]",
                "[maroon]{0}[/]",
            };

            var index = (int)level;
            return string.Format(colorMap[index], textMap[index][width - 1]);
        }
    }
}
