using System.Windows;
using Wing.Client.Core;
using Wing.Client.Modules.Shell.Views;
using Wing.Composite.Presentation.Regions;
using Wing.Composite.Regions;
using Wing.Modularity;
using Wing.ServiceLocation;

namespace Wing.Client.Modules.Shell
{
    [ModuleCategory(ModuleCategory.Core)]
    [ModulePriority(ModulePriority.Low)]
    [ModuleDescription("Provedor do shell do usuário")]
    [ModuleDependency("Theme")]
    public class ShellModule : ModuleBase
    {

        #region IModule Members

        public override void Initialize()
        {
            //criar o view do shell aqui.
            ServiceLocator.Current.Register<IRegionManager, RegionManager>(true);
            ServiceLocator.Current.Register<IShellView, ShellView>(true);
            ServiceLocator.Current.Register<IShellViewPresenter, ShellViewPresenter>(true);
            ServiceLocator.Current.Register<ShellController, ShellController>(true);

            var visualManager = ServiceLocator.Current.GetInstance<IRootVisualManager>();

            visualManager.Dispatch(() =>
            {
                var controller = ServiceLocator.Current.GetInstance<ShellController>();
                //setar o shell view como main view
                ServiceLocator.Current.GetInstance<IRootVisualManager>().SetRootElement((UIElement)
                    ServiceLocator.Current.GetInstance<IShellView>());
            });

        }

        public override void Initialized()
        {
            ServiceLocator.Current.GetInstance<IRootVisualManager>().Dispatch(() =>
            {
                if (Application.Current.IsRunningOutOfBrowser)
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
            });

        }

        #endregion
    }
}
