using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Server.Core
{
    public interface IBootstrapper
    {
        void Run(BootstrapSettings settings);
    }
}