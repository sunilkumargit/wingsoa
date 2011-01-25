using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Logging
{
    public interface ILogManager
    {
        ILogger GetSystemLogger();
        ILogger GetLogger(String name);
        ILogger GetLogger(Type forType);
        void RegisterLogger(ILogger logger);
    }
}
