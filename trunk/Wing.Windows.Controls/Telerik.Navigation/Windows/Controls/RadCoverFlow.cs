namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Telerik.Windows.Controls.Primitives;

    public class RadCoverFlow : System.Windows.Controls.ListBox
    {
        public static readonly DependencyProperty CameraDistanceProperty = DependencyProperty.Register("CameraDistance", typeof(double), typeof(RadCoverFlow), new PropertyMetadata(1000.0, new PropertyChangedCallback(RadCoverFlow.InvalidateItemsHostMeasure)));
        public static readonly DependencyProperty CameraRotationProperty = DependencyProperty.Register("CameraRotation", typeof(double), typeof(RadCoverFlow), new PropertyMetadata(0.0, new PropertyChangedCallback(RadCoverFlow.InvalidateItemsHostMeasure)));
        public static readonly DependencyProperty CameraViewpointProperty = DependencyProperty.Register("CameraViewpoint", typeof(Telerik.Windows.Controls.CameraViewpoint), typeof(RadCoverFlow), new PropertyMetadata(Telerik.Windows.Controls.CameraViewpoint.Bottom, new PropertyChangedCallback(RadCoverFlow.InvalidateItemsHostMeasure)));
        public static readonly DependencyProperty DistanceBetweenItemsProperty = DependencyProperty.Register("DistanceBetweenItems", typeof(double), typeof(RadCoverFlow), new PropertyMetadata(100.0, new PropertyChangedCallback(RadCoverFlow.InvalidateItemsHostArrange)));
        public static readonly DependencyProperty DistanceFromSelectedItemProperty = DependencyProperty.Register("DistanceFromSelectedItem", typeof(double), typeof(RadCoverFlow), new PropertyMetadata(15.0, new PropertyChangedCallback(RadCoverFlow.InvalidateItemsHostMeasure)));
        public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(RadCoverFlow), new PropertyMetadata(new CircleEase { EasingMode=EasingMode.EaseOut }));
        public static readonly DependencyProperty EnableLoadNotificationProperty = DependencyProperty.RegisterAttached("EnableLoadNotification", typeof(bool), typeof(RadCoverFlow), new PropertyMetadata(new PropertyChangedCallback(RadCoverFlow.OnEnableLoadNotificationChanged)));
        public static readonly DependencyProperty IsReflectionEnabledProperty = DependencyProperty.Register("IsReflectionEnabled", typeof(bool), typeof(RadCoverFlow), new PropertyMetadata(true, new PropertyChangedCallback(RadCoverFlow.InvalidateItemsHostMeasure)));
        public static readonly DependencyProperty ItemChangeDelayProperty = DependencyProperty.Register("ItemChangeDelay", typeof(TimeSpan), typeof(RadCoverFlow), new PropertyMetadata(TimeSpan.FromSeconds(0.6)));
        public static readonly DependencyProperty ItemScaleProperty = DependencyProperty.Register("ItemScale", typeof(double), typeof(RadCoverFlow), new PropertyMetadata(1.0, new PropertyChangedCallback(RadCoverFlow.InvalidateItemsHostMeasure)));
        private CoverFlowPanel itemsHost;
        public static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register("OffsetX", typeof(double), typeof(RadCoverFlow), new PropertyMetadata(0.0, new PropertyChangedCallback(RadCoverFlow.InvalidateItemsHostMeasure)));
        public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register("OffsetY", typeof(double), typeof(RadCoverFlow), new PropertyMetadata(0.0, new PropertyChangedCallback(RadCoverFlow.InvalidateItemsHostMeasure)));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(System.Windows.Controls.Orientation), typeof(RadCoverFlow), new PropertyMetadata(System.Windows.Controls.Orientation.Horizontal, new PropertyChangedCallback(RadCoverFlow.OnOrientationChanged)));
        public static readonly DependencyProperty ReflectionHeightProperty = DependencyProperty.Register("ReflectionHeight", typeof(double), typeof(RadCoverFlow), new PropertyMetadata(0.5, new PropertyChangedCallback(RadCoverFlow.InvalidateItemsHostArrange)));
        public static readonly DependencyProperty ReflectionOpacityProperty = DependencyProperty.Register("ReflectionOpacity", typeof(double), typeof(RadCoverFlow), new PropertyMetadata(1.0, new PropertyChangedCallback(RadCoverFlow.InvalidateItemsHostArrange)));
        public static readonly DependencyProperty RotationYProperty = DependencyProperty.Register("RotationY", typeof(double), typeof(RadCoverFlow), new PropertyMetadata(60.0, new PropertyChangedCallback(RadCoverFlow.InvalidateItemsHostArrange)));

        public RadCoverFlow()
        {
            base.DefaultStyleKey = typeof(RadCoverFlow);
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            RadCoverFlowItem coverFlowItem = element as RadCoverFlowItem;
            coverFlowItem.ClearValue(Rad3D.CameraDistanceProperty);
            coverFlowItem.ClearValue(Rad3D.CameraRotationProperty);
            coverFlowItem.ClearValue(CoverFlowPanel.LastPositionProperty);
            coverFlowItem.ClearValue(UIElement.RenderTransformProperty);
            base.ClearContainerForItemOverride(element, item);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RadCoverFlowItem();
        }

        public static bool GetEnableLoadNotification(DependencyObject obj)
        {
            return (bool) obj.GetValue(EnableLoadNotificationProperty);
        }

        private static void InvalidateItemsHostArrange(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            CoverFlowPanel panel = (d as RadCoverFlow).ItemsHost;
            if (panel != null)
            {
                panel.InvalidateArrange();
            }
        }

        private static void InvalidateItemsHostMeasure(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            CoverFlowPanel panel = (d as RadCoverFlow).ItemsHost;
            if (panel != null)
            {
                panel.InvalidateMeasure();
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is RadCoverFlowItem);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.LayoutTransform = base.GetTemplateChild("layoutTransform") as LayoutTransformControl;
            this.UpdateOrientation(this.Orientation);
        }

        private static void OnEnableLoadNotificationChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if ((bool) args.NewValue)
            {
                LoadNotificationBehavior behavior = new LoadNotificationBehavior(d);
                if (behavior.IsValid)
                {
                    d.SetValue(LoadNotificationBehavior.InstanceProperty, behavior);
                }
            }
            else
            {
                d.ClearValue(LoadNotificationBehavior.InstanceProperty);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Key.Left:
                    if (base.SelectedIndex <= 0)
                    {
                        break;
                    }
                    base.SelectedIndex--;
                    return;

                case Key.Up:
                    break;

                case Key.Right:
                    if (base.SelectedIndex < (base.Items.Count - 1))
                    {
                        base.SelectedIndex++;
                    }
                    break;

                default:
                    return;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                if ((base.SelectedIndex + 1) < base.Items.Count)
                {
                    base.SelectedIndex++;
                }
            }
            else if (base.SelectedIndex > 0)
            {
                base.SelectedIndex--;
            }
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RadCoverFlow) d).UpdateOrientation((System.Windows.Controls.Orientation) e.NewValue);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            RadCoverFlowItem coverFlowItem = element as RadCoverFlowItem;
            coverFlowItem.SetBinding(Rad3D.CameraDistanceProperty, new Binding("CameraDistance") { Source = this });
            coverFlowItem.SetBinding(Rad3D.CameraRotationProperty, new Binding("CameraRotation") { Source = this });
            coverFlowItem.ChangeOrientation(this.Orientation);
            base.PrepareContainerForItemOverride(element, item);
        }

        public static void SetEnableLoadNotification(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableLoadNotificationProperty, value);
        }

        private void UpdateOrientation(System.Windows.Controls.Orientation val)
        {
            if (this.LayoutTransform != null)
            {
                if (val == System.Windows.Controls.Orientation.Horizontal)
                {
                    this.LayoutTransform.LayoutTransform = null;
                }
                else
                {
                    this.LayoutTransform.LayoutTransform = new RotateTransform { Angle = 90.0 };
                }
                if (this.ItemsHost != null)
                {
                    for (int i = 0; i < this.ItemsHost.RealizedChildren.Count; i++)
                    {
                        RadCoverFlowItem item = this.ItemsHost.RealizedChildren[i] as RadCoverFlowItem;
                        if (item == null)
                        {
                            break;
                        }
                        item.ChangeOrientation(val);
                    }
                }
            }
        }

        public double CameraDistance
        {
            get
            {
                return (double) base.GetValue(CameraDistanceProperty);
            }
            set
            {
                base.SetValue(CameraDistanceProperty, value);
            }
        }

        public double CameraRotation
        {
            get
            {
                return (double) base.GetValue(CameraRotationProperty);
            }
            set
            {
                base.SetValue(CameraRotationProperty, value);
            }
        }

        public Telerik.Windows.Controls.CameraViewpoint CameraViewpoint
        {
            get
            {
                return (Telerik.Windows.Controls.CameraViewpoint) base.GetValue(CameraViewpointProperty);
            }
            set
            {
                base.SetValue(CameraViewpointProperty, value);
            }
        }

        public double DistanceBetweenItems
        {
            get
            {
                return (double) base.GetValue(DistanceBetweenItemsProperty);
            }
            set
            {
                base.SetValue(DistanceBetweenItemsProperty, value);
            }
        }

        public double DistanceFromSelectedItem
        {
            get
            {
                return (double) base.GetValue(DistanceFromSelectedItemProperty);
            }
            set
            {
                base.SetValue(DistanceFromSelectedItemProperty, value);
            }
        }

        public IEasingFunction EasingFunction
        {
            get
            {
                return (IEasingFunction) base.GetValue(EasingFunctionProperty);
            }
            set
            {
                base.SetValue(EasingFunctionProperty, value);
            }
        }

        public bool IsReflectionEnabled
        {
            get
            {
                return (bool) base.GetValue(IsReflectionEnabledProperty);
            }
            set
            {
                base.SetValue(IsReflectionEnabledProperty, value);
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Virtualizing", Justification="The spelling is correct.")]
        public bool IsVirtualizing
        {
            get
            {
                return CoverFlowPanel.GetIsVirtualizing(this);
            }
            set
            {
                CoverFlowPanel.SetIsVirtualizing(this, value);
            }
        }

        [DefaultValue(0)]
        public TimeSpan ItemChangeDelay
        {
            get
            {
                return (TimeSpan) base.GetValue(ItemChangeDelayProperty);
            }
            set
            {
                base.SetValue(ItemChangeDelayProperty, value);
            }
        }

        public double ItemScale
        {
            get
            {
                return (double) base.GetValue(ItemScaleProperty);
            }
            set
            {
                base.SetValue(ItemScaleProperty, value);
            }
        }

        internal CoverFlowPanel ItemsHost
        {
            get
            {
                if (this.itemsHost == null)
                {
                    IList<CoverFlowPanel> panels = this.ChildrenOfType<CoverFlowPanel>();
                    if ((panels != null) && (panels.Count > 0))
                    {
                        this.itemsHost = panels[0];
                    }
                }
                return this.itemsHost;
            }
        }

        internal LayoutTransformControl LayoutTransform { get; set; }

        public double OffsetX
        {
            get
            {
                return (double) base.GetValue(OffsetXProperty);
            }
            set
            {
                base.SetValue(OffsetXProperty, value);
            }
        }

        public double OffsetY
        {
            get
            {
                return (double) base.GetValue(OffsetYProperty);
            }
            set
            {
                base.SetValue(OffsetYProperty, value);
            }
        }

        public System.Windows.Controls.Orientation Orientation
        {
            get
            {
                return (System.Windows.Controls.Orientation) base.GetValue(OrientationProperty);
            }
            set
            {
                base.SetValue(OrientationProperty, value);
            }
        }

        public double ReflectionHeight
        {
            get
            {
                return (double) base.GetValue(ReflectionHeightProperty);
            }
            set
            {
                base.SetValue(ReflectionHeightProperty, value);
            }
        }

        public double ReflectionOpacity
        {
            get
            {
                return (double) base.GetValue(ReflectionOpacityProperty);
            }
            set
            {
                base.SetValue(ReflectionOpacityProperty, value);
            }
        }

        public double RotationY
        {
            get
            {
                return (double) base.GetValue(RotationYProperty);
            }
            set
            {
                base.SetValue(RotationYProperty, value);
            }
        }
    }
}

