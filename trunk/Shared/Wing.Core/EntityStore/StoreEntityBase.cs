using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.EntityStore
{
    public class StoreEntityBase : IStoreEntity
    {
        private Guid _instanceid = Guid.Empty;

        public Guid InstanceId
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if (_instanceid == Guid.Empty)
                    _instanceid = Guid.NewGuid();
                return _instanceid;
            }
        }
    }
}
