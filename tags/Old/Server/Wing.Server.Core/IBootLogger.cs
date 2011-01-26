using System;

namespace Wing.Server.Core
{
    public interface IBootLogger
    {
        void Log(String message, Exception ex = null);
    }
}
