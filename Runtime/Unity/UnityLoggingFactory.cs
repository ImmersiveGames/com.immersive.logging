using Immersive.Logging.Formatting;
using Immersive.Logging.Levels;
using Immersive.Logging.Loggers;
using Immersive.Logging.Policies;

namespace Immersive.Logging.Unity
{
    public static class UnityLoggingFactory
    {
        public static Logger CreateLogger(LoggingConfigAsset config = null)
        {
            ILogFormatter formatter = config != null
                ? config.CreateFormatter()
                : new UnityConsoleLogFormatter();

            ILogPolicy policy = config != null
                ? config.CreatePolicy()
                : new ConfigurableLogPolicy(true, LogLevel.Info);

            bool suppressStandardLogStackTrace = config == null || config.SuppressStandardLogStackTrace;
            bool suppressWarningStackTrace = config != null && config.SuppressWarningStackTrace;

            return new Logger(
                new UnityConsoleLogSink(
                    formatter,
                    suppressStandardLogStackTrace,
                    suppressWarningStackTrace),
                policy);
        }
    }
}
