namespace Telerik.Windows.Controls.DragDrop
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;
    using Telerik.Windows.Controls;

    internal class HideDuringLayoutChangeHelper
    {
        private static Dictionary<FrameworkElement, HideDuringLayoutChangeHelper> hookedElements = new Dictionary<FrameworkElement, HideDuringLayoutChangeHelper>();
        private Visibility lastVisibility;

        private static void Attach(FrameworkElement element)
        {
            HideDuringLayoutChangeHelper helper = new HideDuringLayoutChangeHelper {
                Parents = INotifyLayoutChangeParents(element),
                Element = element
            };
            hookedElements.Add(element, helper);
            helper.AttachEvents();
            element.Loaded += new RoutedEventHandler(helper.OnElementLoaded);
        }

        private void AttachEvents()
        {
            foreach (INotifyLayoutChange p in this.Parents)
            {
                p.LayoutChangeStarted += new EventHandler(this.LayoutChangeStarted);
                p.LayoutChangeEnded += new EventHandler(this.LayoutChangeEnded);
            }
        }

        private static void Detach(FrameworkElement element)
        {
            HideDuringLayoutChangeHelper helper = hookedElements[element];
            element.Loaded -= new RoutedEventHandler(helper.OnElementLoaded);
            helper.DetachEvents();
            hookedElements.Remove(element);
        }

        private void DetachEvents()
        {
            foreach (INotifyLayoutChange p in this.Parents)
            {
                p.LayoutChangeStarted -= new EventHandler(this.LayoutChangeStarted);
                p.LayoutChangeEnded -= new EventHandler(this.LayoutChangeEnded);
            }
        }

        private static IEnumerable<INotifyLayoutChange> INotifyLayoutChangeParents(UIElement element)
        {
            //.TODO.
            return new List<INotifyLayoutChange>();
            //return element.Paren
        }

        private void LayoutChangeEnded(object sender, EventArgs args)
        {
            this.Element.Visibility = this.lastVisibility;
        }

        private void LayoutChangeStarted(object sender, EventArgs args)
        {
            this.lastVisibility = this.Element.Visibility;
            this.Element.Visibility = Visibility.Collapsed;
        }

        private void OnElementLoaded(object sender, EventArgs args)
        {
            this.DetachEvents();
            this.Parents = INotifyLayoutChangeParents(this.Element);
            this.AttachEvents();
            if (this.Parents.Any<INotifyLayoutChange>(p => p.IsLayoutChanging))
            {
                this.lastVisibility = this.Element.Visibility;
                this.Element.Visibility = Visibility.Collapsed;
            }
        }

        internal static void OnHideDuringLayoutChangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = d as FrameworkElement;
            bool newValue = (bool) e.NewValue;
            if (element != null)
            {
                if (newValue)
                {
                    Attach(element);
                }
                else
                {
                    Detach(element);
                }
            }
        }

        private FrameworkElement Element { get; set; }

        private IEnumerable<INotifyLayoutChange> Parents { get; set; }
    }
}

