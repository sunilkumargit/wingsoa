using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Worker
{
    public enum WorkerServiceStatus
    {
        NotInitialized,
        Running,
        Sleeping,
        Stopped,
        Faulted,
        Stopping
    }
}
