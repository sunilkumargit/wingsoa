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

namespace Wing.Client.Sdk
{
    public delegate void SingleEventHandler<TSenderType>(TSenderType sender);
    public delegate void SingleEventHandler<TSenderType, TEventArgsType>(TSenderType sender, TEventArgsType args);
}
