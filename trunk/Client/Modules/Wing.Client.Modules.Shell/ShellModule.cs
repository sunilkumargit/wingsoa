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
using Wing.Client.Modules.Shell.Views;
using Wing.Composite.Presentation.Regions;

namespace Wing.Client.Modules.Shell
{
    [ModuleCategory(ModuleCategory.Core)]
    [ModulePriority(ModulePriority.High)]
    [ModuleDescription("Provedor do shell do usuário")]
    public class ShellModule : IModule
    {

        #region IModule Members

        public void Initialize()
        {
            //criar o view do shell aqui.
            var shellView = new ShellView();
            ServiceLocator.Current.Register<ShellView>(shellView);
        }

        public void Initialized()
        {
            //setar o shell view como main view
            ServiceLocator.Current.GetInstance<IRootVisualManager>().SetRootElement(
                ServiceLocator.Current.GetInstance<ShellView>());
        }

        #endregion
    }
}
