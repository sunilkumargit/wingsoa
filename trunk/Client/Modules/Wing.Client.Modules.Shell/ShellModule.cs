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

namespace Wing.Client.Modules.Shell
{
    [ModuleCategory(ModuleCategory.Core)]
    [ModulePriority(ModulePriority.Higher)]
    [ModuleDescription("Provedor do shell do usuário")]
    public class ShellModule : IModule
    {

        #region IModule Members

        public void Initialize()
        {
            //criar o view do shell aqui.
            ServiceLocator.Current.Register<ShellView>(new ShellView());

        }

        public void Initialized()
        {
            //setar o shell view como main view
            ServiceLocator.Current.GetInstance<IRootVisualManager>().SetRootElement(
                ServiceLocator.Current.GetInstance<ShellView>());
        }

        #endregion

        /*
        public void AddResourceDictionary(String assetName)
        {
            var resUri = new Uri(String.Format("/Wing.Client.Modules.Shell;component/Assets/{0}.xaml", assetName), UriKind.Relative);
            var resDic = new ResourceDictionary();
            resDic.Source = resUri;
            Application.Current.Resources.MergedDictionaries.Add(resDic);
        }*/
    }
}
