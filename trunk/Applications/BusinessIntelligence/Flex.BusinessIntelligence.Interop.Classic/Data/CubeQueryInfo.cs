using System;
using System.Net;
using Wing.EntityStore;

namespace Flex.BusinessIntelligence.Data
{
    public class CubeQueryInfo : StoreEntityBase
    {
        [PersistentMember]
        public Guid QueryId { get; set; }

        [PersistentMember]
        public Guid GroupId { get; set; }

        [PersistentMember]
        public Guid CubeId { get; set; }

        [PersistentMember(100)]
        public String Name { get; set; }

        [PersistentMember(512)]
        public String Description { get; set; }

        [PersistentMember(Int32.MaxValue)]
        public String QueryData { get; set; }

        [PersistentMember]
        public DateTime? StartPeriod { get; set; }

        [PersistentMember]
        public DateTime? EndPeriod { get; set; }
    }
}
