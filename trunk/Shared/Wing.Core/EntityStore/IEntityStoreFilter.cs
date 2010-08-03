using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.EntityStore
{
    public interface IEntityStoreFilter
    {
        String PropertyName { get; set; }
        ComparisonType Comparison { get; set; }
        Object Value { get; set; }
    }
}
