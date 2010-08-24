using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Ranet.AgOlap.Controls.Gauge
{
    [Description("Main gauge panel.  All gauges (radial, linear, digital panel, and state indicator) should be placed here. It can have flexible content layout. Any Silverlight panel object can be used as content of the gauge panel.")]
    public class Gauge : ContentControl
    {
        private const string ResourceUri = "/Ranet.AgOlap;component/Themes/Gauges.xaml";
        // Fields
        //private const string defaultThemeUri = "/Telerik.Windows.Controls.Gauge;component/Themes/generic.xaml";
        //private static readonly DependencyPropertyKey LogicalChildrenKey = DependencyPropertyExtensions.RegisterReadOnly("LogicalChildren", typeof(ObservableCollection<object>), typeof(Gauge), new PropertyMetadata(new ObservableCollection<object>(), null, null));
        public static readonly DependencyProperty LogicalChildrenProperty = DependencyProperty.Register("LogicalChildren", typeof(ObservableCollection<object>), typeof(Gauge), new PropertyMetadata(new ObservableCollection<object>(), null));

        // Methods
        public Gauge()
        {
            base.DefaultStyleKey = typeof(Gauge);
            this.LogicalChildren = new ObservableCollection<object>();
            this.LogicalChildren.CollectionChanged += new NotifyCollectionChangedEventHandler(this.LogicalChildren_CollectionChanged);
        }

        public object FindName(string name)
        {
            object obj2 = base.FindName(name);
            if (obj2 == null)
            {
                foreach (object obj3 in this.LogicalChildren)
                {
                    if ((obj3 is FrameworkElement) && (((FrameworkElement)obj3).Name == name))
                    {
                        obj2 = obj3;
                        break;
                    }
                }
                if (obj2 == null)
                {
                    foreach (object obj4 in this.LogicalChildren)
                    {
                        obj2 = this.CheckChild(obj4, name);
                        if (obj2 != null)
                        {
                            return obj2;
                        }
                    }
                }
            }
            return obj2;
        }

        private object CheckChild(object logicalChild, string name)
        {
            object obj2 = null;
            if (logicalChild is PropertyChangedNotifier)
            {
                return ((PropertyChangedNotifier)logicalChild).FindName(name);
            }
            if (logicalChild is GaugeBase)
            {
                return ((GaugeBase)logicalChild).FindName(name);
            }
            if (!(logicalChild is FrameworkElement))
            {
                return obj2;
            }
            FrameworkElement element = logicalChild as FrameworkElement;
            if (element is Panel)
            {
                Panel panel = element as Panel;
                foreach (object obj3 in panel.Children)
                {
                    obj2 = this.CheckChild(obj3, name);
                    if (obj2 != null)
                    {
                        return obj2;
                    }
                }
                return obj2;
            }
            return ((FrameworkElement)logicalChild).FindName(name);
        }       

        private void LogicalChildren_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (object obj2 in e.OldItems)
                {
                    DependencyObject obj3 = obj2 as DependencyObject;
                    if (obj3 != null)
                    {
                        obj3.SetValue(RoutedEvent.LogicalParentProperty, null);
                    }
                }
            }
            if (e.NewItems != null)
            {
                foreach (object obj4 in e.NewItems)
                {
                    DependencyObject dependencyObject = obj4 as DependencyObject;
                    if (dependencyObject != null)
                    {
                        dependencyObject.SetValue(RoutedEvent.LogicalParentProperty, new WeakReference(this));
                        if (base.GetValue(StyleManager.ThemeProperty) != null)
                        {
                            SetTheme<GaugeBase>(dependencyObject, base.GetValue(StyleManager.ThemeProperty));
                        }
                        else if (StyleManager.ApplicationTheme() == null)
                        {
                            Theme theme = new Theme(new Uri(ResourceUri, UriKind.Relative));
                            if (theme != null)
                            {
                                base.SetValue(StyleManager.ThemeProperty, theme);
                            }
                        }
                    }
                }
            }
        }

        [Description("Called when the value of the System.Windows.Controls.ContentControl.Content property changes.")]
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (oldContent != null)
            {
                this.LogicalChildren.Remove(oldContent);
            }
            if (newContent != null)
            {
                this.LogicalChildren.Add(newContent);
            }
        }

        [Description("Resets the theme.")]
        public void ResetTheme()
        {
            if (base.GetValue(StyleManager.ThemeProperty) != null)
            {
                SetTheme<GaugeBase>(base.Content as DependencyObject, base.GetValue(StyleManager.ThemeProperty));
            }
        }

        internal static void SetTheme<T>(DependencyObject dependencyObject, object theme)
        {
            if (dependencyObject != null)
            {
                if (dependencyObject is T)
                {
                    dependencyObject.SetValue(StyleManager.ThemeProperty, theme);
                }
                int childrenCount = VisualTreeHelper.GetChildrenCount(dependencyObject);
                if (childrenCount > 0)
                {
                    for (int i = 0; i < childrenCount; i++)
                    {
                        SetTheme<T>(VisualTreeHelper.GetChild(dependencyObject, i), theme);
                    }
                }
            }
        }

        public ObservableCollection<object> LogicalChildren
        {
            get
            {
                return (ObservableCollection<object>)base.GetValue(LogicalChildrenProperty);
            }
            private set { base.SetValue(LogicalChildrenProperty,value); }
        }
    }


}
