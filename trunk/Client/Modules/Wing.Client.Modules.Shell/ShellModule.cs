using Wing.Client.Modules.Shell.Views;
using Wing.Client.Sdk;
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
            ServiceLocator.Current.Register<INavigationHistoryService, NavigationHistoryService>(true);
            ServiceLocator.Current.Register<IShellView, ShellView>(true);
            ServiceLocator.Current.Register<IShellPresentationModel, ShellPresentationModel>(true);
            ServiceLocator.Current.Register<IShellViewPresenter, ShellViewPresenter>(true);
            ServiceLocator.Current.Register<IShellService, ShellService>(true);

            //crior os comandos de NavigateBack e Home
            var navigateBack = CommandsManager.CreateCommand(ShellCommandsNames.NavigateBack, "Voltar");
            var navigateHome = CommandsManager.CreateCommand(ShellCommandsNames.NavigateHome, "Home");
            var navigateTo = CommandsManager.CreateCommand(ShellCommandsNames.NavigateTo, "Ir para...");
        }

        public override void Initialized()
        {
            //iniciar o controlador do shell;
            var shell = ServiceLocator.Current.GetInstance<IShellService>();
            shell.StartShell();
            shell.StatusMessage("Pronto");
        }
        #endregion
    }
}
