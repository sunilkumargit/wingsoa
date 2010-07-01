using System;
namespace Wing.Soa
{
    interface ISoaServices
    {
        Uri BaseAdress { get; set; }
        void RegisterService<TContract, TService>(string serviceName, string baseAddress);
        void RegisterService<TContract, TService>();
    }
}
