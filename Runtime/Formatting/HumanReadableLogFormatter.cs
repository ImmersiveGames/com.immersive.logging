using System;
using System.Globalization;
using System.Text;
using Immersive.Logging.Records;

namespace Immersive.Logging.Formatting
{
    public sealed class HumanReadableLogFormatter : ILogFormatter
    {
        public string Format(LogRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            var builder = new StringBuilder();

            AppendIdentifier(builder, record.Level.ToString());
            AppendIdentifier(builder, record.Category);
            AppendIdentifier(builder, record.Context);

            builder.AppendLine();
            builder.Append("  message: ");
            builder.AppendLine(record.Message);

            if (record.Exception != null)
            {
                builder.Append("  exception: ");
                builder.AppendLine(record.Exception.ToString());
            }

            builder.Append("timestamp: ");
            builder.Append(record.TimestampUtc.ToString("O", CultureInfo.InvariantCulture));

            return builder.ToString().TrimEnd();
        }

        private static void AppendIdentifier(StringBuilder builder, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            builder.Append('[');
            builder.Append(value);
            builder.Append("] ");
        }
    }
}
