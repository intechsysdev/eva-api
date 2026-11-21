using System.Text;

namespace Migracion.Api.Logging
{
    /// <summary>
    /// Logger de archivo minimalista (sin dependencias externas) con un
    /// archivo por día: Logs/eva-api-yyyyMMdd.log. Pensado para que los
    /// errores del API queden en disco y se pueda hacer análisis de causa
    /// raíz aunque IIS no tenga habilitado el stdout log.
    /// </summary>
    public sealed class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _directory;
        private readonly LogLevel _minLevel;
        private readonly object _lock = new();

        public FileLoggerProvider(string directory, LogLevel minLevel)
        {
            _directory = directory;
            _minLevel = minLevel;
            Directory.CreateDirectory(_directory);
        }

        public ILogger CreateLogger(string categoryName) => new FileLogger(this, categoryName);

        public void Dispose()
        {
        }

        private void Write(string line)
        {
            try
            {
                lock (_lock)
                {
                    var file = Path.Combine(_directory, $"eva-api-{DateTime.Now:yyyyMMdd}.log");
                    File.AppendAllText(file, line + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch
            {
                // El logging nunca debe tumbar una petición
            }
        }

        private sealed class FileLogger : ILogger
        {
            private readonly FileLoggerProvider _provider;
            private readonly string _category;

            public FileLogger(FileLoggerProvider provider, string category)
            {
                _provider = provider;
                _category = category;
            }

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) =>
                logLevel != LogLevel.None && logLevel >= _provider._minLevel;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                Exception? exception, Func<TState, Exception?, string> formatter)
            {
                if (!IsEnabled(logLevel))
                {
                    return;
                }

                var sb = new StringBuilder();
                sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                sb.Append(" [").Append(logLevel).Append("] ");
                sb.Append(_category).Append(" - ");
                sb.Append(formatter(state, exception));
                if (exception != null)
                {
                    sb.AppendLine().Append(exception);
                }

                _provider.Write(sb.ToString());
            }
        }
    }
}
