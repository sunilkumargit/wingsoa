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
using Wing.Modularity;
using Wing.ServiceLocation;
using Wing.Client.Core;

namespace Wing.Client.Modules.Login
{
    [ModuleCategory(ModuleCategory.Init)]
    [ModuleDescription("Interface de login do usuário")]
    public class LoginModule : IModule
    {

        #region IModule Members

        public void Initialize()
        {
        }

        #endregion
    }
}
