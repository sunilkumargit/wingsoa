namespace Telerik.Windows.Controls
{
    using System;

    [Obsolete("Please, use RadMenuItem and set its IsSeparator property to true", true)]
    public class RadSeparator : RadMenuItem
    {
        public RadSeparator()
        {
            base.IsSeparator = true;
        }
    }
}

