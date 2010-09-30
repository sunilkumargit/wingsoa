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
using Wing.Client.Sdk.Services;

namespace Wing.Client.Sdk
{
    [System.Diagnostics.DebuggerStepThrough]
    public static class CommandsManager
    {
        private static IGlobalCommandsManager _cmdMngr;

        public static void SetCommandsManager(IGlobalCommandsManager commandManager)
        {
            _cmdMngr = commandManager;

        }

        public static IGlobalCommand CreateCommand(string name, string caption = "", string Tooltip = "")
        {
            return _cmdMngr.CreateCommand(name, caption, Tooltip);
        }

        public static void RemoveCommand(string name)
        {
            _cmdMngr.RemoveCommand(name);
        }

        public static IGlobalCommand GetCommand(string name)
        {
            return _cmdMngr.GetCommand(name);
        }

        public static System.Collections.ObjectModel.ReadOnlyCollection<IGlobalCommand> Commands
        {
            get { return _cmdMngr.Commands; }
        }

        public static void RequeryStateNeeded()
        {
            _cmdMngr.RequeryStateNeeded();
        }
    }
}
