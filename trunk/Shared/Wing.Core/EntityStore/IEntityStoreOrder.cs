using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.EntityStore
{
    public interface IEntityStoreOrder
    {
        String PropertyName { get; set; }
        bool Desc { get; set; }
    }
}
