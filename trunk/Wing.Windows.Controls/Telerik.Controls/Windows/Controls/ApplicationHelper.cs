namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;

    internal class ApplicationHelper
    {
        internal static void AddResizedHandler(EventHandler OnRootSizeChanged)
        {
            Application.Current.Host.Content.Resized += OnRootSizeChanged;
        }

        internal static void AddZoomedHandler(EventHandler OnRootZoomChanged)
        {
            Application.Current.Host.Content.Zoomed += OnRootZoomChanged;
        }

        internal static FrameworkElement GetRootVisual(DependencyObject element)
        {
            if (element == null)
            {
                return RootVisual;
            }
            DependencyObject parent = null;
            while (element != null)
            {
                parent = VisualTreeHelper.GetParent(element);
                if (parent == null)
                {
                    FrameworkElement childElement = element as FrameworkElement;
                    if (childElement != null)
                    {
                        parent = childElement.Parent;
                        if (parent == null)
                        {
                            parent = element;
                            break;
                        }
                    }
                }
                element = parent;
            }
            return (parent as FrameworkElement);
        }

        internal static void RemoveResizedHandler(EventHandler OnRootSizeChanged)
        {
            Application.Current.Host.Content.Resized -= OnRootSizeChanged;
        }

        internal static void RemoveZoomedHandler(EventHandler OnRootZoomChanged)
        {
            Application.Current.Host.Content.Zoomed -= OnRootZoomChanged;
        }

        internal static GeneralTransform TransformToScreenRoot(UIElement target)
        {
            GeneralTransform generalTransform = target.TransformToVisual(null);
            Transform transform = generalTransform as Transform;
            if (transform == null)
            {
                Point p = generalTransform.Transform(new Point(0.0, 0.0));
                return new TranslateTransform { X = p.X * ZoomFactor, Y = p.Y * ZoomFactor };
            }
            TransformGroup group = new TransformGroup();
            group.Children.Add(transform);
            group.Children.Add(new ScaleTransform { ScaleX = 1.0 / ZoomFactor, ScaleY = 1.0 / ZoomFactor });
            return group;
        }

        internal static Size ApplicationSize
        {
            get
            {
                Content plugin = Application.Current.Host.Content;
                return new Size(plugin.ActualWidth / ZoomFactor, plugin.ActualHeight / ZoomFactor);
            }
        }

        internal static FrameworkElement RootVisual
        {
            get
            {
                if (Application.Current == null)
                {
                    return null;
                }
                return (Application.Current.RootVisual as FrameworkElement);
            }
        }

        internal static double ZoomFactor
        {
            get
            {
                if (!Application.Current.Host.Content.IsFullScreen)
                {
                    Content plugin = Application.Current.Host.Content;
                    if (plugin.ZoomFactor != 0.0)
                    {
                        return plugin.ZoomFactor;
                    }
                }
                return 1.0;
            }
        }
    }
}

