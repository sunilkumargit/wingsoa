using System;
using Wing.EntityStore;

namespace Wing.Services.IdentityManagerService
{
    public class UserEntity : StoreEntityBase
    {
        [PersistentMember]
        public String UserName { get; set; }

        [PersistentMember]
        public String Password { get; set; }

        [PersistentMember]
        public String Name { get; set; }

        [PersistentMember]
        public Boolean IsAdministrator { get; set; }

        [PersistentMember(maxLength: 100)]
        public String Email { get; set; }
    }
}