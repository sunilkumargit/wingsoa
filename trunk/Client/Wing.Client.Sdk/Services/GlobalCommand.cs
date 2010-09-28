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
    public abstract class GlobalCommand : IGlobalCommand
    {
        private string _hint;
        private string _caption;
        private bool _isEnabled;

        public GlobalCommand()
        {
            _isEnabled = true;
            _isVisible = true;
        }

        public void FireCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged.Invoke(this, new EventArgs());
            FireStateChanged();
        }

        public void FireStateChanged()
        {
            if (StateChanged != null)
                StateChanged.Invoke(this);
        }

        protected abstract void ExecuteCommand(Object parameter);

        #region IGlobalCommand Members

        public abstract string Name { get; }

        public string Hint { get { return _hint; } set { _hint = value; FireStateChanged(); } }

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; FireStateChanged(); }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; FireCanExecuteChanged(); }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; FireCanExecuteChanged(); }
        }

        public bool IsActive { get { return IsEnabled && IsVisible; } }

        public event SingleEventHandler<IGlobalCommand> StateChanged;

        #endregion

        #region ICommand Members

        public virtual bool CanExecute(object parameter)
        {
            return IsEnabled && IsVisible;
        }

        public event EventHandler CanExecuteChanged;
        private bool _isVisible;

        public virtual void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
                var commandArgs = new GlobalCommandExecutingEventArgs(this);
                eventAggregator.GetEvent<GlobalCommandExecutingEvent>().Publish(commandArgs);

                if (!commandArgs.Aborted)
                {
                    ExecuteCommand(parameter);
                    eventAggregator.GetEvent<GlobalCommandExecutedEvent>().Publish(this);
                }
            }
        }

        #endregion
    }
}