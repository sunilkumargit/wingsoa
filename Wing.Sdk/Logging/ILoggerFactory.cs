using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Logging
{
    public interface ILoggerFactory
    {
        ILogger CreateLogger(String name);
    }
}
