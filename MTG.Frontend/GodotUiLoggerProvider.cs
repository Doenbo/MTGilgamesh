using Godot;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MTG.Frontend;

public class GodotUiLoggerProvider : ILoggerProvider
{
    private readonly RichTextLabel _gameLog;
    public GodotUiLoggerProvider(RichTextLabel gameLog) => _gameLog = gameLog;
    public ILogger CreateLogger(string categoryName) => new GodotUiLogger(_gameLog, categoryName);
    public void Dispose() { }

    private class GodotUiLogger : ILogger
    {
        private readonly RichTextLabel _gameLog;
        private readonly string _category;

        public GodotUiLogger(RichTextLabel gameLog, string category)
        {
            _gameLog = gameLog;
            _category = Path.GetFileNameWithoutExtension(category);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _gameLog.AppendText($"[[color=gray]{_category}[/color]] {formatter(state, exception)}\n");
        }

        public bool IsEnabled(LogLevel logLevel) => true;
        public IDisposable BeginScope<TState>(TState state) => null;
    }
}
