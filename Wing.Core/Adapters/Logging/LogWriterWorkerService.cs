using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Worker;
using Wing.EntityStore;

namespace Wing.Adapters.Logging
{
    public class LogWriterWorkerService : IWorkerService
    {
        private IEntityStore _store;
        private LogWriterBufferService _logBuffer;
        private IWorkerServiceContext _context;
        private List<LogEntryEntity> _entries = new List<LogEntryEntity>();

        public void Initialize(IWorkerServiceContext context)
        {
            _store = context.Services.GetInstance<IEntityStoreManager>().CreateStore("LogData");
            _store.RegisterEntity<LogEntryEntity>();
            _context = context;
            _logBuffer = context.Services.GetInstance<LogWriterBufferService>();
            _logBuffer.OnEntryAdded += new EventHandler(_logBuffer_OnEntryAdded);
            _context.Controller.WakeUp();
        }

        void _logBuffer_OnEntryAdded(object sender, EventArgs e)
        {
            _context.Controller.WakeUp();
        }

        public void Execute(IWorkerServiceContext context)
        {
            LogEntryEntity entry = null;
            while ((entry = _logBuffer.Pop()) != null)
                _entries.Add(entry);
            _store.Save(_entries.ToArray());
        }
    }
}
