namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;

    [AttributeUsage(AttributeTargets.All)]
    public sealed class SRDescriptionAttribute : DescriptionAttribute
    {
        private bool replaced;

        public SRDescriptionAttribute(string description) : base(description)
        {
        }

        public override string Description
        {
            get
            {
                if (!this.replaced)
                {
                    this.replaced = true;
                    base.DescriptionValue = Telerik.Windows.Controls.SR.GetString(base.Description);
                }
                return base.Description;
            }
        }
    }
}

