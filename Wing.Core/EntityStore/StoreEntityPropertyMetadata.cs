using System;

namespace Wing.EntityStore
{
    public class StoreEntityPropertyMetadata
    {
        public String PropertyName { get; internal set; }
        public Type PropertyType { get; internal set; }
        public int MaxLength { get; internal set; }
    }
}
