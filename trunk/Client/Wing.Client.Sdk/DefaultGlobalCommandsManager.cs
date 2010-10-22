using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Wing.Client.Sdk.Services;

namespace Wing.Client.Sdk
{
    public class DefaultGlobalCommandsManager : IGblCommandsManager
    {
        private Dictionary<String, IGblCommand> _commands = new Dictionary<string, IGblCommand>();
        private ObservableCollection<IGblCommand> _commandsList = new ObservableCollection<IGblCommand>();
        private ReadOnlyObservableCollection<IGblCommand> _commandsListReadOnly;

        public DefaultGlobalCommandsManager()
        {
            _commandsListReadOnly = new ReadOnlyObservableCollection<IGblCommand>(_commandsList);
        }

        public IGblCommand CreateCommand(String name, String caption = "", GblCommandUIType uiType = GblCommandUIType.Button, String iconSource = "", String tooltip = "")
        {
            if (GetCommand(name) != null)
                throw new Exception("Já existe um commando com este nome");
            var result = new GblCommand(name, caption, uiType, iconSource, tooltip);
            _commands[name] = result;
            _commandsList.Add(result);
            return result;
        }

        public void RemoveCommand(string name)
        {
            var existing = GetCommand(name);
            if (existing == null) return;
            _commands.Remove(existing.Name);
            _commandsList.Remove(existing);
        }

        public IGblCommand GetCommand(string name)
        {
            IGblCommand result = null;
            _commands.TryGetValue(name, out result);
            return result;
        }

        public ReadOnlyCollection<IGblCommand> Commands
        {
            get { return _commandsListReadOnly; }
        }

        [System.Diagnostics.DebuggerStepThrough]
        public void RequeryStateNeeded()
        {
            foreach (var cmd in _commandsList)
                cmd.FireStateChanged();
        }
    }
}
