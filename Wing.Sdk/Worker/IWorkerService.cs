using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Worker
{
    public interface IWorkerService
    {
        void Initialize(IWorkerServiceContext context);
        void Execute(IWorkerServiceContext context);
    }
}
