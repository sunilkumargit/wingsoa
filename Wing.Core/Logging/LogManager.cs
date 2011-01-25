using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Logging
{
    public class LogManager : ILogManager
    {
        private Dictionary<String, ILogger> _loggers = new Dictionary<string, ILogger>();
        private Object __syncObject = new object();

        public ILogger GetSystemLogger()
        {
            return GetLogger("SYSTEM");
        }

        public ILogger GetLogger(string name)
        {
            Assert.EmptyString(name, "name");
            return GetLoggerInternal(name.ToLower());
        }

        public ILogger GetLogger(Type forType)
        {
            return GetLoggerInternal(String.Format("TYPE({0})", forType.Name));
        }

        public void RegisterLogger(ILogger logger)
        {
            Assert.NullArgument(logger, "logger");
            _loggers[logger.Name] = logger;
        }

        private ILogger GetLoggerInternal(String name)
        {
            ILogger result = null;
            if (!_loggers.TryGetValue(name, out result))
            {
                lock (__syncObject)
                {
                    if (!_loggers.TryGetValue(name, out result))
                    {
                        result = ServiceLocator.GetInstance<ILoggerFactory>().CreateLogger(name);
                        _loggers[result.Name] = result;
                    }
                }
            }
            return result;
        }
    }
}
