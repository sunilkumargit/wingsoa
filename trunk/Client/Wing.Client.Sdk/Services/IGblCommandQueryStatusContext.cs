using System;

namespace Wing.Client.Sdk.Services
{
    public interface IGblCommandQueryStatusContext
    {
        IGblCommand Command { get; }
        Object Parameter { get; set; }
        GblCommandStatus Status { get; set; }
        bool Handled { get; set; }
    }
}
