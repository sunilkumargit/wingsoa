using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Wing.Services.ClientInteropService.Contract
{
    [ServiceContract]
    public interface IClientInteropService
    {
        [OperationContract]
        string GetInfo();
    }
}
