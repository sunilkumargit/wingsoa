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
        private static IServiceSyncBroker _syncBroker = new ServiceSyncBroker();
        private static IChannelManager _channelManager = new ChannelManager();

        public static IChannelManager ChannelManager { get { return _channelManager; } }

        public static TResult InvokeService<TChannel, TResult>(Func<TChannel, IServiceSyncBroker, TResult> action)
        {
            var channel = ChannelManager.GetChannel<TChannel>();
            return action(channel, _syncBroker);
        }

        public static void InvokeService<TChannel>(Action<TChannel, IServiceSyncBroker> action)
        {
            var channel = ChannelManager.GetChannel<TChannel>();
            action(channel, _syncBroker);
        }
    }
}