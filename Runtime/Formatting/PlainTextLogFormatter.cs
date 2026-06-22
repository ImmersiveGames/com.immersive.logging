using System;
using System.Globalization;
using System.Text;
using Immersive.Logging.Records;

namespace Immersive.Logging.Formatting
{
    public sealed class PlainTextLogFormatter : ILogFormatter
    {
        public string Format(LogRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            var builder = new StringBuilder(256);

            builder.Append(record.TimestampUtc.ToString("O", CultureInfo.InvariantCulture));
            builder.Append(" level=");
            builder.Append(record.Level);

            AppendPair(builder, "category", record.Category);
            AppendPair(builder, "context", record.Context);
            AppendPair(builder, "owner", record.OwnerFullName ?? record.OwnerName);
            AppendPair(builder, "message", record.Message);

            if (record.Fields != null)
            {
                for (int i = 0; i < record.Fields.Count; i++)
                {
                    LogField field = record.Fields[i];
                    AppendPair(builder, field.Key, field.Value);
                }
            }

            if (record.Exception != null)
            {
                AppendPair(builder, "exception", record.Exception.GetType().FullName ?? record.Exception.GetType().Name);
                AppendPair(builder, "exceptionMessage", record.Exception.Message);
            }

            return builder.ToString();
        }

        private static void AppendPair(StringBuilder builder, string key, object value)
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
