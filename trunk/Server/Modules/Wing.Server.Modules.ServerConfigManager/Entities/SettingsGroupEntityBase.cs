﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.EntityStore;

namespace Wing.Server.Modules.ServerConfigManager.Entities
{
    public abstract class SettingsGroupEntityBase : StoreEntityBase
    {
        [PersistentMember(256)]
        public String GroupName { get; set; }
    }
}
