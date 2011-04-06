using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.EntityStore;

namespace Wing.Security.Model
{
    public class UserModel : StoreEntity
    {
        [PersistentMember]
        public String SchemaId { get; set; }

        [PersistentMember]
        public string Login { get; set; }

        [PersistentMember]
        public string Name { get; set; }

        [PersistentMember]
        public string Email { get; set; }

        [PersistentMember]
        public bool Active { get; set; }

        [PersistentMember]
        public String PasswordHash { get; set; }
    }
}
