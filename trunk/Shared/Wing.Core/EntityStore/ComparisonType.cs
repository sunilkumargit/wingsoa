using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.EntityStore
{
    public enum ComparisonType
    {
        Equals,
        EqualOrGreater,
        EqualOrLess,
        LessThan,
        GreaterThan,
        NotEqual,
        IsNull,
        IsNotNull
    }
}
