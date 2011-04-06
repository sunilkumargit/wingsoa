using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.EntityStore;

namespace Wing.Security.Model
{
    public class UserRoleRelation : StoreEntity
    {
        [PersistentMember]
        public String Login { get; set; }

        [PersistentMember]
        public String RoleName { get; set; }
    }
}
