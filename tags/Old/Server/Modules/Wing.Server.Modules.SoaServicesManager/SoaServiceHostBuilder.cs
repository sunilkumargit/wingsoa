using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using Wing.Server.Soa;
using Wing.Soa.Interop;

namespace Wing.Server.Modules.SoaServicesManager
{
    public class SoaServiceHostBuilder : ISoaServiceHostBuilder
    {
        public SoaServiceHostBuilder()
        {
            Strategies = new ObservableCollection<ISoaServiceHostBuilderStrategy>();
        }

        #region ISoaServiceHostBuilder Members

        public ISoaServiceHost BuildServiceHost(SoaServiceDescriptor descriptor, object singletonInstance)
        {
            ServiceHost serviceHost = null;
            var section = SettingsManager.GetSection("Services", "BaseConfiguration");
            var defaultAddress = new Uri(section.GetString("baseUri"));
            foreach (var strategy in Strategies)
                strategy.Execute(descriptor, ref serviceHost, ref singletonInstance, ref defaultAddress);
            var host = new SoaServiceHost();
            host.ServiceAddress = defaultAddress;
            host.Descriptor = descriptor;
            host.ServiceHost = serviceHost;
            host.SingletonInstance = singletonInstance;
            return host;
        }

        public ObservableCollection<ISoaServiceHostBuilderStrategy> Strategies { get; private set; }

        #endregion
    }
}