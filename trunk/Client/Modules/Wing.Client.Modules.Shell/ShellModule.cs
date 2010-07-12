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
    [ModuleDescription("Provedor do shell do usuário")]
    public class ShellModule : IModule
    {

        #region IModule Members

        public void Initialize()
        {
            //carregar o tema
            AddResources();
            //setar o shell view como main view
            ServiceLocator.Current.GetInstance<IRootVisualManager>().SetRootElement(new ShellView());
        }

        public void AddResources()
        {
       
        }

        public void AddResourceDictionary(String assetName)
        {
            var resUri = new Uri(String.Format("/Wing.Client.Modules.Shell;component/Assets/{0}.xaml", assetName), UriKind.Relative);
            var resDic = new ResourceDictionary();
            resDic.Source = resUri;
            Application.Current.Resources.MergedDictionaries.Add(resDic);
        }

        #endregion
    }
}
