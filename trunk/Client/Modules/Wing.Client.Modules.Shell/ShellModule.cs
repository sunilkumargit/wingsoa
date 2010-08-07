﻿using System.Windows;
using Wing.Client.Core;
using Wing.Client.Modules.Shell.Views;
using Wing.Composite.Presentation.Regions;
using Wing.Composite.Regions;
using Wing.Modularity;
using Wing.ServiceLocation;
using Wing.Client.Sdk;

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
            ServiceLocator.Current.Register<IRegionManager, RegionManager>();
            ServiceLocator.Current.Register<IShellView, ShellView>(true);
            ServiceLocator.Current.Register<IShellPresentationModel, ShellPresentationModel>(true);
            ServiceLocator.Current.Register<IShellService, ShellService>(true);
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
