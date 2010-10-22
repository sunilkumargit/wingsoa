using Flex.BusinessIntelligence.WingClient.Views.Home;
using Flex.BusinessIntelligence.WingClient.Views.Root;
using Wing.Modularity;
using Wing.ServiceLocation;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Services;
using Wing.EntityStore;
using Wing.Soa.Interop.Client;
using Flex.BusinessIntelligence.Interop.Services;
using System.Collections.Generic;
using Flex.BusinessIntelligence.Data;
using System;
using Wing.Soa.Interop;
using Flex.BusinessIntelligence.WingClient.Views.CubesConfig;
using Flex.BusinessIntelligence.WingClient.Views.CubeProperties;
using Flex.BusinessIntelligence.WingClient.Views.RegisterCube;
using Flex.BusinessIntelligence.WingClient.Views.QueriesList;
using Flex.BusinessIntelligence.Client.Interop;
using Flex.BusinessIntelligence.WingClient.Views.PivotGrid;
using Wing.Client.Core;

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
            AssembliesAlias.RegisterAssemblyAlias("bi", "Flex.BusinessIntelligence.Client.Interop");

            // root do bi
            ServiceLocator.Register<IBIRootPresenter, BIRootPresenter>(true);

            // home do bi
            ServiceLocator.Register<IBIHomeView, BIHomeView>(true);
            ServiceLocator.Register<BIHomePresenter, BIHomePresenter>(true);

            // presentation models publicos
            ServiceLocator.Register<BICubesConfigPresentationModel, BICubesConfigPresentationModel>(true);
            ServiceLocator.Register<BIQueriesListPresentationModel, BIQueriesListPresentationModel>(true);

            // configuração de cubos
            ServiceLocator.Register<BICubesConfigView, BICubesConfigView>();
            ServiceLocator.Register<BICubesConfigPresenter, BICubesConfigPresenter>(true);

            // lista de consultas
            ServiceLocator.Register<BIQueriesListView, BIQueriesListView>();
            ServiceLocator.Register<BIQueriesListPresenter, BIQueriesListPresenter>(true);

            // propriedades do cubo
            ServiceLocator.Register<CubePropertiesPresenter, CubePropertiesPresenter>();

            // proxy para o servicos Soa do BI
            ServiceLocator.Register<ICubeServicesProxy, CubeServicesProxy>(true);

            Wing.Olap.Services.ServiceManager.BaseAddress = ServiceLocator.GetInstance<BootstrapSettings>().ServerBaseAddress.ToString();

            CreateCommands();
        }

        private void CreateCommands()
        {
            // navegar para a home
            CommandsManager.CreateCommand(BICommandNames.NavigateHome, "Meu BI").AddNavigateHandler<BIHomePresenter>();

            // navegar para consultas
            CommandsManager.CreateCommand(BICommandNames.NavigateQueries, "Consultas")
                .AddNavigateHandler<BIQueriesListPresenter, BIHomePresenter>();

            // navegar para os cubos
            CommandsManager.CreateCommand(BICommandNames.NavigateCubes, "Cubos").AddNavigateHandler<BICubesConfigPresenter, BIHomePresenter>();

            //propriedades do cubo
            CommandsManager.CreateCommand(BICommandNames.CubeShowProperties, "Propriedades do cubo")
                .AddDelegateHandler(new CommandExecuteDelegate((ctx) =>
                {
                    ctx.Handled = true;
                    var info = ctx.Parameter as CubeRegistrationInfo;
                    if (info == null)
                    {
                        ctx.Status = GblCommandExecStatus.Error;
                        ctx.OutMessage = "Command parameter is null or has a invalid value";
                    }
                    else
                    {
                        var presenter = ServiceLocator.GetInstance<CubePropertiesPresenter>();
                        presenter.Model.CubeInfo = info;
                        ServiceLocator.GetInstance<IShellService>().ShowPopup(presenter);
                    }
                }), new CommandQueryStatusDelegate((ctx) =>
                {
                    if (ctx.Parameter == null)
                    {
                        ctx.Status = GblCommandStatus.NotSupported;
                        ctx.Handled = true;
                    }
                }));

            // registrar um novo cubo
            CommandsManager.CreateCommand(BICommandNames.RegisterCube, "Registrar um novo cubo")
                .AddDelegateHandler((ctx) =>
                {
                    ctx.Handled = true;
                    var presenter = ServiceLocator.GetInstance<CubeRegisterPresenter>();
                    ServiceLocator.GetInstance<IShellService>().ShowPopup(presenter);
                });

            // nova consulta ao cubo
            CommandsManager.CreateCommand(BICommandNames.NewCubeQuery, "Consultar cubo")
                .AddDelegateHandler((ctx) =>
                {
                    //criar o presenter e o presentation model
                    var presentationModel = new PivotGridPresentationModel((CubeRegistrationInfo)ctx.Parameter);
                    var presenter = new PivotGridPresenter(presentationModel);
                    ServiceLocator.GetInstance<IBIRootPresenter>()
                        .Navigate(presenter);
                }, (ctx) =>
                {
                    if (ctx.Parameter == null)
                        ctx.Status = GblCommandStatus.Disabled;
                });

        }

        public override void Initialized()
        {
            // ativar o presenter;
            var presenter = ServiceLocator.GetInstance<IBIRootPresenter>();
            presenter.Navigate(ServiceLocator.GetInstance<BIHomePresenter>());

            // registrar os menus iniciais e vincula-los aos comandos globais
            var homeMenu = ServiceLocator.GetInstance<IBIHomeView>().MainMenu;
            homeMenu.CreateItem(BIMainMenuNames.Home, CommandsManager.GetCommand(BICommandNames.NavigateHome));
            homeMenu.CreateItem(BIMainMenuNames.Queries, CommandsManager.GetCommand(BICommandNames.NavigateQueries));
            homeMenu.CreateItem(BIMainMenuNames.Config, "Configurações").RedirectSelectionToFirstChild = true;
            homeMenu.CreateChildItem(BIMainMenuNames.Cubes, BIMainMenuNames.Config, CommandsManager.GetCommand(BICommandNames.NavigateCubes));
        }
    }

}
