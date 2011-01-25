using System;

namespace Wing.Bootstrap
{
    public interface IBootLogger
    {
        void Log(String message, Exception ex = null);
    }
}
