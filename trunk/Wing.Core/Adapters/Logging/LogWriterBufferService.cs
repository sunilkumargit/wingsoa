using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Logging;

namespace Wing.Adapters.Logging
{
    public class LogWriterBufferService
    {
        private Queue<LogEntryEntity> _buffer = new Queue<LogEntryEntity>();
        private Object _lockObject = new object();

        public void AddEntry(String logName, Category category, Priority priority, String message)
        {
            lock (_lockObject)
            {
                _buffer.Enqueue(new LogEntryEntity()
                {
                    LogName = logName,
                    CategoryIndex = (int)category,
                    PriorityIndex = (int)priority,
                    Message = message
                });
            }
            if (OnEntryAdded != null)
                OnEntryAdded.Invoke(this, new EventArgs());
        }

        internal event EventHandler OnEntryAdded;

        internal LogEntryEntity Pop()
        {
            if (_buffer.Count == 0)
                return null;
            lock (_lockObject)
            {
                if (_buffer.Count == 0)
                    return null;
                return _buffer.Dequeue();
            }
        }
    }
}
