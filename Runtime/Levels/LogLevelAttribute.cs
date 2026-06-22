using System;

namespace Immersive.Logging.Levels
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public sealed class LogLevelAttribute : Attribute
    {
        public LogLevelAttribute(LogLevel minimumLevel)
        {
            MinimumLevel = minimumLevel;
        }

        public LogLevel MinimumLevel { get; }
    }
}
