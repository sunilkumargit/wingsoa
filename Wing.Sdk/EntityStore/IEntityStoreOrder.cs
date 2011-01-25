using System;

namespace Wing.EntityStore
{
    public interface IEntityStoreOrder
    {
        String PropertyName { get; set; }
        bool Desc { get; set; }
    }
}
