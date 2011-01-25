using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Worker
{
    public interface IWorkerServiceController
    {
        Type ServiceType { get; }
        String ServiceName { get; }
        IWorkerServiceRecoveryPolicy RecoveryPolicy { get; }
        void Start();
        void Stop();
        void WakeUp();
        WorkerServiceStatus Status { get; }
        bool HasErrors { get; }
        IEnumerable<Exception> GetErrors();
    }
}
