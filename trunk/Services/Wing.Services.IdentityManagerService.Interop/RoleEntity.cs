using System;
using Wing.EntityStore;

namespace Wing.Services.IdentityManagerService
{
    public class RoleEntity : StoreEntityBase
    {
        [PersistentMember]
        public String RoleName { get; set; }

        [PersistentMember]
        public String Description { get; set; }
    }
}
