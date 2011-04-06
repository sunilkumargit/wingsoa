using System;
using System.Runtime.Serialization;

namespace Wing.EntityStore
{
    [DataContract]
    public class StoreEntity
    {
        [DataMember(Name = "InstanceId")]
        internal Int32 _instanceid = 0;

        public Int32 InstanceId { get { return _instanceid; } }

    }
}
