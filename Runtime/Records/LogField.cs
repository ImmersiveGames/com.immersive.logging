using System;

namespace Immersive.Logging.Records
{
    public readonly struct LogField
    {
        public LogField(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Field key cannot be null, empty, or whitespace.", nameof(key));
            }

            Key = key.Trim();
            Value = value;
        }

        public string Key { get; }

        public object Value { get; }
    }
}
