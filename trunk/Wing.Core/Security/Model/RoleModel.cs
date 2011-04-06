using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.EntityStore;

namespace Wing.Security.Model
{
    public class RoleModel : StoreEntity
    {
        [PersistentMember]
        public string RoleName { get; set; }

        [PersistentMember]
        public string GroupName { get; set; }
    }
}
