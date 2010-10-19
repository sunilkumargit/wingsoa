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
using System.Collections.ObjectModel;

namespace Wing.Client.Sdk.Services
{
    public interface IGlobalCommandsManager
    {
        IGlobalCommand CreateCommand(String name, String caption = "", GblCommandUIType uiType = GblCommandUIType.Button, String iconSource = "", String tooltip = "");
        void RemoveCommand(String name);
        IGlobalCommand GetCommand(String name);
        ReadOnlyCollection<IGlobalCommand> Commands { get; }
        void RequeryStateNeeded();
    }
}