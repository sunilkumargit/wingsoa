using System;
using Wing.EntityStore;

namespace Wing.Server.Modules.ServerStorage
{
    public class ServerStoreTraceEntity : StoreEntityBase
    {
        [PersistentMember]
        public DateTime Date { get; set; }

        [PersistentMember(512)]
        public String DBPath { get; set; }
    }
}
