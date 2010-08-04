using System;

namespace Wing.EntityStore
{
    public interface IStoreEntity
    {
        Guid InstanceId { get; }
    }
}
