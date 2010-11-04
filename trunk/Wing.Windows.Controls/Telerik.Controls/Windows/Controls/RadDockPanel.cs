namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using Telerik.Windows;

    [DefaultProperty("LastChildFill")]
    public class RadDockPanel : Panel
    {
        public static readonly DependencyProperty DockProperty = DependencyProperty.RegisterAttached("Dock", typeof(Dock), typeof(RadDockPanel), new System.Windows.PropertyMetadata(Dock.Left, new PropertyChangedCallback(RadDockPanel.OnDockChanged)));
        public static readonly DependencyProperty LastChildFillProperty = DependencyProperty.Register("LastChildFill", typeof(bool), typeof(RadDockPanel), new System.Windows.PropertyMetadata(true, new PropertyChangedCallback(RadDockPanel.OnLastChildFillChanged)));

        public RadDockPanel()
        {
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            UIElementCollection internalChildren = base.Children;
            int count = internalChildren.Count;
            int num2 = count - (this.LastChildFill ? 1 : 0);
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

        internal static bool IsValidDock(object o)
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
                RadDockPanel parent = reference.Parent as RadDockPanel;
                if (parent != null)
                {
                    parent.InvalidateMeasure();
                }
            }
        }

        private static void OnLastChildFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadDockPanel panel = d as RadDockPanel;
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

