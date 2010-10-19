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
        private static IGblCommandsManager _cmdMngr;

        public static void SetCommandsManager(IGblCommandsManager commandManager)
        {
            _cmdMngr = commandManager;
        }

        public static IGblCommand CreateCommand(String name, String caption = "", GblCommandUIType uiType = GblCommandUIType.Button, String iconSource = "", String tooltip = "")
        {
            return _cmdMngr.CreateCommand(name, caption, uiType, iconSource, tooltip);
        }

        public static void RemoveCommand(string name)
        {
            _cmdMngr.RemoveCommand(name);
        }

        public static IGblCommand GetCommand(string name)
        {
            return _cmdMngr.GetCommand(name);
        }

        public static System.Collections.ObjectModel.ReadOnlyCollection<IGblCommand> Commands
        {
            get { return _cmdMngr.Commands; }
        }

        public static void RequeryStateNeeded()
        {
            _cmdMngr.RequeryStateNeeded();
        }
    }
}
