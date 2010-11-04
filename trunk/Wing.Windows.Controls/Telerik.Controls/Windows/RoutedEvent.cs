namespace Telerik.Windows
{
    using System;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Windows;

    public sealed class RoutedEvent
    {
        public static readonly DependencyProperty LogicalParentProperty = DependencyProperty.RegisterAttached("LogicalParent", typeof(WeakReference), typeof(Telerik.Windows.RoutedEvent), null);

        internal RoutedEvent(string name, Telerik.Windows.RoutingStrategy routingStrategy, Type handlerType, Type ownerType)
        {
            this.Name = name;
            this.RoutingStrategy = routingStrategy;
            this.HandlerType = handlerType;
            this.OwnerType = ownerType;
            this.GlobalIndex = GlobalEventManager.GetNextAvailableGlobalIndex(this);
        }

        public Telerik.Windows.RoutedEvent AddOwner(Type ownerType)
        {
            GlobalEventManager.AddOwner(this, ownerType);
            return this;
        }

        public static DependencyObject GetLogicalParent(DependencyObject obj)
        {
            WeakReference weakRef = (WeakReference) obj.GetValue(LogicalParentProperty);
            if (weakRef == null)
            {
                return null;
            }
            return (weakRef.Target as DependencyObject);
        }

        internal bool IsLegalHandler(Delegate handler)
        {
            Type type = handler.GetType();
            if (type != this.HandlerType)
            {
                return (type == typeof(RoutedEventHandler));
            }
            return true;
        }

        public static void SetLogicalParent(DependencyObject obj, DependencyObject value)
        {
            obj.SetValue(LogicalParentProperty, new WeakReference(value));
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[] { this.OwnerType.Name, this.Name });
        }

        internal int GlobalIndex { get; private set; }

        public Type HandlerType { get; private set; }

        public string Name { get; private set; }

        public Type OwnerType { get; private set; }

        public Telerik.Windows.RoutingStrategy RoutingStrategy { get; private set; }
    }
}

