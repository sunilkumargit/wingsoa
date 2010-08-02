using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.EntityStore
{
    public class StoreEntityPropertyMetadata
    {
        public String PropertyName { get; internal set; }
        public Type PropertyType { get; internal set; }
        public int MaxLength { get; internal set; }
    }
}
