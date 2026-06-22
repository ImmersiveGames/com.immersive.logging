using System;
using System.Collections.Generic;
using System.Text;
using Immersive.Logging.Records;
using Immersive.Logging.Policies;
using UnityEngine;

namespace Immersive.Logging.Unity
{
    public sealed class UnityFrameDedupeLogPolicy : ILogPolicy
    {
        private readonly bool _deduplicateSameFrame;
        private readonly bool _warnRepeatedSameFrame;
        private readonly HashSet<string> _seenThisFrame = new HashSet<string>(StringComparer.Ordinal);
        private readonly HashSet<string> _warnedThisFrame = new HashSet<string>(StringComparer.Ordinal);
        private int _lastFrame = -1;

        public UnityFrameDedupeLogPolicy(bool deduplicateSameFrame = true, bool warnRepeatedSameFrame = false)
        {
            _deduplicateSameFrame = deduplicateSameFrame;
            _warnRepeatedSameFrame = warnRepeatedSameFrame;
        }

        public bool ShouldWrite(LogRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            int frame = Time.frameCount;
            if (frame != _lastFrame)
            {
                _seenThisFrame.Clear();
                _warnedThisFrame.Clear();
                _lastFrame = frame;
            }

            string signature = BuildSignature(record);
            if (_seenThisFrame.Add(signature))
            {
                return true;
            }

            if (_warnRepeatedSameFrame && _warnedThisFrame.Add(signature))
            {
                Debug.LogWarning(BuildRepeatedWarning(record, frame));
            }

            return !_deduplicateSameFrame;
        }

        private static string BuildSignature(LogRecord record)
        {
            var builder = new StringBuilder(192);
            builder.Append(record.Level)
                .Append('|').Append(record.Category)
                .Append('|').Append(record.Context)
                .Append('|').Append(record.OwnerFullName ?? record.OwnerName)
                .Append('|').Append(record.Message);

            if (record.Fields != null)
            {
                for (int i = 0; i < record.Fields.Count; i++)
                {
                    LogField field = record.Fields[i];
                    builder.Append('|').Append(field.Key).Append('=').Append(ToInvariantString(field.Value));
                }
            }

            return builder.ToString();
        }

        private static string ToInvariantString(object value)
        {
            if (value == null)
            {
                return "null";
            }

            var formattable = value as IFormattable;
            if (formattable != null)
            {
                return formattable.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
            }

            return value.ToString() ?? string.Empty;
        }

        private static string BuildRepeatedWarning(LogRecord record, int frame)
        {
            return "[WARNING][Immersive.Logging][UnityFrameDedupeLogPolicy] Repeated log in same frame. " +
                "frame='" + frame + "' owner='" + (record.OwnerFullName ?? record.OwnerName ?? "null") + "' message='" + record.Message + "'.";
        }
    }
}
