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
    public class GblCommand : IGblCommand, ICommand
    {
        private ObservableCollection<IGblCommandHandler> _handlers = new ObservableCollection<IGblCommandHandler>();
        private ReadOnlyObservableCollection<IGblCommandHandler> _handlersReadOnly;

        public GblCommand(String name, String caption, GblCommandUIType uiType, String iconSource, String toolTip)
        {
            Assert.NullArgument(name, "name");
            _handlersReadOnly = new ReadOnlyObservableCollection<IGblCommandHandler>(_handlers);
            Name = name;
            Caption = caption;
            Tooltip = Tooltip;
            IconSource = iconSource;
            UIType = uiType;
        }

        public string Name { get; private set; }
        public string Tooltip { get; private set; }
        public string Caption { get; private set; }
        public string IconSource { get; private set; }
        public GblCommandUIType UIType { get; private set; }

        public void AddHandler(IGblCommandHandler handler)
        {
            if (_handlers.Contains(handler))
                return;
            _handlers.Add(handler);
            FireStateChanged();
        }

        public void RemoveHandler(IGblCommandHandler handler)
        {
            _handlers.Remove(handler);
            FireStateChanged();
        }

        public virtual void QueryStatus(object parameter, ref GblCommandStatus status)
        {
            var en = _handlers.GetEnumerator();
            IGblCommandQueryStatusContext ctx = new GblCommandHandlerContext(this, parameter);
            ctx.Status = GblCommandStatus.Enabled;
            while (en.MoveNext() && !ctx.Handled)
                en.Current.QueryStatus(ctx);
            status = ctx.Status;
        }

        public GblCommandStatus QueryStatus(Object parameter = null)
        {
            return ServiceLocator.GetInstance<ISyncBroker>().Sync<GblCommandStatus>(() =>
            {
                var status = GblCommandStatus.Enabled;
                QueryStatus(parameter, ref status);
                return status;
            });
        }

        public virtual void Execute(object parameter, ref GblCommandExecStatus execStatus, ref bool handled, ref string outMessage)
        {
            if (QueryStatus(parameter) == GblCommandStatus.Enabled)
            {
                IGblCommandExecuteContext ctx = new GblCommandHandlerContext(this, parameter);
                ctx.Status = execStatus;
                ctx.Handled = handled;
                ctx.OutMessage = outMessage;
                VisualContext.Sync(() =>
                {
                    var en = _handlers.GetEnumerator();
                    while (en.MoveNext() && !ctx.Handled)
                        en.Current.Execute(ctx);
                });
                handled = ctx.Handled;
                execStatus = ctx.Status;
                outMessage = ctx.OutMessage;
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

        public ReadOnlyObservableCollection<IGblCommandHandler> Handlers { get { return _handlersReadOnly; } }

        public event GblCommandStateChanged StateChanged;

        public void FireStateChanged()
        {
            if (StateChanged != null)
                StateChanged.Invoke(this);
        }

        public ICommand GetCommandWrapper()
        {
            return this;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return QueryStatus(parameter) == GblCommandStatus.Enabled;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { }
            remove { }
        }

        void ICommand.Execute(object parameter)
        {
            Execute(parameter);
        }
    }

    internal class GblCommandHandlerContext : IGblCommandQueryStatusContext, IGblCommandExecuteContext
    {
        public GblCommandHandlerContext(IGblCommand command, Object parameter)
        {
            Command = command;
            Parameter = parameter;
        }


        public IGblCommand Command { get; private set; }

        public object Parameter { get; set; }

        public bool Handled { get; set; }

        GblCommandStatus IGblCommandQueryStatusContext.Status { get; set; }

        GblCommandExecStatus IGblCommandExecuteContext.Status { get; set; }

        string IGblCommandExecuteContext.OutMessage { get; set; }
    }
}