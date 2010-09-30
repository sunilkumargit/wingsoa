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
using Wing.Events;
using Wing.Client.Sdk.Events;

namespace Wing.Client.Sdk.Services
{
    public class GlobalCommand : IGlobalCommand
    {
        private ObservableCollection<IGlobalCommandHandler> _handlers = new ObservableCollection<IGlobalCommandHandler>();
        private ReadOnlyObservableCollection<IGlobalCommandHandler> _handlersReadOnly;

        public GlobalCommand(String name, String caption, String toolTip)
        {
            Assert.NullArgument(name, "name");
            _handlersReadOnly = new ReadOnlyObservableCollection<IGlobalCommandHandler>(_handlers);
            Name = name;
            Caption = caption;
            Tooltip = Tooltip;
        }

        public string Name { get; private set; }
        public string Tooltip { get; private set; }
        public string Caption { get; private set; }

        public void AddHandler(IGlobalCommandHandler handler)
        {
            if (_handlers.Contains(handler))
                return;
            _handlers.Add(handler);
            FireStateChanged();
        }

        public void RemoveHandler(IGlobalCommandHandler handler)
        {
            _handlers.Remove(handler);
            FireStateChanged();
        }

        public virtual void QueryStatus(object parameter, ref GblCommandStatus status)
        {
            bool handled = false;
            status = GblCommandStatus.Enabled;
            var en = _handlers.GetEnumerator();
            while (en.MoveNext() && !handled)
                en.Current.QueryStatus(this, parameter, ref status, ref handled);
        }

        public GblCommandStatus QueryStatus(Object parameter = null)
        {
            var status = GblCommandStatus.Enabled;
            QueryStatus(parameter, ref status);
            return status;
        }

        public virtual void Execute(object parameter, ref GblCommandExecStatus execStatus, ref bool handled, ref string outMessage)
        {
            if (QueryStatus(parameter) == GblCommandStatus.Enabled)
            {
                var en = _handlers.GetEnumerator();
                while (en.MoveNext() && !handled)
                    en.Current.Execute(this, parameter, ref execStatus, ref handled, ref outMessage);
            }
        }

        public GblCommandExecStatus Execute(Object parameter = null)
        {
            var execStatus = GblCommandExecStatus.Executed;
            var handled = false;
            var outMessage = "";
            Execute(parameter, ref execStatus, ref handled, ref outMessage);
            return execStatus;
        }

        public ReadOnlyObservableCollection<IGlobalCommandHandler> Handlers { get { return _handlersReadOnly; } }

        public event GblCommandStateChanged StateChanged;

        public void FireStateChanged()
        {
            if (StateChanged != null)
                StateChanged.Invoke(this);
        }
    }
}