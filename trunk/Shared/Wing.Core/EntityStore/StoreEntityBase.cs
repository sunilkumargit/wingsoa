using System;
using System.Runtime.Serialization;

namespace Wing.EntityStore
{
    [DataContract]
    public class StoreEntityBase : IStoreEntity
    {
        [DataMember]
        private Guid _instanceid = Guid.NewGuid();

        public Guid InstanceId
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _instanceid; }
        }
    }
}
