namespace Telerik.Windows.Controls.ToolBar
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using Telerik.Windows.Controls;

    public class RadToolBarTrayPanel : Panel
    {
        public RadToolBarTrayPanel()
        {
            this.HostTray = null;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size returnSize = base.ArrangeOverride(finalSize);
            if (this.HostTray != null)
            {
                returnSize = this.HostTray.ArrangeToolBars(finalSize);
            }
            return returnSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size returnSize = base.MeasureOverride(availableSize);
            if (this.HostTray != null)
            {
                returnSize = this.HostTray.MeasureToolBars(availableSize);
            }
            return returnSize;
        }

        internal RadToolBarTray HostTray { get; set; }
    }
}

