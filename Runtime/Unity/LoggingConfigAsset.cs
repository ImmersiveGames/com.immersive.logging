using System;
using System.Collections.Generic;
using Immersive.Logging.Levels;
using Immersive.Logging.Policies;
using UnityEngine;

namespace Immersive.Logging.Unity
{
    [CreateAssetMenu(
        fileName = "LoggingConfig",
        menuName = "Immersive/Logging/Logging Config",
        order = 30)]
    public sealed class LoggingConfigAsset : ScriptableObject
    {
        [Serializable]
        public sealed class NamespaceRule
        {
            public string ruleId = string.Empty;
            public bool enabled = true;
            public string namespacePrefix = string.Empty;
            public LogLevel minimumLevel = LogLevel.Info;
        }

        [Serializable]
        public sealed class TypeRule
        {
            public string ruleId = string.Empty;
            public bool enabled = true;
            public string ownerTypeName = string.Empty;
            public LogLevel minimumLevel = LogLevel.Info;
        }

        [Header("Global")]
        [SerializeField] private bool globalEnabled = true;
        [SerializeField] private LogLevel defaultMinimumLevel = LogLevel.Info;
        [SerializeField] private bool honorLogLevelAttributes = true;

        [Header("Console Format")]
        [SerializeField] private bool includeTimestamp = false;
        [SerializeField] private bool useRichText = false;
        [SerializeField] private bool includeExceptionDetails = true;

        [Header("Frame Hygiene")]
        [SerializeField] private bool deduplicateSameFrame = false;
        [SerializeField] private bool warnRepeatedSameFrame = false;

        [Header("Rules")]
        [SerializeField] private List<NamespaceRule> namespaceRules = new List<NamespaceRule>();
        [SerializeField] private List<TypeRule> typeRules = new List<TypeRule>();

        public bool GlobalEnabled => globalEnabled;

        public LogLevel DefaultMinimumLevel => defaultMinimumLevel;

        public bool HonorLogLevelAttributes => honorLogLevelAttributes;

        public bool IncludeTimestamp => includeTimestamp;

        public bool UseRichText => useRichText;

        public bool IncludeExceptionDetails => includeExceptionDetails;

        public bool DeduplicateSameFrame => deduplicateSameFrame;

        public bool WarnRepeatedSameFrame => warnRepeatedSameFrame;

        public IReadOnlyList<NamespaceRule> NamespaceRules => namespaceRules;

        public IReadOnlyList<TypeRule> TypeRules => typeRules;

        public ILogPolicy CreatePolicy()
        {
            var basePolicy = new ConfigurableLogPolicy(
                globalEnabled,
                defaultMinimumLevel,
                BuildNamespaceRules(namespaceRules),
                BuildTypeRules(typeRules),
                honorLogLevelAttributes);

            if (!deduplicateSameFrame && !warnRepeatedSameFrame)
            {
                return basePolicy;
            }

            return new CompositeLogPolicy(
                basePolicy,
                new UnityFrameDedupeLogPolicy(deduplicateSameFrame, warnRepeatedSameFrame));
        }

        public UnityConsoleLogFormatter CreateFormatter()
        {
            return new UnityConsoleLogFormatter(includeTimestamp, useRichText, includeExceptionDetails);
        }

        internal static IReadOnlyList<NamespaceLogRule> BuildNamespaceRules(IReadOnlyList<NamespaceRule> rules)
        {
            var result = new List<NamespaceLogRule>();
            if (rules == null)
            {
                return result;
            }

            for (int i = 0; i < rules.Count; i++)
            {
                NamespaceRule rule = rules[i];
                if (rule == null || !rule.enabled || string.IsNullOrWhiteSpace(rule.namespacePrefix))
                {
                    continue;
                }

                string ruleId = string.IsNullOrWhiteSpace(rule.ruleId) ? "namespace_rule_" + i : rule.ruleId.Trim();
                result.Add(new NamespaceLogRule(ruleId, rule.namespacePrefix, rule.minimumLevel));
            }

            return result;
        }

        internal static IReadOnlyList<TypeLogRule> BuildTypeRules(IReadOnlyList<TypeRule> rules)
        {
            var result = new List<TypeLogRule>();
            if (rules == null)
            {
                return result;
            }

            for (int i = 0; i < rules.Count; i++)
            {
                TypeRule rule = rules[i];
                if (rule == null || !rule.enabled || string.IsNullOrWhiteSpace(rule.ownerTypeName))
                {
                    continue;
                }

                string ruleId = string.IsNullOrWhiteSpace(rule.ruleId) ? "type_rule_" + i : rule.ruleId.Trim();
                result.Add(new TypeLogRule(ruleId, rule.ownerTypeName, rule.minimumLevel));
            }

            return result;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            TrimRules();
            WarnInvalidRules();
        }

        private void TrimRules()
        {
            if (namespaceRules != null)
            {
                for (int i = 0; i < namespaceRules.Count; i++)
                {
                    NamespaceRule rule = namespaceRules[i];
                    if (rule == null)
                    {
                        continue;
                    }

                    rule.ruleId = (rule.ruleId ?? string.Empty).Trim();
                    rule.namespacePrefix = (rule.namespacePrefix ?? string.Empty).Trim();
                }
            }

            if (typeRules != null)
            {
                for (int i = 0; i < typeRules.Count; i++)
                {
                    TypeRule rule = typeRules[i];
                    if (rule == null)
                    {
                        continue;
                    }

                    rule.ruleId = (rule.ruleId ?? string.Empty).Trim();
                    rule.ownerTypeName = (rule.ownerTypeName ?? string.Empty).Trim();
                }
            }
        }

        private void WarnInvalidRules()
        {
            WarnDuplicateNamespaceRules();
            WarnDuplicateTypeRules();
        }

        private void WarnDuplicateNamespaceRules()
        {
            if (namespaceRules == null)
            {
                return;
            }

            var ids = new HashSet<string>(StringComparer.Ordinal);
            var prefixes = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < namespaceRules.Count; i++)
            {
                NamespaceRule rule = namespaceRules[i];
                if (rule == null)
                {
                    Debug.LogWarning($"[WARNING][Immersive.Logging][LoggingConfigAsset] Namespace rule at index '{i}' is null in asset '{name}'.", this);
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(rule.ruleId) && !ids.Add(rule.ruleId))
                {
                    Debug.LogWarning($"[WARNING][Immersive.Logging][LoggingConfigAsset] Duplicate namespace rule id '{rule.ruleId}' in asset '{name}'.", this);
                }

                if (!string.IsNullOrWhiteSpace(rule.namespacePrefix) && !prefixes.Add(rule.namespacePrefix))
                {
                    Debug.LogWarning($"[WARNING][Immersive.Logging][LoggingConfigAsset] Duplicate namespace prefix '{rule.namespacePrefix}' in asset '{name}'.", this);
                }
            }
        }

        private void WarnDuplicateTypeRules()
        {
            if (typeRules == null)
            {
                return;
            }

            var ids = new HashSet<string>(StringComparer.Ordinal);
            var owners = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < typeRules.Count; i++)
            {
                TypeRule rule = typeRules[i];
                if (rule == null)
                {
                    Debug.LogWarning($"[WARNING][Immersive.Logging][LoggingConfigAsset] Type rule at index '{i}' is null in asset '{name}'.", this);
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(rule.ruleId) && !ids.Add(rule.ruleId))
                {
                    Debug.LogWarning($"[WARNING][Immersive.Logging][LoggingConfigAsset] Duplicate type rule id '{rule.ruleId}' in asset '{name}'.", this);
                }

                if (!string.IsNullOrWhiteSpace(rule.ownerTypeName) && !owners.Add(rule.ownerTypeName))
                {
                    Debug.LogWarning($"[WARNING][Immersive.Logging][LoggingConfigAsset] Duplicate owner type rule '{rule.ownerTypeName}' in asset '{name}'.", this);
                }
            }
        }
#endif
    }
}
