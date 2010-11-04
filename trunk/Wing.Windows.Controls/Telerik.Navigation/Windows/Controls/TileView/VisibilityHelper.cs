namespace Telerik.Windows.Controls.TileView
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;

    public class VisibilityHelper : FrameworkElement
    {
        public static readonly DependencyProperty ContainerVisibilityProperty = DependencyProperty.Register("ContainerVisibility", typeof(Visibility), typeof(VisibilityHelper), new PropertyMetadata(new PropertyChangedCallback(VisibilityHelper.OnContainerVisibilityPropertyChanged)));

        protected virtual void OnContainerVisibilityChanged(Visibility oldValue, Visibility newValue)
        {
            if (this.ContainerVisibilityChangeCallback != null)
            {
                this.ContainerVisibilityChangeCallback();
            }
        }

        private static void OnContainerVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VisibilityHelper source = d as VisibilityHelper;
            Visibility newValue = (Visibility) e.NewValue;
            Visibility oldValue = (Visibility) e.OldValue;
            if (source != null)
            {
                source.OnContainerVisibilityChanged(oldValue, newValue);
            }
        }

        public Visibility ContainerVisibility
        {
            get
            {
                return (Visibility) base.GetValue(ContainerVisibilityProperty);
            }
            set
            {
                base.SetValue(ContainerVisibilityProperty, value);
            }
        }

        public Action ContainerVisibilityChangeCallback { get; set; }
    }
}

