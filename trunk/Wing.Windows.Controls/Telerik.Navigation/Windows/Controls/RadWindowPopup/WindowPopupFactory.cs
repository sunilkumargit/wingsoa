namespace Telerik.Windows.Controls.RadWindowPopup
{
    using System;
    using System.Windows;

    internal abstract class WindowPopupFactory
    {
        protected WindowPopupFactory()
        {
        }

        protected abstract WindowPopup CreatePopup(UIElement content);
        public WindowPopup GetPopup(UIElement child, UIElement modalCanvas, bool isTopMost)
        {
            WindowPopup popup = this.CreatePopup(child);
            popup.Configure(child, modalCanvas, isTopMost);
            return popup;
        }
    }
}

