using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.EntityStore
{
    public class StoreEntityBase : IStoreEntity
    {
        private Guid _instanceid = Guid.NewGuid();

        public Guid InstanceId
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _instanceid; }
        }
    }
}
