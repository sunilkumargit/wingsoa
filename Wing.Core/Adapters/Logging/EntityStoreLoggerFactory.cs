using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Logging;

namespace Wing.Adapters.Logging
{
    class EntityStoreLoggerFactory : ILoggerFactory
    {
        public ILogger CreateLogger(string name)
        {
            return new EntityStoreLogWriter(name);
        }
    }
}
