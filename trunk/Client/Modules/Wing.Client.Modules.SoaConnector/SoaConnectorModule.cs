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
using Wing.Soa.Interop.Client;
using Wing.Soa.Interop;
using System.ServiceModel;
using Wing.ServiceLocation;
using Wing.Client.Core;
using System.Collections.Generic;

namespace Wing.Client.Modules.SoaConnector
{
    [Module("SoaConnector")]
    [ModuleDescription("Conector da arquitetura de serviços do Wing")]
    [ModuleCategory(ModuleCategory.Core)]
    [ModulePriority(ModulePriority.High)]
    public class SoaConnectorModule : ModuleBase
    {
        public override void Initialize()
        {
            base.Initialize();

            SoaClientManager.SetMetadataProvider(new SoaClientMetadataProvider());
        }
    }
}