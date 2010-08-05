using Wing.Client.Core;
using Wing.Modularity;
using Wing.ServiceLocation;
using System.Windows;

namespace Wing.Client.Modules.DefaultTheme
{
    [Module("Theme")]
    [ModuleCategory(ModuleCategory.Core)]
    [ModulePriority(ModulePriority.Low)]
    [ModuleDescription("Tema padrão da interface do usuário")]
    public class DefaultThemeModule : ModuleBase
    {
        public override void Initialize()
        {
            // adicionar os estilos ao dicionário global
            var visualManager = ServiceLocator.Current.GetInstance<IRootVisualManager>();
            visualManager.AddResourceDictionary("Wing.Client.Modules.DefaultTheme", "WingTheme");
        }

        public override void Run()
        {
            // maximizar a janela principal.
            ServiceLocator.Current.GetInstance<IRootVisualManager>().Dispatch(() =>
            {
                if (Application.Current.IsRunningOutOfBrowser)
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
            });
        }
    }
}
