using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Wing.Server.Soa;
using Wing.Soa.Interop;

namespace Wing.Server.Modules.SoaServicesManager
{
    public class SoaServicesManager : ISoaServicesManager
    {
        private Dictionary<String, ISoaServiceHost> _services = new Dictionary<string, ISoaServiceHost>();
        private ObservableCollection<ISoaServiceHost> _servicesList = new ObservableCollection<ISoaServiceHost>();
        private ReadOnlyObservableCollection<ISoaServiceHost> _servicesListReadOnly;
        private ISoaServiceHostBuilder _hostBuilder;

        public SoaServicesManager(ISoaServiceHostBuilder hostBuilder)
        {
            _hostBuilder = hostBuilder;
            _servicesListReadOnly = new ReadOnlyObservableCollection<ISoaServiceHost>(_servicesList);
        }

        #region ISoaServicesManager Members

        public ISoaServiceHost RegisterService(SoaServiceDescriptor descriptor, bool autoStart = true, object singletonInstance = null)
        {
            if (GetService(descriptor.ServiceName) != null)
                throw new Exception("Um serviço já foi registrado com este nome.");
            var host = _hostBuilder.BuildServiceHost(descriptor, singletonInstance);
            _services[descriptor.ServiceName] = host;
            _servicesList.Add(host);
            if (autoStart)
                host.Start();
            return host;
        }

        public ISoaServiceHost GetService(string serviceName)
        {
            ISoaServiceHost host = null;
            _services.TryGetValue(serviceName, out host);
            if (host == null && !serviceName.EndsWith("Service"))
                return GetService(serviceName + "Service");
            return host;
        }

        public ISoaServiceHost RemoveService(string serviceName)
        {
            var host = GetService(serviceName);
            if (host.State == SoaServiceState.Running)
                host.Stop();
            _services.Remove(serviceName);
            _servicesList.Remove(host);
            return host;
        }

        public ReadOnlyObservableCollection<ISoaServiceHost> Services
        {
            get { return _servicesListReadOnly; }
        }

        public ISoaServiceHostBuilder HostBuilder { get { return _hostBuilder; } }

        #endregion
    }
}