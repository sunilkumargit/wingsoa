using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.EntityStore;

namespace Wing.Security.Model
{
    public class SchemaModel : StoreEntity
    {
        [PersistentMember]
        public string SchemaId { get; set; }

        [PersistentMember]
        public string Name { get; set; }
    }
}
