using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.EntityStore;

namespace Wing.Server.Modules.ServerConfigManager.Entities
{
    public abstract class SettingsPropertyEntityBase : StoreEntityBase
    {
        [PersistentMember(256)]
        public String GroupName { get; set; }

        [PersistentMember(256)]
        public String SectionName { get; set; }

        [PersistentMember(256)]
        public String PropertyName { get; set; }

        [PersistentMember(Int32.MaxValue)]
        public String Value { get; set; }
    }
}
