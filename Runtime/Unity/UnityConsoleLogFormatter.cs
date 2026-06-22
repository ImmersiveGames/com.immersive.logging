using System;
using System.Globalization;
using System.Text;
using Immersive.Logging.Formatting;
using Immersive.Logging.Levels;
using Immersive.Logging.Records;

namespace Immersive.Logging.Unity
{
    public sealed class UnityConsoleLogFormatter : ILogFormatter
    {
        private readonly bool _includeTimestamp;
        private readonly bool _useRichText;
        private readonly bool _includeExceptionDetails;

        public UnityConsoleLogFormatter(
            bool includeTimestamp = false,
            bool useRichText = false,
            bool includeExceptionDetails = true)
        {
            _includeTimestamp = includeTimestamp;
            _useRichText = useRichText;
            _includeExceptionDetails = includeExceptionDetails;
        }

        public string Format(LogRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            var builder = new StringBuilder(256);
            AppendIdentifier(builder, ToDisplayLevel(record.Level), GetLevelColor(record.Level));
            AppendIdentifier(builder, record.Category, null);
            AppendIdentifier(builder, GetOwnerOrContext(record), null);

            if (builder.Length > 0)
            {
                builder.Append(' ');
            }

            builder.Append(record.Message);
            AppendFields(builder, record);
            AppendExceptionSummary(builder, record.Exception);

            if (_includeTimestamp)
            {
                AppendField(builder, "utc", record.TimestampUtc.ToString("O", CultureInfo.InvariantCulture));
            }

            if (_includeExceptionDetails && record.Exception != null)
            {
                builder.AppendLine();
                builder.Append(record.Exception);
            }

            return builder.ToString().TrimEnd();
        }

        private static string GetOwnerOrContext(LogRecord record)
        {
            if (!string.IsNullOrWhiteSpace(record.OwnerName))
            {
                return record.OwnerName;
            }

            return record.Context;
        }

        private static string ToDisplayLevel(LogLevel level)
        {
            return level.ToString().ToUpperInvariant();
        }

        private string GetLevelColor(LogLevel level)
        {
            if (!_useRichText)
            {
                return null;
            }

            switch (level)
            {
                case LogLevel.Trace:
                    return "#9E9E9E";
                case LogLevel.Debug:
                    return "#A8DEED";
                case LogLevel.Info:
                    return "#FFFFFF";
                case LogLevel.Warning:
                    return "#FFD54F";
                case LogLevel.Error:
                case LogLevel.Fatal:
                    return "#FF6B6B";
                default:
                    return null;
            }
        }

        private void AppendIdentifier(StringBuilder builder, string value, string color)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            string trimmed = value.Trim();
            if (_useRichText && !string.IsNullOrWhiteSpace(color))
            {
                builder.Append("<color=");
                builder.Append(color);
                builder.Append(">");
                builder.Append('[');
                builder.Append(trimmed);
                builder.Append(']');
                builder.Append("</color>");
                return;
            }

            builder.Append('[');
            builder.Append(trimmed);
            builder.Append(']');
        }

        private static void AppendFields(StringBuilder builder, LogRecord record)
        {
            if (record.Fields == null || record.Fields.Count == 0)
            {
                return;
            }

            for (int i = 0; i < record.Fields.Count; i++)
            {
                LogField field = record.Fields[i];
                AppendField(builder, field.Key, field.Value);
            }
        }

        private static void AppendExceptionSummary(StringBuilder builder, Exception exception)
        {
            if (exception == null)
            {
                return;
            }

            string typeName = exception.GetType().FullName ?? exception.GetType().Name;
            AppendField(builder, "exception", typeName + ": " + exception.Message);
        }

        private static void AppendField(StringBuilder builder, string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            builder.Append(' ');
            builder.Append(key.Trim());
            builder.Append("='");
            builder.Append(EscapeValue(value));
            builder.Append('\'');
        }

        private static string EscapeValue(object value)
        {
            if (value == null)
            {
                return "null";
            }

            string text = value is IFormattable formattable
                ? formattable.ToString(null, CultureInfo.InvariantCulture)
                : value.ToString();

            text = text ?? string.Empty;
            return text.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\r", "\\r").Replace("\n", "\\n");
        }
    }
}
