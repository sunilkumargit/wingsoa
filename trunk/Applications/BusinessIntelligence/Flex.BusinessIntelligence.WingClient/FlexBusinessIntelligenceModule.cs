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

            ServiceLocator.Current.Register<IBIRootPresenter, BIRootPresenter>(true);

            ServiceLocator.Current.Register<IBIHomeView, BIHomeView>(true);
            ServiceLocator.Current.Register<BIHomePresenter, BIHomePresenter>(true);

            ServiceLocator.Current.Register<BICubesConfigView, BICubesConfigView>();
            ServiceLocator.Current.Register<BICubesConfigPresenter, BICubesConfigPresenter>(true);

            ServiceLocator.Current.Register<CubePropertiesPresenter, CubePropertiesPresenter>();

            ServiceLocator.Current.Register<ICubeServicesProxy, CubeServicesProxy>(true);

            //criar os commandos basicos
            var navigateHomeCommand = CommandsManager.CreateCommand(BICommandNames.NavigateHome, "Meu BI")
                .AddNavigateHandler<BIHomePresenter>();

            //consultas
            var navigateQueriesCommand = CommandsManager.CreateCommand(BICommandNames.NavigateQueries, "Consultas")
                .AddNavigateHandler<BIQueriesListPresenter, BIHomePresenter>();

            //cubos
            var navigateConfigCubosCommand = CommandsManager.CreateCommand(BICommandNames.NavigateCubes, "Cubos")
                .AddNavigateHandler<BICubesConfigPresenter, BIHomePresenter>();

            //propriedades do cubo
            var cubeShowPropertiesCommand = CommandsManager.CreateCommand(BICommandNames.CubeShowProperties, "Propriedades do cubo")
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
                        var presenter = ServiceLocator.Current.GetInstance<CubePropertiesPresenter>();
                        presenter.Model.CubeInfo = info;
                        ServiceLocator.Current.GetInstance<IShellService>().ShowPopup(presenter);
                    }
                }), new CommandQueryStatusDelegate((ctx) =>
                {
                    if (ctx.Parameter == null)
                    {
                        ctx.Status = GblCommandStatus.NotSupported;
                        ctx.Handled = true;
                    }
                }));

            //novo cubo
            var registerCubeCommand = CommandsManager.CreateCommand(BICommandNames.RegisterCube, "Registrar um novo cubo")
                .AddDelegateHandler((ctx) =>
                {
                    ctx.Handled = true;
                    var presenter = ServiceLocator.Current.GetInstance<CubeRegisterPresenter>();
                    ServiceLocator.Current.GetInstance<IShellService>().ShowPopup(presenter);
                });
        }

        public override void Initialized()
        {
            // ativar o presenter;
            var presenter = ServiceLocator.Current.GetInstance<IBIRootPresenter>();
            presenter.Navigate(ServiceLocator.Current.GetInstance<BIHomePresenter>());

            // registrar os menus iniciais e vincula-los aos comandos globais
            var homeMenu = ServiceLocator.Current.GetInstance<IBIHomeView>().MainMenu;
            homeMenu.CreateItem(BIMainMenuNames.Home, CommandsManager.GetCommand(BICommandNames.NavigateHome));
            homeMenu.CreateItem(BIMainMenuNames.Queries, CommandsManager.GetCommand(BICommandNames.NavigateQueries));
            homeMenu.CreateItem(BIMainMenuNames.Config, "Configurações").RedirectSelectionToFirstChild = true;
            homeMenu.CreateChildItem(BIMainMenuNames.Cubes, BIMainMenuNames.Config, CommandsManager.GetCommand(BICommandNames.NavigateCubes));
        }
    }

}
