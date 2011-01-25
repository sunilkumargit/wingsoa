using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Worker
{
    public interface IWorkerServiceRecoveryPolicy
    {
        void OnServiceFail(IWorkerServiceController subscription);
    }
}
