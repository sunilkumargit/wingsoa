using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Wing.Logging;

namespace Wing.Worker
{
    class WorkerServiceController : IWorkerServiceController
    {
        private IWorkerService _instance;
        private WorkerServicesManager _manager;
        private WorkerServiceContext _context;
        private List<Exception> _errors = new List<Exception>();

        public WorkerServiceController(WorkerServicesManager manager, String serviceName, IWorkerService instance, IWorkerServiceRecoveryPolicy recoveryPolicy)
        {
            _instance = instance;
            _manager = manager;
            ServiceName = serviceName;
            RecoveryPolicy = recoveryPolicy;
        }

        public Type ServiceType
        {
            get { return _instance.GetType(); }
        }

        public string ServiceName { get; private set; }
        public IWorkerServiceRecoveryPolicy RecoveryPolicy { get; private set; }

        public void Start()
        {
            _manager.ScheduleProcess(this);
        }

        public void Stop()
        {
            if (this.Status == WorkerServiceStatus.Faulted)
                return;
            if (this.Status == WorkerServiceStatus.Running)
                this.Status = WorkerServiceStatus.Stopping;
            else
                this.Status = WorkerServiceStatus.Stopped;
        }

        public void WakeUp()
        {
            if (this.Status == WorkerServiceStatus.NotInitialized
                || this.Status == WorkerServiceStatus.Sleeping
                || this.Status == WorkerServiceStatus.Running)
                _manager.ScheduleProcess(this);
        }

        public bool HasErrors
        {
            get { return _errors.Count > 0; }
        }

        public IEnumerable<Exception> GetErrors()
        {
            return _errors;
        }

        public WorkerServiceStatus Status { get; private set; }

        private WorkerServiceContext CreateWorkerContext()
        {
            return new WorkerServiceContext(_manager, this,
                ServiceLocator
                    .GetInstance<ILogManager>()
                    .GetLogger(String.Format("WORKER:{0}", ServiceName)));
        }

        internal void ExecuteInternal()
        {
            if (Status == WorkerServiceStatus.Stopped
                || Status == WorkerServiceStatus.Stopping
                || Status == WorkerServiceStatus.Running
                || Status == WorkerServiceStatus.Faulted)
                return;

            if (Status == WorkerServiceStatus.Sleeping)
            {
                BackgroundThreadPool.Enqueue(() =>
                {
                    try
                    {
                        Status = WorkerServiceStatus.Running;
                        _instance.Execute(_context);
                        if (Status == WorkerServiceStatus.Running)
                            Status = WorkerServiceStatus.Sleeping;
                        else if (Status == WorkerServiceStatus.Stopping)
                            Status = WorkerServiceStatus.Stopped;
                    }
                    catch (Exception ex)
                    {
                        _context.Logger.LogException("Error on execute service", ex, Priority.High);
                        _errors.Add(ex);
                        Status = WorkerServiceStatus.Faulted;
                    }
                });
            }
        }

        internal void InitializeInternal()
        {
            if (Status != WorkerServiceStatus.NotInitialized)
                return;

            if (_context == null)
                _context = CreateWorkerContext();

            if (Status == WorkerServiceStatus.NotInitialized)
            {
                try
                {
                    _instance.Initialize(_context);
                    Status = WorkerServiceStatus.Sleeping;
                }
                catch (Exception ex)
                {
                    _context.Logger.LogException("Error on initialize service", ex, Priority.High);
                    _errors.Add(ex);
                    Status = WorkerServiceStatus.Faulted;
                    return;
                }
            }
        }
    }
}
