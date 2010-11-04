namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Telerik.Windows.Controls.Primitives;

    public class RadCoverFlowItem : System.Windows.Controls.ListBoxItem
    {
        private Orientation childOrientation = Orientation.Horizontal;
        public static readonly DependencyProperty IsContentValidProperty = DependencyProperty.Register("IsContentValid", typeof(bool), typeof(RadCoverFlowItem), new PropertyMetadata(true, new PropertyChangedCallback(RadCoverFlowItem.OnIsContentValidChanged)));
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(RadCoverFlowItem), new PropertyMetadata(new PropertyChangedCallback(RadCoverFlowItem.OnIsLoadingChanged)));
        private Orientation parentOrientation = Orientation.Horizontal;

        public RadCoverFlowItem()
        {
            base.DefaultStyleKey = typeof(RadCoverFlowItem);
        }

        internal void ChangeOrientation(Orientation newVal)
        {
            if (this.LayoutTransform != null)
            {
                if (newVal == Orientation.Horizontal)
                {
                    this.LayoutTransform.LayoutTransform = null;
                }
                else
                {
                    this.LayoutTransform.LayoutTransform = new RotateTransform { Angle = -90.0 };
                }
                this.childOrientation = this.parentOrientation = newVal;
            }
            else
            {
                this.parentOrientation = newVal;
            }
        }

        private void ChangeVisualState(bool useTransitions)
        {
            if (this.IsLoading)
            {
                VisualStateManager.GoToState(this, "Loading", useTransitions);
            }
            else if (!this.IsContentValid)
            {
                VisualStateManager.GoToState(this, "InvalidContent", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Loaded", useTransitions);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.LayoutTransform = (LayoutTransformControl) base.GetTemplateChild("layoutTransform");
            if (this.parentOrientation != this.childOrientation)
            {
                this.ChangeOrientation(this.parentOrientation);
            }
            this.ChangeVisualState(false);
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            DependencyObject element = newContent as DependencyObject;
            if (element != null)
            {
                RadCoverFlow.SetEnableLoadNotification(element, true);
            }
            element = oldContent as DependencyObject;
            if (element != null)
            {
                RadCoverFlow.SetEnableLoadNotification(element, false);
            }
            base.OnContentChanged(oldContent, newContent);
        }

        private static void OnIsContentValidChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as RadCoverFlowItem).IsLoading = false;
        }

        private static void OnIsLoadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as RadCoverFlowItem).ChangeVisualState(true);
        }

        public bool IsContentValid
        {
            get
            {
                return (bool) base.GetValue(IsContentValidProperty);
            }
            set
            {
                base.SetValue(IsContentValidProperty, value);
            }
        }

        public bool IsLoading
        {
            get
            {
                return (bool) base.GetValue(IsLoadingProperty);
            }
            set
            {
                base.SetValue(IsLoadingProperty, value);
            }
        }

        internal LayoutTransformControl LayoutTransform { get; set; }

        internal System.Windows.Media.Animation.Storyboard Storyboard { get; set; }
    }
}

