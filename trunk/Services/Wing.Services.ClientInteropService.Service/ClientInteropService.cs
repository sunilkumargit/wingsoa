using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Wing.Services.ClientInteropService.Contract;

namespace Wing.Services.ClientInteropService.Service
{
    public class ClientInteropService : IClientInteropService
    {
        #region IClientInteropService Members

        public string GetInfo()
        {
            return "Service ok";
        }

        #endregion
    }
}
