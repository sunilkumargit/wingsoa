namespace Wing.Soa.Interop.Client
{
    public interface IChannelManager
    {
        System.Collections.Generic.List<Wing.Soa.Interop.Client.IChannelFactoryFactory> ChannelFactoryFactories { get; }
        TChannel GetChannel<TChannel>();
    }
}
