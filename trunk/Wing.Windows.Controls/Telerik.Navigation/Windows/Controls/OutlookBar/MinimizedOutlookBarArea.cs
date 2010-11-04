namespace Telerik.Windows.Controls.OutlookBar
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using Telerik.Windows.Controls;

    [StyleTypedProperty(Property="ItemContainerStyle", StyleTargetType=typeof(MinimizedOutlookBarItem))]
    public class MinimizedOutlookBarArea : System.Windows.Controls.ListBox
    {
        private Telerik.Windows.Controls.OutlookBar.OutlookBarPanel outlookBarPanel;
        private WeakReference parentReference = new WeakReference(null);

        public MinimizedOutlookBarArea()
        {
            base.DefaultStyleKey = typeof(MinimizedOutlookBarArea);
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            ContentControl control = element as ContentControl;
            if (control != null)
            {
                control.Content = null;
                control.ClearValue(Control.IsEnabledProperty);
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MinimizedOutlookBarItem { ParentOutlookBar = this.ParentOutlookBar };
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is MinimizedOutlookBarItem);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (this.OutlookBarPanel == null)
            {
                this.outlookBarPanel = VisualTreeHelper.GetParent(element) as Telerik.Windows.Controls.OutlookBar.OutlookBarPanel;
            }
        }

        internal Telerik.Windows.Controls.OutlookBar.OutlookBarPanel OutlookBarPanel
        {
            get
            {
                return this.outlookBarPanel;
            }
        }

        internal RadOutlookBar ParentOutlookBar
        {
            get
            {
                return (this.parentReference.Target as RadOutlookBar);
            }
            set
            {
                this.parentReference = new WeakReference(value);
            }
        }
    }
}

