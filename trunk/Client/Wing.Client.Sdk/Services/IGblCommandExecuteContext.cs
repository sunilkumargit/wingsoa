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
    public interface IGblCommandExecuteContext
    {
        IGblCommand Command { get; }
        Object Parameter { get; set; }
        GblCommandExecStatus Status { get; set; }
        bool Handled { get; set; }
        String OutMessage { get; set; }
    }
}
