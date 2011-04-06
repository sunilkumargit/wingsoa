using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.EntityStore;

namespace Wing.Security.Model
{
    public class RoleSchemaRelation : StoreEntity
    {
        [PersistentMember]
        public String SchemaId { get; set; }

        [PersistentMember]
        public String RoleName { get; set; }
    }
}
