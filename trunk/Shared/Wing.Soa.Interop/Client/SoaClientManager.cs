using System;

namespace Wing.Soa.Interop.Client
{
    public static class SoaClientManager
    {
        private static IServiceSyncBroker _syncBroker = new ServiceSyncBroker();
        private static IChannelManager _channelManager = new ChannelManager();

        public static IChannelManager ChannelManager { get { return _channelManager; } }

        public static void InvokeService<TChannel>(Action<TChannel, IServiceSyncBroker> action)
        {
            var channel = ChannelManager.GetChannel<TChannel>();
            action(channel, _syncBroker);
        }
    }
}