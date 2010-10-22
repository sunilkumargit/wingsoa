using System;

namespace Wing.Client.Sdk.Services
{
    public interface IGblCommandExecuteContext
    {
        IGblCommand Command { get; }
        Object Parameter { get; set; }
        GblCommandExecStatus Status { get; set; }
        bool Handled { get; set; }
        String OutMessage { get; set; }
    }
}
