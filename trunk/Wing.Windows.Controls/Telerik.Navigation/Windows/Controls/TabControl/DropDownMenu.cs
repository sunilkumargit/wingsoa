namespace Telerik.Windows.Controls.TabControl
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Telerik.Windows;
    using Telerik.Windows.Controls;

    public class DropDownMenu : RadContextMenu
    {
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private FrameworkElement borderMaskElement;
        private WeakReference parentTabReference = new WeakReference(null);

        protected internal override void ChangeVisualState(bool useTransitions)
        {
            base.ChangeVisualState(useTransitions);
            Control placementTarget = Telerik.Windows.RoutedEvent.GetLogicalParent(this) as Control;
            if ((placementTarget != null) && (this.borderMaskElement != null))
            {
                base.Dispatcher.BeginInvoke(delegate
                {
                    this.borderMaskElement.Height = this.BorderThickness.Top;
                    this.borderMaskElement.Width = Math.Max((double)0.0, (double)((placementTarget.ActualWidth - placementTarget.BorderThickness.Left) - placementTarget.BorderThickness.Right));
                    double maskHorizontalOffset = 0.0;
                    if (this.IsOpen)
                    {
                        try
                        {
                            maskHorizontalOffset = placementTarget.TransformToVisual(this).Transform(new Point(0.0, placementTarget.ActualHeight)).X;
                        }
                        catch (ArgumentException)
                        {
                        }
                    }
                    TranslateTransform translateTransform = this.borderMaskElement.RenderTransform as TranslateTransform;
                    if (translateTransform != null)
                    {
                        return;
                    }
                    translateTransform = new TranslateTransform();
                    translateTransform.X = maskHorizontalOffset + placementTarget.BorderThickness.Left;
                    this.borderMaskElement.RenderTransform = translateTransform;
                });
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            RadMenuItem menuItem = element as RadMenuItem;
            if (menuItem != null)
            {
                menuItem.Header = null;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DropDownMenuItem();
        }

        public override void OnApplyTemplate()
        {
            this.borderMaskElement = base.GetTemplateChild("BorderMaskElement") as FrameworkElement;
            base.OnApplyTemplate();
            this.ChangeVisualState(true);
        }

        protected override void OnOpened(RadRoutedEventArgs e)
        {
            base.OnOpened(e);
            this.ChangeVisualState(true);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (this.ParentTabControl != null)
            {
                this.ParentTabControl.PrepareContainerForDropDownItem(element as RadMenuItem, item);
            }
            base.PrepareContainerForItemOverride(element, item);
        }

        internal RadTabControl ParentTabControl
        {
            get
            {
                return (this.parentTabReference.Target as RadTabControl);
            }
            set
            {
                this.parentTabReference = new WeakReference(value);
            }
        }
    }
}

