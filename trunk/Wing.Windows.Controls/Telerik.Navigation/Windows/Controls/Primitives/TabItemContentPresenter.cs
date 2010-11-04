namespace Telerik.Windows.Controls.Primitives
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Telerik.Windows.Controls;

    public class TabItemContentPresenter : ContentControl
    {
        private WeakReference ownerReference;

        public TabItemContentPresenter()
        {
            base.DefaultStyleKey = typeof(ContentControl);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size result;
            base.RenderTransformOrigin = new Point(0.0, 0.0);
            TransformGroup renderTransform = new TransformGroup();
            if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                result = base.ArrangeOverride(finalSize.Swap()).Swap();
            }
            else
            {
                result = base.ArrangeOverride(finalSize);
            }
            if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                renderTransform.Children.Add(new RotateTransform { Angle = -90.0, CenterY = 0.0, CenterX = 0.0 });
                renderTransform.Children.Add(new TranslateTransform { Y = result.Height });
            }
            base.RenderTransform = renderTransform;
            return result;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                return base.MeasureOverride(availableSize.Swap()).Swap();
            }
            return base.MeasureOverride(availableSize);
        }

        private bool HasOwner
        {
            get
            {
                return (this.Owner != null);
            }
        }

        private System.Windows.Controls.Orientation Orientation
        {
            get
            {
                if (!this.HasOwner)
                {
                    return System.Windows.Controls.Orientation.Horizontal;
                }
                return this.Owner.TabOrientation;
            }
        }

        private RadTabControl Owner
        {
            get
            {
                if (this.TabItemOwner == null)
                {
                    return null;
                }
                return this.TabItemOwner.Owner;
            }
        }

        internal RadTabItem TabItemOwner
        {
            get
            {
                if (this.ownerReference != null)
                {
                    return (this.ownerReference.Target as RadTabItem);
                }
                return null;
            }
            set
            {
                this.ownerReference = new WeakReference(value);
            }
        }
    }
}

