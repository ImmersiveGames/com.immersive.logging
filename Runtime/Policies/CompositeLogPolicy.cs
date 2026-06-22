using System;
using Immersive.Logging.Records;

namespace Immersive.Logging.Policies
{
    public sealed class CompositeLogPolicy : ILogPolicy
    {
        private readonly ILogPolicy[] _policies;

        public CompositeLogPolicy(params ILogPolicy[] policies)
        {
            _policies = policies ?? new ILogPolicy[0];
        }

        public bool ShouldWrite(LogRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            for (int i = 0; i < _policies.Length; i++)
            {
                ILogPolicy policy = _policies[i];
                if (policy != null && !policy.ShouldWrite(record))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
