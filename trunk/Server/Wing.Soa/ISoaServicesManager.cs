
namespace Wing.Soa
{
    public interface ISoaServicesManager
    {
        ISoaServiceHostBuilder Builder { get; }
        void RegisterService(SoaServiceDescriptor serviceDescriptor);
    }
}
