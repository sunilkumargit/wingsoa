using Wing.Client.Modules.Shell.Views;
using Wing.Client.Sdk;
using Wing.Modularity;
using Wing.ServiceLocation;

namespace Wing.Client.Modules.Shell
{
    [Module("Shell")]
    [ModuleCategory(ModuleCategory.Core)]
    [ModulePriority(ModulePriority.Low)]
    [ModuleDescription("Provedor do shell do usuário")]
    [ModuleDependency("Theme")]
    public class ShellModule : ModuleBase
    {

        #region IModule Members

        public override void Initialize()
        {
            ServiceLocator.Register<INavigationHistoryService, NavigationHistoryService>(true);
            ServiceLocator.Register<IShellView, ShellView>(true);
            ServiceLocator.Register<IShellPresentationModel, ShellPresentationModel>(true);
            ServiceLocator.Register<IShellViewPresenter, ShellViewPresenter>(true);
            ServiceLocator.Register<IShellService, ShellService>(true);

            //crior os comandos de NavigateBack e Home
            var navigateBack = CommandsManager.CreateCommand(ShellCommandsNames.NavigateBack, "Voltar");
            var navigateHome = CommandsManager.CreateCommand(ShellCommandsNames.NavigateHome, "Home");
            var navigateTo = CommandsManager.CreateCommand(ShellCommandsNames.NavigateTo, "Ir para...");
        }

        public override void Initialized()
        {
            //iniciar o controlador do shell;
            var shell = ServiceLocator.GetInstance<IShellService>();
            if (shell is ShellService)
            {
                ((ShellService)shell).StartShell();
            }
            shell.StatusMessage("Pronto");
        }
        #endregion
    }
}
