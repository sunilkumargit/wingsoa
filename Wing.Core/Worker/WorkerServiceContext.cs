using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Services;
using Wing.Logging;

namespace Wing.Worker
{
    class WorkerServiceContext : IWorkerServiceContext
    {
        public WorkerServiceContext(IWorkerServicesManager manager, IWorkerServiceController controller, ILogger logger)
        {
            Manager = manager;
            Controller = controller;
            Logger = logger;
        }

        public IWorkerServicesManager Manager { get; private set; }
        public IServiceLocator Services { get { return ServiceLocator.GetCurrent(); } }
        public IWorkerServiceController Controller { get; private set; }
        public ILogger Logger { get; private set; }
    }
}
