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
using Wing.Utils;
using Wing.ServiceLocation;

namespace Wing.Client.Sdk.Services
{
    public class GlobalDelegateCommand<TParameterType> : GlobalCommand where TParameterType : class
    {
        private string _name;
        private Action<TParameterType, IGlobalCommand> _executeDelegate;

        public GlobalDelegateCommand(String name, Action<TParameterType> action) : this(name, (p, c) => action.Invoke(p)) { }

        public GlobalDelegateCommand(String name, Action<TParameterType, IGlobalCommand> action)
        {
            Assert.NullArgument(name, "name");
            Assert.NullArgument(action, "action");
            _name = name;
            _executeDelegate = action;
        }

        protected override void ExecuteCommand(object parameter)
        {
            _executeDelegate.Invoke(parameter as TParameterType, this);
        }

        public override string Name
        {
            get { return _name; }
        }
    }

    public class GlobalDelegateCommand : GlobalDelegateCommand<object>
    {
        public GlobalDelegateCommand(String name, Action<Object> action) : this(name, (p, c) => action.Invoke(p)) { }

        public GlobalDelegateCommand(String name, Action<Object, IGlobalCommand> action) : base(name, action) { }

        public GlobalDelegateCommand(String name, Action action) : this(name, (o) => action.Invoke()) { }
    }
}