namespace Telerik.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Automation.Peers;

    public abstract class ListBoxItem : ContentControl, ISelectable
    {
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(Telerik.Windows.Controls.ListBoxItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(Telerik.Windows.Controls.ListBoxItem.IsSelectedChanged)));
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification="RoutedEvent is immutable")]
        public static readonly Telerik.Windows.RoutedEvent SelectedEvent = Selector.SelectedEvent.AddOwner(typeof(Telerik.Windows.Controls.ListBoxItem));
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification="RoutedEvent is immutable")]
        public static readonly Telerik.Windows.RoutedEvent UnselectedEvent = Selector.UnselectedEvent.AddOwner(typeof(Telerik.Windows.Controls.ListBoxItem));

        public event RoutedEventHandler Selected
        {
            add
            {
                this.AddHandler(SelectedEvent, value);
            }
            remove
            {
                this.RemoveHandler(SelectedEvent, value);
            }
        }

        public event RoutedEventHandler Unselected
        {
            add
            {
                this.AddHandler(UnselectedEvent, value);
            }
            remove
            {
                this.RemoveHandler(UnselectedEvent, value);
            }
        }

        internal ListBoxItem()
        {
        }

        private void HandleIsSelectedChanged(bool newValue, RadRoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        private static void IsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            d.SetValue(Selector.IsSelectedProperty, args.NewValue);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new Telerik.Windows.Controls.Automation.Peers.ListBoxItemAutomationPeer(this, (Telerik.Windows.Controls.Automation.Peers.SelectorAutomationPeer) FrameworkElementAutomationPeer.FromElement(this.ParentSelector));
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
            }
        }

        protected virtual void OnSelected(RadRoutedEventArgs e)
        {
            this.HandleIsSelectedChanged(true, e);
        }

        protected virtual void OnUnselected(RadRoutedEventArgs e)
        {
            this.HandleIsSelectedChanged(false, e);
        }

        void ISelectable.OnSelected(RadRoutedEventArgs e)
        {
            this.OnSelected(e);
        }

        void ISelectable.OnUnselected(RadRoutedEventArgs e)
        {
            this.OnUnselected(e);
        }

        public bool IsSelected
        {
            get
            {
                return (bool) base.GetValue(IsSelectedProperty);
            }
            set
            {
                base.SetValue(IsSelectedProperty, value);
            }
        }

        internal Selector ParentSelector
        {
            get
            {
                return (Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(this) as Selector);
            }
        }
    }
}

