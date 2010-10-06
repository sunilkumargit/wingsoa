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
using Wing.Client.Sdk;

namespace Wing.Client.Modules.SoaConnector
{
    [Module("SoaConnector")]
    [ModuleDescription("Conector da arquitetura de serviços do Wing")]
    [ModuleCategory(ModuleCategory.Core)]
    [ModulePriority(ModulePriority.Higher)]
    public class SoaConnectorModule : ModuleBase
    {
        private SoaDefaultChannelFactoryFactory _channelFactoryFactory;

        public override void Initialize()
        {
            base.Initialize();
            _channelFactoryFactory = new SoaDefaultChannelFactoryFactory();
            SoaClientManager.ChannelManager.ChannelFactoryFactories.Add(_channelFactoryFactory);
            //invocar o servico para retornar as informações de conexão
            var settings = ServiceLocator.Current.GetInstance<BootstrapSettings>();
            var factory = new ChannelFactory<ISoaMetadataProviderService>(new BasicHttpBinding(), new EndpointAddress(settings.SoaMetadataProviderUri));
            var channel = factory.CreateChannel();
            var asyncResult = channel.BeginGetServicesConnectionInfo(null, null);
            var result = channel.EndGetServicesConnectionInfo(asyncResult);
            foreach (var p in result)
                _channelFactoryFactory.ConnectionInfo.Add(p.ContractRef, p);
        }
    }
}