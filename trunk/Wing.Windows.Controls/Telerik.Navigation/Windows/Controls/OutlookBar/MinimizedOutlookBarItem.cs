namespace Telerik.Windows.Controls.OutlookBar
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Telerik.Windows.Controls;

    public class MinimizedOutlookBarItem : System.Windows.Controls.ListBoxItem, IOutlookBarItem
    {
        private OutlookBarItemPosition location = OutlookBarItemPosition.ActiveArea;
        private WeakReference parentReference = new WeakReference(null);
        public static readonly DependencyProperty SmallIconProperty = DependencyProperty.Register("SmallIcon", typeof(ImageSource), typeof(MinimizedOutlookBarItem), null);

        public MinimizedOutlookBarItem()
        {
            base.DefaultStyleKey = typeof(MinimizedOutlookBarItem);
        }

        internal virtual void ChangeVisualState()
        {
            this.ChangeVisualState(false);
        }

        internal virtual void ChangeVisualState(bool useTransitions)
        {
            if (!base.IsEnabled)
            {
                VisualStateManager.GoToState(this, "Unselected", false);
            }
        }

        private void MinimizedOutlookBarItem_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.ChangeVisualState();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            base.IsEnabledChanged -= new DependencyPropertyChangedEventHandler(this.MinimizedOutlookBarItem_IsEnabledChanged);
            base.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.MinimizedOutlookBarItem_IsEnabledChanged);
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

        public ImageSource SmallIcon
        {
            get
            {
                return (ImageSource) base.GetValue(SmallIconProperty);
            }
            set
            {
                base.SetValue(SmallIconProperty, value);
            }
        }

        OutlookBarItemPosition IOutlookBarItem.Location
        {
            get
            {
                return this.location;
            }
            set
            {
                if ((this.ParentOutlookBar != null) && this.ParentOutlookBar.MinimizedItemsOutlookBarItems.ContainsKey(this))
                {
                    IOutlookBarItem outlookBarItem = this.ParentOutlookBar.MinimizedItemsOutlookBarItems[this];
                    this.location = outlookBarItem.Location;
                    outlookBarItem.Location = value;
                }
            }
        }
    }
}

