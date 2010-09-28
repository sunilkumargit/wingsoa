using Flex.BusinessIntelligence.WingClient.Views.Home;
using Flex.BusinessIntelligence.WingClient.Views.Root;
using Wing.Modularity;
using Wing.ServiceLocation;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Services;

namespace Flex.BusinessIntelligence.WingClient
{
    [Module("FlexBI")]
    [ModuleDescription("Flex Business Intelligence - UI")]
    [ModuleCategory(ModuleCategory.Init)]
    [ModulePriority(ModulePriority.High)]
    public class FlexBusinessIntelligenceModule : ModuleBase
    {
        public override void Initialize()
        {
            //registrar primeiro este assembly com um alias bi
            AssembliesAlias.RegisterAssemblyAliasOfType("bi", this.GetType());
            AssembliesAlias.RegisterAssemblyAlias("bi", "Flex.BusinessIntelligence.Interop");

            ServiceLocator.Current.Register<IBIRootPresenter, BIRootPresenter>(true);

            ServiceLocator.Current.Register<IBIHomeView, BIHomeView>(true);
            ServiceLocator.Current.Register<BIHomePresenter, BIHomePresenter>(true);

            //criar os commandos basicos
            var navigateHomeCommand = new GlobalNavigateCommand<BIHomePresenter>(BICommandNames.NavigateHome) { Caption = "Home" };
            //var nagigateMyReports = new GlobalDelegateCommand<BIMyReportsPresenter>(BICommandNames.NavigateMyQueries) { Caption = "Minhas consultas" }

            // registra-los no command manager.
            var commandManager = ServiceLocator.Current.GetInstance<IGlobalCommandsManager>();
            commandManager.RegisterCommand(navigateHomeCommand);
        }

        public override void Initialized()
        {
            // ativar o presenter;
            var presenter = ServiceLocator.Current.GetInstance<IBIRootPresenter>();
            presenter.Navigate(ServiceLocator.Current.GetInstance<BIHomePresenter>());

            var commandManager = ServiceLocator.Current.GetInstance<IGlobalCommandsManager>();
            // registrar os menus iniciais e vincula-los aos comandos globais
            var homeMenu = ServiceLocator.Current.GetInstance<IBIHomeView>().MainMenu;
            var item = homeMenu.CreateItem(BIMainMenuNames.Home, commandManager.GetCommand(BICommandNames.NavigateHome));
        }
    }

}
