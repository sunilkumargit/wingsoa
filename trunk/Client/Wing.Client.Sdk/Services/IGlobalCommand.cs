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
    public interface IGlobalCommand : ICommand
    {
        String Name { get; }
        String Hint { get; }
        String Caption { get; set; }
        bool IsEnabled { get; set; }
        bool IsVisible { get; set; }
        bool IsActive { get; }
        event SingleEventHandler<IGlobalCommand> StateChanged;
    }
}