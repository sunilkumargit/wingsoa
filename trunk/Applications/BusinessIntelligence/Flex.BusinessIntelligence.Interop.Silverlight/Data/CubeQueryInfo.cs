using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
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
