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
using Wing.Client.Core;
using Wing.ServiceLocation;

namespace Wing.Client.Bootstrap
{
    public class Bootstrapper : IBootstrapper
    {

        #region IBootstrapper Members

        public void Run(BootstrapSettings settings)
        {
            //ServiceLocator.Current.Register<BootstrapSettings>(settings);
            //var s = ServiceLocator.Current.GetInstance<BootstrapSettings>();
        }

        #endregion
    }
}
