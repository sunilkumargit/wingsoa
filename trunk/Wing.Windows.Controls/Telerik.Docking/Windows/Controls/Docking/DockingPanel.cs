namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    [DefaultProperty("LastChildFill")]
    public class DockingPanel : Panel
    {
        public static readonly DependencyProperty DockProperty = DependencyProperty.RegisterAttached("Dock", typeof(Dock), typeof(DockingPanel), new PropertyMetadata(Dock.Left, new PropertyChangedCallback(DockingPanel.OnDockChanged)));
        public static readonly DependencyProperty InitialSizeProperty = DependencyProperty.RegisterAttached("InitialSize", typeof(Size), typeof(DockingPanel), new PropertyMetadata(new Size(240.0, 180.0)));
        public static readonly DependencyProperty LastChildFillProperty = DependencyProperty.Register("LastChildFill", typeof(bool), typeof(DockingPanel), new PropertyMetadata(true, new PropertyChangedCallback(DockingPanel.OnLastChildFillChanged)));

        protected override Size ArrangeOverride(Size finalSize)
        {
            UIElementCollection internalChildren = base.Children;
            int count = internalChildren.Count;
            UIElement lastVisible = base.Children.Cast<UIElement>().LastOrDefault<UIElement>(e => e.Visibility != Visibility.Collapsed);
            int num2 = count;
            if (this.LastChildFill && (lastVisible != null))
            {
                num2 = base.Children.IndexOf(lastVisible);
            }
            double x = 0.0;
            double y = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            for (int i = 0; i < count; i++)
            {
                UIElement element = internalChildren[i];
                if (element != null)
                {
                    Size desiredSize = element.DesiredSize;
                    Rect finalRect = new Rect(x, y, Math.Max((double) 0.0, (double) (finalSize.Width - (x + num5))), Math.Max((double) 0.0, (double) (finalSize.Height - (y + num6))));
                    if (i < num2)
                    {
                        switch (GetDock(element))
                        {
                            case Dock.Left:
                                x += desiredSize.Width;
                                finalRect.Width = desiredSize.Width;
                                break;

                            case Dock.Top:
                                y += desiredSize.Height;
                                finalRect.Height = desiredSize.Height;
                                break;

                            case Dock.Right:
                                num5 += desiredSize.Width;
                                finalRect.X = Math.Max((double) 0.0, (double) (finalSize.Width - num5));
                                finalRect.Width = desiredSize.Width;
                                break;

                            case Dock.Bottom:
                                num6 += desiredSize.Height;
                                finalRect.Y = Math.Max((double) 0.0, (double) (finalSize.Height - num6));
                                finalRect.Height = desiredSize.Height;
                                break;
                        }
                    }
                    element.Arrange(finalRect);
                }
            }
            return finalSize;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification="Dock should be used on UIElements only.")]
        public static Dock GetDock(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (Dock) element.GetValue(DockProperty);
        }

        public static Size GetInitialSize(DependencyObject obj)
        {
            return (Size) obj.GetValue(InitialSizeProperty);
        }

        private static bool IsValidDock(object o)
        {
            Dock dock = (Dock) o;
            if (((dock != Dock.Left) && (dock != Dock.Top)) && (dock != Dock.Right))
            {
                return (dock == Dock.Bottom);
            }
            return true;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            FrameworkElement lastVisibleChild = base.Children.OfType<FrameworkElement>().LastOrDefault<FrameworkElement>(e => e.Visibility != Visibility.Collapsed);
            if (lastVisibleChild != null)
            {
                if (!double.IsNaN(lastVisibleChild.Width) || !double.IsNaN(lastVisibleChild.Height))
                {
                    SetInitialSize(lastVisibleChild, new Size(lastVisibleChild.Width, lastVisibleChild.Height));
                    lastVisibleChild.Width = lastVisibleChild.Height = double.NaN;
                }
                RadSplitContainer c = lastVisibleChild as RadSplitContainer;
                if (c != null)
                {
                    c.SplitterPosition = null;
                }
            }
            foreach (FrameworkElement element in from fe in base.Children.OfType<FrameworkElement>()
                where !object.ReferenceEquals(fe, lastVisibleChild)
                select fe)
            {
                Dock dock = GetDock(element);
                SetSplitterPosition(element as RadSplitContainer, dock);
                switch (dock)
                {
                    case Dock.Left:
                    case Dock.Right:
                    {
                        if (double.IsNaN(element.Width))
                        {
                            element.Width = GetInitialSize(element).Width;
                        }
                        continue;
                    }
                    case Dock.Top:
                    case Dock.Bottom:
                    {
                        if (double.IsNaN(element.Height))
                        {
                            element.Height = GetInitialSize(element).Height;
                        }
                        continue;
                    }
                }
            }
            UIElementCollection internalChildren = base.Children;
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            int num5 = 0;
            int count = internalChildren.Count;
            while (num5 < count)
            {
                UIElement element = internalChildren[num5];
                if (element != null)
                {
                    Size remainingSize = new Size(Math.Max((double) 0.0, (double) (availableSize.Width - num3)), Math.Max((double) 0.0, (double) (availableSize.Height - num4)));
                    element.Measure(remainingSize);
                    Size desiredSize = element.DesiredSize;
                    switch (GetDock(element))
                    {
                        case Dock.Left:
                        case Dock.Right:
                            num2 = Math.Max(num2, num4 + desiredSize.Height);
                            num3 += desiredSize.Width;
                            break;

                        case Dock.Top:
                        case Dock.Bottom:
                            num = Math.Max(num, num3 + desiredSize.Width);
                            num4 += desiredSize.Height;
                            break;
                    }
                }
                num5++;
            }
            return new Size(Math.Max(num, num3), Math.Max(num2, num4));
        }

        private static void OnDockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!IsValidDock(e.NewValue))
            {
                throw new ArgumentException("Invalid Dock value");
            }
            FrameworkElement reference = d as FrameworkElement;
            if (reference != null)
            {
                FrameworkElement parent = reference.Parent as FrameworkElement;
                if (parent != null)
                {
                    parent.InvalidateMeasure();
                }
            }
        }

        private static void OnLastChildFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement panel = d as FrameworkElement;
            if (panel != null)
            {
                panel.InvalidateArrange();
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification="Dock should be used on UIElements only.")]
        public static void SetDock(UIElement element, Dock dock)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(DockProperty, dock);
        }

        public static void SetInitialSize(DependencyObject obj, Size value)
        {
            obj.SetValue(InitialSizeProperty, value);
        }

        private static void SetSplitterPosition(RadSplitContainer container, Dock dock)
        {
            if (container != null)
            {
                switch (dock)
                {
                    case Dock.Left:
                        container.SplitterPosition = Dock.Right;
                        return;

                    case Dock.Top:
                        container.SplitterPosition = Dock.Bottom;
                        return;

                    case Dock.Right:
                        container.SplitterPosition = Dock.Left;
                        return;

                    case Dock.Bottom:
                        container.SplitterPosition = Dock.Top;
                        return;

                    default:
                        return;
                }
            }
        }

        public bool LastChildFill
        {
            get
            {
                return (bool) base.GetValue(LastChildFillProperty);
            }
            set
            {
                base.SetValue(LastChildFillProperty, value);
            }
        }
    }
}

