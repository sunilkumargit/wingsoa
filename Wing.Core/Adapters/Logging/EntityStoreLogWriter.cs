using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Logging;
using Wing.Worker;

namespace Wing.Adapters.Logging
{
    class EntityStoreLogWriter : ILogger
    {
        private LogWriterBufferService _buffer;

        public EntityStoreLogWriter(String name)
        {
            Assert.EmptyString(name, "name");
            this.Name = name;
            _buffer = ServiceLocator.GetInstance<LogWriterBufferService>();

        }

        public string Name { get; private set; }
        public void Log(string message, Category category, Priority priority)
        {
            _buffer.AddEntry(Name, category, priority, message);
        }

        public void LogException(string message, Exception exception, Priority priority)
        {
            _buffer.AddEntry(Name, Category.Exception, priority, 
                String.Format("{0} [Exception: {1}]", message, exception == null ? "" : exception.ToString()));
        }
    }
}
