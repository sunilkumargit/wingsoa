namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Telerik.Windows;
    using Telerik.Windows.Controls.OutlookBar;

    [TemplatePart(Name="HeaderElement", Type=typeof(ContentControl)), TemplateVisualState(Name="Selected", GroupName="CommonStates"), TemplateVisualState(Name="MouseOver", GroupName="CommonStates")]
    public class RadOutlookBarItem : RadTabItem, IOutlookBarItem
    {
        private ContentControl headerElement;
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(RadOutlookBarItem), null);
        private OutlookBarItemPosition location;
        public static readonly DependencyProperty MinimizedContentProperty = DependencyProperty.Register("MinimizedContent", typeof(object), typeof(RadOutlookBarItem), null);
        public static readonly DependencyProperty MinimizedContentTemplateProperty = DependencyProperty.Register("MinimizedContentTemplate", typeof(DataTemplate), typeof(RadOutlookBarItem), null);
        public static readonly DependencyProperty MinimizedContentTemplateSelectorProperty = DependencyProperty.Register("MinimizedContentTemplateSelector", typeof(DataTemplateSelector), typeof(RadOutlookBarItem), null);
        private WeakReference minimizedItem = new WeakReference(null);
        private RadOutlookBar parentOutlookBar;
        public static readonly Telerik.Windows.RoutedEvent PositionChangedEvent = EventManager.RegisterRoutedEvent("PositionChanged", RoutingStrategy.Bubble, typeof(PositionChangedEventHandler), typeof(RadOutlookBarItem));
        public static readonly DependencyProperty SmallIconProperty = DependencyProperty.Register("SmallIcon", typeof(ImageSource), typeof(RadOutlookBarItem), null);
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(RadOutlookBarItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadOutlookBarItem.OnTitleChanged)));
        public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(RadOutlookBarItem), null);
        public static readonly DependencyProperty TitleTemplateSelectorProperty = DependencyProperty.Register("TitleTemplateSelector", typeof(DataTemplateSelector), typeof(RadOutlookBarItem), null);

        public event PositionChangedEventHandler PositionChanged
        {
            add
            {
                this.AddHandler(PositionChangedEvent, value);
            }
            remove
            {
                this.RemoveHandler(PositionChangedEvent, value);
            }
        }

        public RadOutlookBarItem()
        {
            base.DefaultStyleKey = typeof(RadOutlookBarItem);
        }

        internal void ClearContent()
        {
            if (this.headerElement != null)
            {
                this.headerElement.ContentTemplate = null;
                this.headerElement.Content = null;
            }
        }

        private void FirePossitionChangedEvent(OutlookBarItemPosition oldLocation, OutlookBarItemPosition newLocation)
        {
            this.RaiseEvent(new PositionChangedEventArgs(this, oldLocation, newLocation));
        }

        public override void OnApplyTemplate()
        {
            this.ClearContent();
            base.OnApplyTemplate();
            this.headerElement = base.GetTemplateChild("HeaderElement") as ContentControl;
        }

        protected override void OnHeaderChanged(object oldHeader, object newHeader)
        {
            if (this.ParentOutlookBar != null)
            {
                this.ParentOutlookBar.UpdateTitle();
                if (this.MinimizedItem != null)
                {
                    this.MinimizedItem.Content = this.ParentOutlookBar.GetMinimizedContent(base.Header);
                }
            }
        }

        private void OnIsInPanelChanged(OutlookBarItemPosition position)
        {
            if (this.ParentOutlookBar != null)
            {
                this.ParentOutlookBar.OnItemContainerPositionChanged(this, position);
            }
        }

        protected override void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            base.OnIsSelectedChanged(oldValue, newValue);
            base.IsTabStop = false;
        }

        protected virtual void OnTitleChanged(object oldHeader, object newHeader)
        {
            if (this.ParentOutlookBar != null)
            {
                this.ParentOutlookBar.UpdateTitle();
            }
        }

        private static void OnTitleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as RadOutlookBarItem).OnTitleChanged(e.OldValue, e.NewValue);
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override ControlTemplate BottomTemplate
        {
            get
            {
                return base.BottomTemplate;
            }
            set
            {
                throw new NotSupportedException("BottomTemplate is not supported by RadOutlookBarItem");
            }
        }

        internal ContentControl HeaderElement
        {
            get
            {
                return this.headerElement;
            }
        }

        public ImageSource Icon
        {
            get
            {
                return (ImageSource) base.GetValue(IconProperty);
            }
            set
            {
                base.SetValue(IconProperty, value);
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool IsBreak
        {
            get
            {
                return base.IsBreak;
            }
            set
            {
                throw new NotSupportedException("IsBreak is not supported by RadOutlookBarItem");
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override ControlTemplate LeftTemplate
        {
            get
            {
                return base.LeftTemplate;
            }
            set
            {
                throw new NotSupportedException("LeftTemplate is not supported by RadOutlookBarItem");
            }
        }

        public object MinimizedContent
        {
            get
            {
                return base.GetValue(MinimizedContentProperty);
            }
            set
            {
                base.SetValue(MinimizedContentProperty, value);
            }
        }

        public DataTemplate MinimizedContentTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(MinimizedContentTemplateProperty);
            }
            set
            {
                base.SetValue(MinimizedContentTemplateProperty, value);
            }
        }

        public DataTemplateSelector MinimizedContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector) base.GetValue(MinimizedContentTemplateSelectorProperty);
            }
            set
            {
                base.SetValue(MinimizedContentTemplateSelectorProperty, value);
            }
        }

        internal MinimizedOutlookBarItem MinimizedItem
        {
            get
            {
                return (this.minimizedItem.Target as MinimizedOutlookBarItem);
            }
            set
            {
                this.minimizedItem = new WeakReference(value);
            }
        }

        private RadOutlookBar ParentOutlookBar
        {
            get
            {
                if (this.parentOutlookBar == null)
                {
                    this.parentOutlookBar = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(this) as RadOutlookBar;
                }
                return this.parentOutlookBar;
            }
        }

        public OutlookBarItemPosition Position
        {
            get
            {
                return this.location;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ControlTemplate RightTemplate
        {
            get
            {
                return base.RightTemplate;
            }
            set
            {
                throw new NotSupportedException("RightTemplate is not supported by RadOutlookBarItem");
            }
        }

        public ImageSource SmallIcon
        {
            get
            {
                return (ImageSource) base.GetValue(SmallIconProperty);
            }
            set
            {
                base.SetValue(SmallIconProperty, value);
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Orientation TabOrientation
        {
            get
            {
                return base.TabOrientation;
            }
            internal set
            {
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Dock TabStripPlacement
        {
            get
            {
                return base.TabStripPlacement;
            }
            internal set
            {
            }
        }

        OutlookBarItemPosition IOutlookBarItem.Location
        {
            get
            {
                return this.location;
            }
            set
            {
                OutlookBarItemPosition oldLocation = this.location;
                this.location = value;
                if (value != oldLocation)
                {
                    this.OnIsInPanelChanged(value);
                    this.FirePossitionChangedEvent(oldLocation, value);
                }
            }
        }

        public object Title
        {
            get
            {
                return base.GetValue(TitleProperty);
            }
            set
            {
                base.SetValue(TitleProperty, value);
            }
        }

        public DataTemplate TitleTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(TitleTemplateProperty);
            }
            set
            {
                base.SetValue(TitleTemplateProperty, value);
            }
        }

        public DataTemplateSelector TitleTemplateSelector
        {
            get
            {
                return (DataTemplateSelector) base.GetValue(TitleTemplateSelectorProperty);
            }
            set
            {
                base.SetValue(TitleTemplateSelectorProperty, value);
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ControlTemplate TopTemplate
        {
            get
            {
                return base.TopTemplate;
            }
            set
            {
                throw new NotSupportedException("TopTemplate is not supported by RadOutlookBarItem");
            }
        }
    }
}

