using System;
using Immersive.Logging.Levels;
using Immersive.Logging.Records;

namespace Immersive.Logging.Policies
{
    public sealed class NamespaceLogRule
    {
        public NamespaceLogRule(string ruleId, string namespacePrefix, LogLevel minimumLevel)
        {
            if (string.IsNullOrWhiteSpace(namespacePrefix))
            {
                throw new ArgumentException("Namespace prefix cannot be null, empty, or whitespace.", nameof(namespacePrefix));
            }

            RuleId = string.IsNullOrWhiteSpace(ruleId) ? namespacePrefix.Trim() : ruleId.Trim();
            NamespacePrefix = namespacePrefix.Trim();
            MinimumLevel = minimumLevel;
        }

        public string RuleId { get; }

        public string NamespacePrefix { get; }

        public LogLevel MinimumLevel { get; }

        public bool Matches(LogRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            return StartsWith(record.OwnerNamespace, NamespacePrefix) || StartsWith(record.Category, NamespacePrefix);
        }

        private static bool StartsWith(string value, string prefix)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                value.StartsWith(prefix, StringComparison.Ordinal);
        }
    }
}
