using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.EntityStore
{
    public interface IStoreEntity
    {
        Guid InstanceId { get; }
    }
}
