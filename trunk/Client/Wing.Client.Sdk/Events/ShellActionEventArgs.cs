using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Client.Sdk.Events
{
    public class ShellActionEventArgs
    {
        public ShellAction Action { get; private set; }

        public ShellActionEventArgs(ShellAction command)
        {
            Action = command;
        }
    }
}
