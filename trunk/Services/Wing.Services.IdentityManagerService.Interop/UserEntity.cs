using System;
using Wing.EntityStore;
using System.Runtime.Serialization;

namespace Wing.Services.IdentityManagerService
{
    [DataContract]
    public class UserEntity : StoreEntityBase
    {
        [DataMember]
        [PersistentMember]
        public String UserName { get; set; }

        [DataMember]
        [PersistentMember]
        public String Password { get; set; }

        [DataMember]
        [PersistentMember]
        public String Name { get; set; }

        [DataMember]
        [PersistentMember]
        public Boolean IsAdministrator { get; set; }

        [DataMember]
        [PersistentMember(maxLength: 100)]
        public String Email { get; set; }
    }
}