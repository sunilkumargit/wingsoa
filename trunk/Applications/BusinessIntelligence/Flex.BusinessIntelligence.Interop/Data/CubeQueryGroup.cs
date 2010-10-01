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
    public class CubeQueryGroup : StoreEntityBase
    {
        [PersistentMember]
        public Guid GroupId { get; set; }

        [PersistentMember(256)]
        public String GroupName { get; set; }
    }
}