using System;
using Immersive.Logging.Formatting;
using Immersive.Logging.Levels;
using Immersive.Logging.Records;
using Immersive.Logging.Sinks;
using UnityEngine;

namespace Immersive.Logging.Unity
{
    public sealed class UnityConsoleLogSink : ILogSink
    {
        private readonly ILogFormatter _formatter;
        private readonly bool _suppressStandardLogStackTrace;
        private readonly bool _suppressWarningStackTrace;

        public UnityConsoleLogSink(
            ILogFormatter formatter = null,
            bool suppressStandardLogStackTrace = true,
            bool suppressWarningStackTrace = false)
        {
            _formatter = formatter ?? new UnityConsoleLogFormatter();
            _suppressStandardLogStackTrace = suppressStandardLogStackTrace;
            _suppressWarningStackTrace = suppressWarningStackTrace;
        }

        public void Write(LogRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            string message = _formatter.Format(record);

            switch (record.Level)
            {
                case LogLevel.Warning:
                    WriteWarning(message);
                    return;
                case LogLevel.Error:
                case LogLevel.Fatal:
                    Debug.LogError(message);
                    return;
                default:
                    WriteStandardLog(message);
                    return;
            }
        }

        private void WriteStandardLog(string message)
        {
            if (_suppressStandardLogStackTrace)
            {
                Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "{0}", message);
                return;
            }

            Debug.Log(message);
        }

        private void WriteWarning(string message)
        {
            if (_suppressWarningStackTrace)
            {
                Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "{0}", message);
                return;
            }

            Debug.LogWarning(message);
        }
    }
}
