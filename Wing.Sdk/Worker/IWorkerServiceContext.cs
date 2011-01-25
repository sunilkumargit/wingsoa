using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Services;
using Wing.Logging;

namespace Wing.Worker
{
    public interface IWorkerServiceContext
    {
        IWorkerServicesManager Manager { get; }
        IServiceLocator Services { get; }
        IWorkerServiceController Controller { get; }
        ILogger Logger { get; }
    }
}
