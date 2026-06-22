using System;
using System.Collections.Generic;
using Immersive.Logging.Levels;
using Immersive.Logging.Records;

namespace Immersive.Logging.Policies
{
    public sealed class ConfigurableLogPolicy : ILogPolicy
    {
        private readonly NamespaceLogRule[] _namespaceRules;
        private readonly TypeLogRule[] _typeRules;

        public ConfigurableLogPolicy(
            bool enabled = true,
            LogLevel defaultMinimumLevel = LogLevel.Info,
            IEnumerable<NamespaceLogRule> namespaceRules = null,
            IEnumerable<TypeLogRule> typeRules = null,
            bool honorLogLevelAttribute = true)
        {
            Enabled = enabled;
            DefaultMinimumLevel = defaultMinimumLevel;
            HonorLogLevelAttribute = honorLogLevelAttribute;
            _namespaceRules = SortNamespaceRules(namespaceRules);
            _typeRules = CopyTypeRules(typeRules);
        }

        public bool Enabled { get; }

        public LogLevel DefaultMinimumLevel { get; }

        public bool HonorLogLevelAttribute { get; }

        public bool ShouldWrite(LogRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (!Enabled)
            {
                return false;
            }

            LogLevel minimumLevel = ResolveMinimumLevel(record);
            return record.Level >= minimumLevel;
        }

        private LogLevel ResolveMinimumLevel(LogRecord record)
        {
            for (int i = 0; i < _typeRules.Length; i++)
            {
                TypeLogRule rule = _typeRules[i];
                if (rule.Matches(record))
                {
                    return rule.MinimumLevel;
                }
            }

            for (int i = 0; i < _namespaceRules.Length; i++)
            {
                NamespaceLogRule rule = _namespaceRules[i];
                if (rule.Matches(record))
                {
                    return rule.MinimumLevel;
                }
            }

            if (HonorLogLevelAttribute && record.OwnerType != null)
            {
                var attribute = Attribute.GetCustomAttribute(record.OwnerType, typeof(LogLevelAttribute)) as LogLevelAttribute;
                if (attribute != null)
                {
                    return attribute.MinimumLevel;
                }
            }

            return DefaultMinimumLevel;
        }

        private static NamespaceLogRule[] SortNamespaceRules(IEnumerable<NamespaceLogRule> rules)
        {
            if (rules == null)
            {
                return new NamespaceLogRule[0];
            }

            var list = new List<NamespaceLogRule>();
            foreach (NamespaceLogRule rule in rules)
            {
                if (rule != null)
                {
                    list.Add(rule);
                }
            }

            list.Sort((a, b) =>
            {
                int prefixLengthCompare = b.NamespacePrefix.Length.CompareTo(a.NamespacePrefix.Length);
                if (prefixLengthCompare != 0)
                {
                    return prefixLengthCompare;
                }

                return string.Compare(a.RuleId, b.RuleId, StringComparison.Ordinal);
            });

            return list.ToArray();
        }

        private static TypeLogRule[] CopyTypeRules(IEnumerable<TypeLogRule> rules)
        {
            if (rules == null)
            {
                return new TypeLogRule[0];
            }

            var list = new List<TypeLogRule>();
            foreach (TypeLogRule rule in rules)
            {
                if (rule != null)
                {
                    list.Add(rule);
                }
            }

            return list.ToArray();
        }
    }
}
