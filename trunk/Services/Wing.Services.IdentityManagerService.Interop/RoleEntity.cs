using System;
using System.Runtime.Serialization;
using Wing.EntityStore;

namespace Wing.Services.IdentityManagerService
{
    [DataContract]
    public class RoleEntity : StoreEntityBase
    {
        [DataMember]
        [PersistentMember]
        public String RoleName { get; set; }

        [DataMember]
        [PersistentMember]
        public String Description { get; set; }
    }
}
