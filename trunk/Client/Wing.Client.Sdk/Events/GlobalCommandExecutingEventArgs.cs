using Wing.Composite.Events;
using Wing.Client.Sdk.Services;

namespace Wing.Client.Sdk.Events
{
    public class GlobalCommandExecutingEventArgs
    {
        public GlobalCommandExecutingEventArgs(IGlobalCommand command)
        {
            Command = command;
        }

        public IGlobalCommand Command { get; private set; }
        public bool Aborted { get; private set; }

        public void Abort() { Aborted = true; }
    }
}
