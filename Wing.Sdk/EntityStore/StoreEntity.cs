using System;
using System.Runtime.Serialization;

namespace Wing.EntityStore
{
    [DataContract]
    public class StoreEntity 
    {
        [DataMember(Name = "InstanceId")]
        internal Guid _instanceid = Guid.NewGuid();

        public Guid InstanceId { get { return _instanceid; } }

    }
}
