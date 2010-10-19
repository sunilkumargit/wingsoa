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
    public interface IGlobalCommand
    {
        String Name { get; }
        String Tooltip { get; }
        String Caption { get; }
        String IconSource { get; }
        GblCommandUIType UIType { get; }
        void QueryStatus(Object parameter, ref GblCommandStatus status);
        GblCommandStatus QueryStatus(Object parameter = null);
        void Execute(Object parameter, ref GblCommandExecStatus execStatus, ref bool handled, ref string outMessage);
        GblCommandExecStatus Execute(Object parameter = null);
        void AddHandler(IGlobalCommandHandler handler);
        void RemoveHandler(IGlobalCommandHandler handler);
        ReadOnlyObservableCollection<IGlobalCommandHandler> Handlers { get; }
        event GblCommandStateChanged StateChanged;
        void FireStateChanged();
    }

    public delegate void GblCommandStateChanged(IGlobalCommand command);
}