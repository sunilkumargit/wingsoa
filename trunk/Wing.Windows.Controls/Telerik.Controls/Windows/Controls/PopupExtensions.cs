namespace Telerik.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    [SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors")]
    public class PopupExtensions
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.RegisterAttached("IsOpen", typeof(bool), typeof(PopupExtensions), new PropertyMetadata(new PropertyChangedCallback(PopupExtensions.OnIsOpenChanged)));
        public static readonly DependencyProperty OwnerProperty = DependencyProperty.RegisterAttached("Owner", typeof(FrameworkElement), typeof(PopupExtensions), new PropertyMetadata(new PropertyChangedCallback(PopupExtensions.OnOwnerChanged)));
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.RegisterAttached("Placement", typeof(Telerik.Windows.Controls.PlacementMode), typeof(PopupExtensions), new PropertyMetadata(new PropertyChangedCallback(PopupExtensions.OnPlacementChanged)));
        public static readonly DependencyProperty PlacementRectangleProperty = DependencyProperty.RegisterAttached("PlacementRectangle", typeof(Rect), typeof(PopupExtensions), new PropertyMetadata(Rect.Empty, new PropertyChangedCallback(PopupExtensions.OnPlacementRectangleChanged)));
        public static readonly DependencyProperty PlacementTargetProperty = DependencyProperty.RegisterAttached("PlacementTarget", typeof(UIElement), typeof(PopupExtensions), new PropertyMetadata(new PropertyChangedCallback(PopupExtensions.OnPlacementTargetChanged)));
        public static readonly DependencyProperty StaysOpenProperty = DependencyProperty.RegisterAttached("StaysOpen", typeof(bool), typeof(PopupExtensions), new PropertyMetadata(new PropertyChangedCallback(PopupExtensions.OnStaysOpenChanged)));
        private static readonly DependencyProperty WrapperProperty = DependencyProperty.RegisterAttached("Wrapper", typeof(PopupWrapper), typeof(PopupExtensions), null);

        private static void AdjustPopupLocation(Popup popup)
        {
            PopupWrapper wrapper = GetWrapper(popup);
            wrapper.CatchClickOutsidePopup = wrapper.CloseOnOutsideClick = !GetStaysOpen(popup);
            wrapper.PlacementRectangle = GetPlacementRectangle(popup);
            wrapper.PlacementTarget = GetPlacementTarget(popup);
            wrapper.Placement = GetPlacement(popup);
            wrapper.PopupOwner = GetOwner(popup);
            wrapper.AdjustPopupLocation();
        }

        public static bool GetIsOpen(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsOpenProperty);
        }

        public static FrameworkElement GetOwner(DependencyObject obj)
        {
            return (FrameworkElement) obj.GetValue(OwnerProperty);
        }

        public static Telerik.Windows.Controls.PlacementMode GetPlacement(DependencyObject obj)
        {
            return (Telerik.Windows.Controls.PlacementMode) obj.GetValue(PlacementProperty);
        }

        public static Rect GetPlacementRectangle(DependencyObject obj)
        {
            return (Rect) obj.GetValue(PlacementRectangleProperty);
        }

        public static UIElement GetPlacementTarget(DependencyObject obj)
        {
            return (UIElement) obj.GetValue(PlacementTargetProperty);
        }

        public static bool GetStaysOpen(DependencyObject obj)
        {
            return (bool) obj.GetValue(StaysOpenProperty);
        }

        private static PopupWrapper GetWrapper(DependencyObject obj)
        {
            return (PopupWrapper) obj.GetValue(WrapperProperty);
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Popup popup = d as Popup;
            if (popup != null)
            {
                ShowHidePopup(popup, (bool) e.NewValue);
            }
        }

        private static void OnOwnerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateWrapper(d as Popup);
        }

        private static void OnPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateWrapper(d as Popup);
        }

        private static void OnPlacementRectangleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateWrapper(d as Popup);
        }

        private static void OnPlacementTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateWrapper(d as Popup);
        }

        private static void OnPopupClosed(object sender, EventArgs e)
        {
            Popup popup = sender as Popup;
            SetIsOpen(popup, false);
        }

        private static void OnPopupOpened(object sender, EventArgs e)
        {
            Popup popup = sender as Popup;
            AdjustPopupLocation(popup);
            SetIsOpen(popup, true);
        }

        private static void OnStaysOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateWrapper(d as Popup);
        }

        public static void SetIsOpen(DependencyObject obj, bool value)
        {
            obj.SetValue(IsOpenProperty, value);
        }

        public static void SetOwner(DependencyObject obj, UIElement value)
        {
            obj.SetValue(OwnerProperty, value);
        }

        public static void SetPlacement(DependencyObject obj, Telerik.Windows.Controls.PlacementMode value)
        {
            obj.SetValue(PlacementProperty, value);
        }

        public static void SetPlacementRectangle(DependencyObject obj, Rect value)
        {
            obj.SetValue(PlacementRectangleProperty, value);
        }

        public static void SetPlacementTarget(DependencyObject obj, UIElement value)
        {
            obj.SetValue(PlacementTargetProperty, value);
        }

        public static void SetStaysOpen(DependencyObject obj, bool value)
        {
            obj.SetValue(StaysOpenProperty, value);
        }

        private static void SetWrapper(DependencyObject obj, PopupWrapper value)
        {
            obj.SetValue(WrapperProperty, value);
        }

        private static void ShowHidePopup(Popup popup, bool isShow)
        {
            UpdateWrapper(popup);
            PopupWrapper wrapper = GetWrapper(popup);
            if (isShow)
            {
                if (popup.IsOpen)
                {
                    AdjustPopupLocation(popup);
                }
                else
                {
                    wrapper.ShowPopup();
                }
            }
            else if (popup.IsOpen)
            {
                wrapper.HidePopup();
            }
        }

        private static void UpdateWrapper(Popup popup)
        {
            if (popup != null)
            {
                if (GetWrapper(popup) == null)
                {
                    PopupWrapper wrapper = new PopupWrapper(popup, GetOwner(popup));
                    SetWrapper(popup, wrapper);
                    popup.Opened += new EventHandler(PopupExtensions.OnPopupOpened);
                    popup.Closed += new EventHandler(PopupExtensions.OnPopupClosed);
                }
                if (popup.IsOpen)
                {
                    AdjustPopupLocation(popup);
                }
            }
        }
    }
}

