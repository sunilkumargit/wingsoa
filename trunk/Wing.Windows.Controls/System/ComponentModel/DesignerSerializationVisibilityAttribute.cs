namespace System.ComponentModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class DesignerSerializationVisibilityAttribute : Attribute
    {
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly DesignerSerializationVisibilityAttribute Content = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content);
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly DesignerSerializationVisibilityAttribute Default = Visible;
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly DesignerSerializationVisibilityAttribute Hidden = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden);
        private DesignerSerializationVisibility visibility;
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly DesignerSerializationVisibilityAttribute Visible = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible);

        public DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility visibility)
        {
            this.visibility = visibility;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            DesignerSerializationVisibilityAttribute attribute = obj as DesignerSerializationVisibilityAttribute;
            return ((attribute != null) && (attribute.Visibility == this.visibility));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool IsDefaultAttribute()
        {
            return this.Equals(Default);
        }

        public DesignerSerializationVisibility Visibility
        {
            get
            {
                return this.visibility;
            }
        }
    }
}

