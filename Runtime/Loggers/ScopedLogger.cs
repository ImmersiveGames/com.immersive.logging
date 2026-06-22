using System;
using Immersive.Logging.Levels;
using Immersive.Logging.Records;

namespace Immersive.Logging.Loggers
{
    public sealed class ScopedLogger
    {
        private readonly Logger _logger;
        private readonly Type _ownerType;
        private readonly string _category;
        private readonly string _context;

        internal ScopedLogger(Logger logger, Type ownerType, string category, string context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ownerType = ownerType ?? throw new ArgumentNullException(nameof(ownerType));
            _category = category;
            _context = context;
        }

        public Type OwnerType => _ownerType;

        public string Category => _category;

        public string Context => _context;

        public void Trace(string message, params LogField[] fields)
        {
            Write(LogLevel.Trace, message, null, fields);
        }

        public void Debug(string message, params LogField[] fields)
        {
            Write(LogLevel.Debug, message, null, fields);
        }

        public void Info(string message, params LogField[] fields)
        {
            Write(LogLevel.Info, message, null, fields);
        }

        public void Warning(string message, params LogField[] fields)
        {
            Write(LogLevel.Warning, message, null, fields);
        }

        public void Error(string message, params LogField[] fields)
        {
            Write(LogLevel.Error, message, null, fields);
        }

        public void Error(string message, Exception exception, params LogField[] fields)
        {
            Write(LogLevel.Error, message, exception, fields);
        }

        public void Fatal(string message, params LogField[] fields)
        {
            Write(LogLevel.Fatal, message, null, fields);
        }

        public void Fatal(string message, Exception exception, params LogField[] fields)
        {
            Write(LogLevel.Fatal, message, exception, fields);
        }

        public void Write(LogLevel level, string message, Exception exception = null, params LogField[] fields)
        {
            _logger.Write(level, message, _category, _context, _ownerType, exception, fields);
        }
    }
}
