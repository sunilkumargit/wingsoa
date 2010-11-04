namespace Telerik.Windows.Controls
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public sealed class ThemeLocationAttribute : Attribute
    {
        private ThemeLocation location;

        public ThemeLocationAttribute(ThemeLocation location)
        {
            this.location = location;
        }

        public ThemeLocation Location
        {
            get
            {
                return this.location;
            }
        }
    }
}

