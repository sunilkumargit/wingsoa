using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.EntityStore
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=false)]
    public class PersistentMemberAttribute : Attribute
    {
        public int MaxLength { get; private set; }

        public PersistentMemberAttribute() { }
        public PersistentMemberAttribute(int maxLength)
        {
            MaxLength = maxLength;
        }
    }
}
