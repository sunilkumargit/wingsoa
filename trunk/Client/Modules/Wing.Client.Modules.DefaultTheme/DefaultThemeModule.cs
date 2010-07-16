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

namespace Wing.Client.Modules.DefaultTheme
{
    [ModuleCategory(ModuleCategory.Init)]
    [ModulePriority(ModulePriority.Higher)]
    [ModuleDescription("Tema padrão da interface do usuário")]
    public class DefaultThemeModule:IModule
    {
        public void Initialize()
        {
            var visualManager = ServiceLocator.Current.GetInstance<IRootVisualManager>();
            visualManager.AddResourceDictionary("Wing.Client.Modules.DefaultTheme", "Brushes");
        }

        public void Initialized()
        {
           //
        }
    }
}
