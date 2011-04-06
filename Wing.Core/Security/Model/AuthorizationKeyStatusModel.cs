using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.EntityStore;

namespace Wing.Security.Model
{
    public class AuthorizationKeyStatusModel : StoreEntity
    {
        [PersistentMember(4096)]
        public String AuthorizationKey { get; set; }

        [PersistentMember]
        public String AccountType { get; set; }

        [PersistentMember]
        public String AccountName { get; set; }

        [PersistentMember]
        public int AuthorizationStatusIndex { get; set; }

        public AuthorizationStatus Status
        {
            get { return (AuthorizationStatus)AuthorizationStatusIndex; }
            set { AuthorizationStatusIndex = (int)value; }
        }
    }
}
