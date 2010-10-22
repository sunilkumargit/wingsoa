using System.ServiceModel;

namespace Wing.Soa.Interop.Client
{
    public interface IChannelFactoryFactory
    {
        ChannelFactory<TChannel> CreateChannelFactory<TChannel>();
    }
}
