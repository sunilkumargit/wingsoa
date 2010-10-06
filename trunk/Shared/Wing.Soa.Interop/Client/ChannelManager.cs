using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Wing.Soa.Interop.Client
{
    internal class ChannelManager : IChannelManager
    {
        private Object _lockObj = new Object();
        private Dictionary<Type, ICommunicationObject> _channels = new Dictionary<Type, ICommunicationObject>();
        private List<IChannelFactoryFactory> _channelFactories = new List<IChannelFactoryFactory>();

        public ChannelManager()
        {
        }

        public List<IChannelFactoryFactory> ChannelFactoryFactories { get { return _channelFactories; } }

        public TChannel GetChannel<TChannel>()
        {
            ICommunicationObject result = null;
            if (!_channels.TryGetValue(typeof(TChannel), out result))
            {
                lock (_lockObj)
                {
                    if (!_channels.TryGetValue(typeof(TChannel), out result))
                    {
                        result = CreateChannel<TChannel>();
                        _channels[typeof(TChannel)] = result;
                    }
                }
            }
            return (TChannel)result;
        }

        private ICommunicationObject CreateChannel<TChannel>()
        {
            foreach (var factory in _channelFactories)
            {
                var channel = factory.CreateChannelFactory<TChannel>();
                if (channel != null)
                {
                    var channelCommObj = (ICommunicationObject)channel.CreateChannel();
                    channelCommObj.Faulted += new EventHandler(channelCommObj_Faulted);
                    channelCommObj.Open();
                    return channelCommObj;
                }
            }
            return null;
        }

        void channelCommObj_Faulted(object sender, EventArgs e)
        {
            var commObj = (ICommunicationObject)sender;
            commObj.Faulted -= channelCommObj_Faulted;

            lock (_lockObj)
            {
                var keys = _channels.Where(p => p.Value == commObj).Select(p => p.Key);

                /* Remove all items matching the channel. 
                 * This is somewhat defensive as there should only be one instance 
                 * of the channel in the channel dictionary. */
                foreach (var key in keys)
                    _channels.Remove(key);
            }

        }
    }
}
