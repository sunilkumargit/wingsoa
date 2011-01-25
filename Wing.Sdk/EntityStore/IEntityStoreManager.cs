using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.EntityStore
{
    public interface IEntityStoreManager
    {
        IEntityStore CreateStore(String name);
        IEntityStore GetStore(String name);
    }
}
