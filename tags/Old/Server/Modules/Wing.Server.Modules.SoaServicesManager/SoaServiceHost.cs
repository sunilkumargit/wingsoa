using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using Wing.Server.Soa;
using Wing.Soa.Interop;

namespace Wing.Server.Modules.SoaServicesManager
{
    internal class SoaServiceHost : ISoaServiceHost
    {
        private ObservableCollection<Exception> _exceptions = new ObservableCollection<Exception>();
        private ReadOnlyObservableCollection<Exception> _exceptionsReadOnly;
        private SoaServiceState _state;

        public SoaServiceHost()
        {
            _exceptionsReadOnly = new ReadOnlyObservableCollection<Exception>(_exceptions);
        }

        #region ISoaServiceHost Members

        public Uri ServiceAddress { get; internal set; }
        public SoaServiceDescriptor Descriptor { get; internal set; }
        public ServiceHost ServiceHost { get; internal set; }
        public ReadOnlyObservableCollection<Exception> Exceptions { get { return _exceptionsReadOnly; } }
        public bool HasErrors { get { return _exceptions.Count > 0; } }
        public SoaServiceState State { get { return _state; } }
        public object SingletonInstance { get; internal set; }

        public void Stop()
        {
            ServiceHost.Close();
        }

        public void Start()
        {
            ServiceHost.Open();
            _state = SoaServiceState.Running;
        }

        public SoaServiceHostInfo GetStateInfo()
        {
            return new SoaServiceHostInfo()
            {
                DefaultAddress = this.ServiceAddress,
                ServiceName = this.Descriptor.ServiceName,
                State = this.State
            };
        }

        public SoaServiceConnectionInfo GetConnectionInfo()
        {
            return new SoaServiceConnectionInfo()
            {
                ServiceName = this.Descriptor.ServiceName,
                Address = this.ServiceAddress,
                ContractRef = this.Descriptor.ContractTypeRefName,
                ServiceBindingMode = SoaServiceBindingMode.BasicHttp
            };
        }

        #endregion
    }
}
