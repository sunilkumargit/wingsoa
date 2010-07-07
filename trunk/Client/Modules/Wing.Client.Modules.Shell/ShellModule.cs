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

namespace Wing.Client.Modules.Shell
{
    [ModuleCategory(ModuleCategory.Core)]
    [ModuleDescription("Provedor do shell do usuário")]
    public class ShellModule : IModule
    {

        #region IModule Members

        public void Initialize()
        {
            //setar o shell view como main view
            ServiceLocator.Current.GetInstance<IRootVisualManager>().SetRootElement(new ShellView());
        }

        #endregion
    }
}
