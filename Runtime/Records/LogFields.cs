namespace Immersive.Logging.Records
{
    public static class LogFields
    {
        public static readonly LogField[] Empty = new LogField[0];

        public static LogField Field(string key, object value)
        {
            return new LogField(key, value);
        }

        public static LogField[] Of(string key, object value)
        {
            return new[] { new LogField(key, value) };
        }

        public static LogField[] Of(string key1, object value1, string key2, object value2)
        {
            return new[]
            {
                new LogField(key1, value1),
                new LogField(key2, value2)
            };
        }

        public static LogField[] Of(string key1, object value1, string key2, object value2, string key3, object value3)
        {
            return new[]
            {
                new LogField(key1, value1),
                new LogField(key2, value2),
                new LogField(key3, value3)
            };
        }

        public static LogField[] Of(
            string key1,
            object value1,
            string key2,
            object value2,
            string key3,
            object value3,
            string key4,
            object value4)
        {
            return new[]
            {
                new LogField(key1, value1),
                new LogField(key2, value2),
                new LogField(key3, value3),
                new LogField(key4, value4)
            };
        }

        public static LogField[] Of(params LogField[] fields)
        {
            return fields ?? Empty;
        }
    }
}
