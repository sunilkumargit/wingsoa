using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Wing.Soa.Interop.Client
{
    public static class SoaClientManager
    {
        private static ISoaClientServiceMetadataProvider _metadaProvider;
        private static Dictionary<Type, ChannelFactory> _channelFactories = new Dictionary<Type, ChannelFactory>();

        public static void SetMetadataProvider(ISoaClientServiceMetadataProvider metadataProvider)
        {
            _metadaProvider = metadataProvider;
        }

        public static TResult ExecuteAndCleanUp<TChannel, TResult>(Func<TChannel, TResult> action)
        {
            var channel = CreateChannel<TChannel>();
            try
            {
                return Execute<TChannel, TResult>(channel, action);
            }
            finally
            {
                CloseChannel(channel);
            }
        }

        public static void ExecuteAndCleanUp<TChannel>(Action<TChannel> action)
        {
            ExecuteAndCleanUp<TChannel, Object>((channel) =>
            {
                action(channel);
                return null;
            });
        }

        public static TResult Execute<TChannel, TResult>(TChannel channel, Func<TChannel, TResult> action)
        {
            var retries = 0;
            TResult result;
            Exception exception = null;
            while (++retries <= 3)
            {
                try
                {
                    result = action(channel);
                    return result;
                }
                catch (TimeoutException tex)
                {
                    retries++;
                    exception = tex;
                }
                catch (CommunicationException cex)
                {
                    retries++;
                    exception = cex;
                }
                catch (Exception ex)
                {
                    exception = ex;
                    break;
                }
            }
            //não conseguiu executar, exception...
            throw exception;
        }

        public static void Execute<TChannel>(TChannel channel, Action<TChannel> action)
        {
            Execute<TChannel, object>(channel, (channel_) =>
            {
                action(channel_);
                return null;
            });
        }

        private static object _lockObject = new Object();

        public static TChannel CreateChannel<TChannel>()
        {
            lock (_lockObject)
            {
                if (!_channelFactories.ContainsKey(typeof(TChannel)))
                {
                    _metadaProvider.GetServiceConnectionInfoByContractType(typeof(TChannel), (connInfo) =>
                    {
                        if (connInfo == null)
                            throw new Exception("Service reference does not found. " + typeof(TChannel).FullName);
                        var binding = CreateBinding(connInfo);
                        var endPoint = CreateEndpoint(connInfo);
                        _channelFactories[typeof(TChannel)] = new ChannelFactory<TChannel>(binding, endPoint);
                    });
                }
            }
            var factory = (ChannelFactory<TChannel>)_channelFactories[typeof(TChannel)];
            return factory.CreateChannel();
        }

        private static Binding CreateBinding(SoaServiceConnectionInfo connInfo)
        {
            switch (connInfo.ServiceBindingMode)
            {
                case SoaServiceBindingMode.BasicHttp: return new BasicHttpBinding();
            }
            return null;
        }

        private static EndpointAddress CreateEndpoint(SoaServiceConnectionInfo connInfo)
        {
            return new EndpointAddress(connInfo.Address);
        }

        public static void CloseChannel(Object channel)
        {
            var commObj = channel as ICommunicationObject;

            if (commObj.State == CommunicationState.Faulted)
                commObj.Abort();
            else if (commObj.State != CommunicationState.Closed)
            {
                try
                {
                    commObj.Close();
                }
                catch (CommunicationException)
                {
                    commObj.Abort();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
