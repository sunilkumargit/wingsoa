namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;

    [AttributeUsage(AttributeTargets.All)]
    public sealed class SRCategoryAttribute : CategoryAttribute
    {
        public SRCategoryAttribute(string category) : base(category)
        {
        }

        protected override string GetLocalizedString(string value)
        {
            return Telerik.Windows.Controls.SR.GetString(value);
        }
    }
}

