using System;
using System.Net;
using Wing.EntityStore;

namespace Flex.BusinessIntelligence.Data
{
    public class CubeRegistrationInfo : StoreEntityBase
    {
        [PersistentMember]
        public Guid CubeId { get; set; }

        [PersistentMember(100)]
        public String CubeName { get; set; }

        [PersistentMember(100)]
        public String ServerName { get; set; }

        [PersistentMember(100)]
        public String CatalogName { get; set; }

        [PersistentMember(512)]
        public String Description { get; set; }

        [PersistentMember(100)]
        public String UserName { get; set; }

        [PersistentMember(100)]
        public String Password { get; set; }

        [PersistentMember]
        public Guid DefaultGroupId { get; set; }

        public string GetConnectionString()
        {
            return "";
        }
    }
}