using System;
using System.Collections.ObjectModel;

namespace Wing.Client.Sdk.Services
{
    public interface IGblCommandsManager
    {
        IGblCommand CreateCommand(String name, String caption = "", GblCommandUIType uiType = GblCommandUIType.Button, String iconSource = "", String tooltip = "");
        void RemoveCommand(String name);
        IGblCommand GetCommand(String name);
        ReadOnlyCollection<IGblCommand> Commands { get; }
        void RequeryStateNeeded();
    }
}