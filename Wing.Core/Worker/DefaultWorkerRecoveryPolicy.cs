using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Worker
{
    class DefaultWorkerRecoveryPolicy: IWorkerServiceRecoveryPolicy
    {
        public void OnServiceFail(IWorkerServiceController subscription)
        {
            return;
        }
    }
}
