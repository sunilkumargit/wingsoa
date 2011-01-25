using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Worker
{
    public interface IWorkerServicesManager
    {
        IWorkerServiceController RegisterService(String serviceName, IWorkerService instance, IWorkerServiceRecoveryPolicy defaultRecoveryPolicy);
        void StartService(String serviceName);
        void StopService(String serviceName);
        IEnumerable<IWorkerServiceController> GetRegisteredServices();
    }
}
