using System;
using System.Collections.Generic;
using Immersive.Logging.Levels;

namespace Immersive.Logging.Records
{
    public sealed class LogRecord
    {
        private static readonly LogField[] EmptyFields = new LogField[0];

        private readonly LogLevel _level;
        private readonly string _message;
        private readonly string _category;
        private readonly string _context;
        private readonly Type _ownerType;
        private readonly Exception _exception;
        private readonly DateTime _timestampUtc;
        private readonly LogField[] _fields;

        public LogRecord(
            LogLevel level,
            string message,
            string category = null,
            string context = null,
            Exception exception = null,
            Type ownerType = null,
            IEnumerable<LogField> fields = null,
            DateTime? timestampUtc = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Message cannot be null, empty, or whitespace.", nameof(message));
            }

            _level = level;
            _message = message.Trim();
            _category = Normalize(category);
            _context = Normalize(context);
            _ownerType = ownerType;
            _exception = exception;
            _timestampUtc = timestampUtc ?? DateTime.UtcNow;
            _fields = CopyFields(fields);
        }

        public LogLevel Level => _level;

        public string Message => _message;

        public string Category => _category;

        public string Context => _context;

        public Type OwnerType => _ownerType;

        public string OwnerName => _ownerType?.Name;

        public string OwnerFullName => _ownerType?.FullName;

        public string OwnerNamespace => _ownerType?.Namespace;

        public Exception Exception => _exception;

        public DateTime TimestampUtc => _timestampUtc;

        public IReadOnlyList<LogField> Fields => _fields;

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static LogField[] CopyFields(IEnumerable<LogField> fields)
        {
            if (fields == null)
            {
                return EmptyFields;
            }

            var list = new List<LogField>();
            foreach (LogField field in fields)
            {
                list.Add(field);
            }

            return list.Count == 0 ? EmptyFields : list.ToArray();
        }
    }
}
