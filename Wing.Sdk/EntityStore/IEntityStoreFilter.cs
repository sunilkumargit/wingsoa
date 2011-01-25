using System;

namespace Wing.EntityStore
{
    public interface IEntityStoreFilter
    {
        String PropertyName { get; set; }
        ComparisonType Comparison { get; set; }
        Object Value { get; set; }
    }
}
