namespace Telerik.Windows.Controls
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Data;

    public class ContainerBinding
    {
        public static readonly DependencyProperty ContainerBindingsProperty = DependencyProperty.RegisterAttached("ContainerBindings", typeof(ContainerBindingCollection), typeof(ContainerBinding), null);

        internal void Apply(FrameworkElement container)
        {
            DependencyProperty property = this.GetDependencyProperty(container.GetType());
            if (property != null)
            {
                container.SetBinding(property, this.Binding);
            }
        }

        internal static void ApplyContainerBindings(FrameworkElement container, DataTemplate template)
        {
            if (template != null)
            {
                if (container == null)
                {
                    throw new ArgumentNullException("container");
                }
                ContainerBindingCollection bindings = GetContainerBindings(template);
                if (bindings != null)
                {
                    foreach (ContainerBinding containerBinding in bindings)
                    {
                        containerBinding.Apply(container);
                    }
                }
            }
        }

        public static ContainerBindingCollection GetContainerBindings(DependencyObject element)
        {
            return (ContainerBindingCollection) element.GetValue(ContainerBindingsProperty);
        }

        private DependencyProperty GetDependencyProperty(Type targetType)
        {
            FieldInfo field = targetType.GetField(this.PropertyName + "Property", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);
            if (field != null)
            {
                return (field.GetValue(null) as DependencyProperty);
            }
            return null;
        }

        internal void Remove(FrameworkElement container)
        {
            DependencyProperty property = this.GetDependencyProperty(container.GetType());
            if (property != null)
            {
                container.ClearValue(property);
            }
        }

        internal static void RemoveContaienrBindings(FrameworkElement container, DataTemplate template)
        {
            if (template != null)
            {
                if (container == null)
                {
                    throw new ArgumentNullException("container");
                }
                ContainerBindingCollection bindings = GetContainerBindings(template);
                if (bindings != null)
                {
                    foreach (ContainerBinding containerBinding in bindings)
                    {
                        containerBinding.Remove(container);
                    }
                }
            }
        }

        public static void SetContainerBindings(DependencyObject element, ContainerBindingCollection value)
        {
            element.SetValue(ContainerBindingsProperty, value);
        }

        public System.Windows.Data.Binding Binding { get; set; }

        public string PropertyName { get; set; }
    }
}

