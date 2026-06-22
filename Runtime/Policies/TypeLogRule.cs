using System;
using Immersive.Logging.Levels;
using Immersive.Logging.Records;

namespace Immersive.Logging.Policies
{
    public sealed class TypeLogRule
    {
        public TypeLogRule(string ruleId, string ownerTypeName, LogLevel minimumLevel)
        {
            if (string.IsNullOrWhiteSpace(ownerTypeName))
            {
                throw new ArgumentException("Owner type name cannot be null, empty, or whitespace.", nameof(ownerTypeName));
            }

            RuleId = string.IsNullOrWhiteSpace(ruleId) ? ownerTypeName.Trim() : ruleId.Trim();
            OwnerTypeName = ownerTypeName.Trim();
            MinimumLevel = minimumLevel;
        }

        public string RuleId { get; }

        public string OwnerTypeName { get; }

        public LogLevel MinimumLevel { get; }

        public bool Matches(LogRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            return string.Equals(record.OwnerFullName, OwnerTypeName, StringComparison.Ordinal) ||
                string.Equals(record.OwnerName, OwnerTypeName, StringComparison.Ordinal);
        }
    }
}
