using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Wing.ServiceLocation;
using Wing.Utils;

namespace Wing.Client.Sdk.Services
{
    public class DelegateCommandHandler : IGlobalCommandHandler
    {
        private CommandQueryStatusDelegate _queryStatusDelegate;
        private CommandExecuteDelegate _executeDelegate;

        public DelegateCommandHandler(CommandQueryStatusDelegate queryStatusDelegate, CommandExecuteDelegate executeDelegate)
        {
            Assert.NullArgument(queryStatusDelegate, "queryStatusDelegate");
            Assert.NullArgument(executeDelegate, "executeDelegate");
            _queryStatusDelegate = queryStatusDelegate;
            _executeDelegate = executeDelegate;
        }

        public void QueryStatus(IGlobalCommand command, ref object parameter, ref GblCommandStatus status, ref bool handled)
        {
            _queryStatusDelegate.Invoke(command, ref parameter, ref status, ref handled);
        }

        public void Execute(IGlobalCommand command, ref object parameter, ref GblCommandExecStatus execStatus, ref bool handled, ref string outMessage)
        {
            _executeDelegate.Invoke(command, ref parameter, ref execStatus, ref handled, ref outMessage);
        }
    }

    public delegate void CommandQueryStatusDelegate(IGlobalCommand command, ref object parameter, ref GblCommandStatus status, ref bool handled);
    public delegate void CommandExecuteDelegate(IGlobalCommand command, ref object parameter, ref GblCommandExecStatus execStatus, ref bool handled, ref string outMessage);
}
