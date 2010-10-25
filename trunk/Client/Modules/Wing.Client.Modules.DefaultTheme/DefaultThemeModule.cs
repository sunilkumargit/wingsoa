using System.Windows;
using Wing.Client.Core;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Controls;
using Wing.Modularity;
using Wing.ServiceLocation;

namespace Wing.Client.Modules.DefaultTheme
{
    [Module("Theme")]
    [ModuleCategory(ModuleCategory.Core)]
    [ModulePriority(ModulePriority.Higher)]
    [ModuleDescription("Tema padrão da interface do usuário")]
    [ModuleLoadGroup(WingClientModuleLoadGroups.Initialization)]
    public class DefaultThemeModule : ModuleBase
    {
        public override void Initialize()
        {
            // adicionar os estilos ao dicionário global
            VisualContext.Sync(() =>
            {
                var visualManager = ServiceLocator.GetInstance<IRootVisualManager>();
                visualManager.AddResourceDictionary("Wing.Client.Modules.DefaultTheme", "WingTheme");
            });

            ServiceLocator.Register<ViewBagDefaultContainer, ViewBagDefaultContainer>();
        }

        public override void Run()
        {
            // maximizar a janela principal.
            VisualContext.Async(() =>
            {
                if (Application.Current.IsRunningOutOfBrowser)
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
            });
        }
    }
}
