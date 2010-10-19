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
    public interface IGlobalCommandHandler
    {
        void QueryStatus(IGlobalCommand command, ref Object parameter, ref GblCommandStatus status, ref bool handled);
        void Execute(IGlobalCommand command, ref Object parameter, ref GblCommandExecStatus execStatus, ref bool handled, ref string outMessage);
    }
}
