using System;

namespace Wing.EntityStore
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
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
