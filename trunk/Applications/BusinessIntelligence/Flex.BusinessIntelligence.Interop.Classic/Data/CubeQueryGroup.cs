using System;
using System.Net;
using Wing.EntityStore;

namespace Flex.BusinessIntelligence.Data
{
    public class CubeQueryGroup : StoreEntityBase
    {
        [PersistentMember]
        public Guid GroupId { get; set; }

        [PersistentMember(256)]
        public String GroupName { get; set; }
    }
}