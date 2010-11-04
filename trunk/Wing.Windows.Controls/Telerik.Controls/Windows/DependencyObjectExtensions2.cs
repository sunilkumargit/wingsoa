namespace Telerik.Windows
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Windows;

    public static class DependencyObjectExtensions
    {
        internal static readonly DependencyProperty PropertyChangesAllowedProperty = DependencyProperty.RegisterAttached("PropertyChangesAllowed", typeof(bool), typeof(DependencyObjectExtensions), null);

        public static void AddHandler(this DependencyObject element, Telerik.Windows.RoutedEvent routedEvent, Delegate handler)
        {
            RadRoutedEventHelper.AddHandler(element, routedEvent, handler, false);
        }

        public static void AddHandler(this DependencyObject element, Telerik.Windows.RoutedEvent routedEvent, Delegate handler, bool handlesToo)
        {
            RadRoutedEventHelper.AddHandler(element, routedEvent, handler, handlesToo);
        }

        public static void ClearValue(this DependencyObject element, DependencyPropertyKey key)
        {
            element.SetValue(PropertyChangesAllowedProperty, true);
            element.ClearValue(key.DependencyProperty);
            element.SetValue(PropertyChangesAllowedProperty, false);
        }

        internal static DependencyObjectType GetDependencyObjectType(this DependencyObject element)
        {
            return (element.GetValue(RadRoutedEventHelper.DependencyObjectTypeProperty) as DependencyObjectType);
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification="WPF code port")]
        public static void RaiseEvent(this DependencyObject element, RadRoutedEventArgs e)
        {
            RadRoutedEventHelper.RaiseEvent(element, e);
        }

        public static void RemoveHandler(this DependencyObject element, Telerik.Windows.RoutedEvent routedEvent, Delegate handler)
        {
            RadRoutedEventHelper.RemoveHandler(element, routedEvent, handler);
        }

        internal static void SetDependencyObjectType(this DependencyObject element, DependencyObjectType type)
        {
            element.SetValue(RadRoutedEventHelper.DependencyObjectTypeProperty, type);
        }

        public static void SetValue(this DependencyObject element, DependencyPropertyKey key, object value)
        {
            element.SetValue(PropertyChangesAllowedProperty, true);
            element.SetValue(key.DependencyProperty, value);
            element.SetValue(PropertyChangesAllowedProperty, false);
        }

        internal static void SetValueFromCoerce(this DependencyObject element, DependencyProperty dp, object coercedValue)
        {
            element.SetValue(Telerik.Windows.PropertyMetadata.CanCoerceReadOnlyPropertyProperty, true);
            element.SetValue(dp, coercedValue);
            element.SetValue(Telerik.Windows.PropertyMetadata.CanCoerceReadOnlyPropertyProperty, false);
        }
    }
}

