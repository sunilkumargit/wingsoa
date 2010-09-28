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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Wing.Client.Sdk
{
    public class DefaultGlobalCommandsManager : IGlobalCommandsManager
    {
        private Dictionary<String, IGlobalCommand> _commands = new Dictionary<string, IGlobalCommand>();
        private ObservableCollection<IGlobalCommand> _commandsList = new ObservableCollection<IGlobalCommand>();
        private ReadOnlyObservableCollection<IGlobalCommand> _commandsListReadOnly;

        public DefaultGlobalCommandsManager()
        {
            _commandsListReadOnly = new ReadOnlyObservableCollection<IGlobalCommand>(_commandsList);
        }

        public IGlobalCommand RegisterCommand(IGlobalCommand command)
        {
            if (GetCommand(command.Name) != null)
                throw new Exception("Já existe um commando com este nome");
            _commands[command.Name] = command;
            _commandsList.Add(command);
            return command;
        }

        public void RemoveCommand(string name)
        {
            var existing = GetCommand(name);
            if (existing == null) return;
            _commands.Remove(existing.Name);
            _commandsList.Remove(existing);
        }

        public void ExecuteCommand(string name, object parameter)
        {
            var existing = GetCommand(name);
            if (existing != null)
                existing.Execute(parameter);
        }

        public IGlobalCommand GetCommand(string name)
        {
            IGlobalCommand result = null;
            _commands.TryGetValue(name, out result);
            return result;
        }

        public ReadOnlyCollection<IGlobalCommand> Commands
        {
            get { return _commandsListReadOnly; }
        }
    }
}
