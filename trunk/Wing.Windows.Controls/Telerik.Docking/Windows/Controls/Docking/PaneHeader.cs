namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using Telerik.Windows;
    using Telerik.Windows.Controls;

    public class PaneHeader : Control
    {
        internal static readonly Telerik.Windows.RoutedEvent DragCompletedEvent = RadPane.DragCompletedEvent.AddOwner(typeof(PaneHeader));
        internal static readonly Telerik.Windows.RoutedEvent DragDeltaEvent = RadPane.DragDeltaEvent.AddOwner(typeof(PaneHeader));
        internal static readonly Telerik.Windows.RoutedEvent DragStartedEvent = RadPane.DragStartedEvent.AddOwner(typeof(PaneHeader));
        private RadToggleButton headerDropDownMenu;
        private static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsDragging", typeof(bool), typeof(PaneHeader), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(PaneHeader.OnIsDraggingPropertyChanged)));
        public static readonly DependencyProperty IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;
        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(PaneHeader), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(PaneHeader.OnIsHightlightedChange)));
        private bool isMouseCaptured;
        public static readonly DependencyProperty SelectedPaneProperty = DependencyProperty.Register("SelectedPane", typeof(RadPane), typeof(PaneHeader), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(PaneHeader.OnSelectedPaneChange)));
        public static readonly DependencyProperty SelectedTitleTemplateProperty = DependencyProperty.Register("SelectedTitleTemplate", typeof(DataTemplate), typeof(PaneHeader), null);
        private ContentControl titleElement;

        public PaneHeader()
        {
            base.DefaultStyleKey = typeof(PaneHeader);
            base.Loaded += new RoutedEventHandler(this.PaneHeader_Loaded);
            base.LostMouseCapture += new MouseEventHandler(this.OnLostMouseCapture);
        }

        private void BindDropDownVisibility()
        {
            if ((this.headerDropDownMenu != null) && (this.SelectedPane != null))
            {
                RadContextMenu.GetContextMenu(this.SelectedPane);
            }
        }

        internal void CancelDrag()
        {
            if (this.IsDragging)
            {
                if (this.isMouseCaptured)
                {
                    base.ReleaseMouseCapture();
                    this.isMouseCaptured = false;
                }
                this.ClearValue(IsDraggingPropertyKey);
                this.StopDrag();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.titleElement != null)
            {
                this.titleElement.ClearValue(ContentControl.ContentProperty);
                this.titleElement.ClearValue(ContentControl.ContentTemplateProperty);
            }
            this.titleElement = base.GetTemplateChild("TitleElement") as ContentControl;
            this.headerDropDownMenu = base.GetTemplateChild("HeaderDropDownMenu") as RadToggleButton;
            FrameworkElement closeButton = base.GetTemplateChild("HeaderCloseButton") as FrameworkElement;
            if (closeButton != null)
            {
                Binding binding = new Binding("SelectedPane.CanUserClose") {
                    RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
                    Converter = new BooleanToVisibilityConverter()
                };
                closeButton.SetBinding(UIElement.VisibilityProperty, binding);
            }
        }

        protected virtual void OnDraggingChanged(DependencyPropertyChangedEventArgs eventArgs)
        {
        }

        private static void OnIsDraggingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PaneHeader) d).OnDraggingChanged(e);
        }

        private static void OnIsHightlightedChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PaneHeader header = d as PaneHeader;
            if (header != null)
            {
                header.UpdateVisualState();
            }
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            this.CancelDrag();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!this.IsDragging && this.CanFloat)
            {
                base.Focus();
                this.isMouseCaptured = base.CaptureMouse();
                this.SetValue(IsDraggingPropertyKey, this.isMouseCaptured);
                if (this.isMouseCaptured)
                {
                    bool flag = false;
                    try
                    {
                        this.StartDrag(e);
                        flag = true;
                    }
                    finally
                    {
                        if (!flag)
                        {
                            this.CancelDrag();
                        }
                    }
                }
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (this.isMouseCaptured && this.IsDragging)
            {
                e.Handled = true;
                base.ReleaseMouseCapture();
                this.isMouseCaptured = false;
                this.ClearValue(IsDraggingPropertyKey);
                this.EndDrag(e);
            }
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.IsDragging)
            {
                this.DragDelta(e);
            }
        }

        private static void OnSelectedPaneChange(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            PaneHeader header = d as PaneHeader;
            if (header != null)
            {
                header.UpdateVisualState();
                header.BindDropDownVisibility();
            }
        }

        private void PaneHeader_Loaded(object sender, RoutedEventArgs e)
        {
            base.Dispatcher.DesignerSafeBeginInvoke(delegate {
                this.UpdateVisualState();
            });
        }

        internal void UpdateCheckedState(bool isChecked)
        {
            if (this.headerDropDownMenu != null)
            {
                this.headerDropDownMenu.IsChecked = new bool?(isChecked);
            }
        }

        protected void UpdateVisualState()
        {
            if (this.IsHighlighted)
            {
                VisualStateManager.GoToState(this, "Highlighted", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "NotHighlighted", false);
            }
            if (this.SelectedPane != null)
            {
                if (this.SelectedPane.IsPinned)
                {
                    VisualStateManager.GoToState(this, "Pinned", false);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Unpinned", false);
                }
                RadPaneGroup paneGroup = this.PaneHeaderGroup ?? this.SelectedPane.PaneGroup;
                if (paneGroup != null)
                {
                    RadPane selectedPane = paneGroup.SelectedPane ?? this.SelectedPane;
                    RadContextMenu contextMenu = RadContextMenu.GetContextMenu(selectedPane);
                    if ((contextMenu != null) && (contextMenu.Items.Count > 0))
                    {
                        VisualStateManager.GoToState(this, "CommandsMenuNormalState", false);
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, "CommandsMenuHiddenState", false);
                    }
                }
            }
        }

        private bool CanFloat
        {
            get
            {
                if (this.PaneHeaderGroup == null)
                {
                    return false;
                }
                return (from pane in this.PaneHeaderGroup.Items.Cast<object>()
                    where pane is RadPane
                    select pane).All<object>(pane => (pane as RadPane).CanFloat);
            }
        }

        [Category("Appearance"), Browsable(false)]
        public bool IsDragging
        {
            get
            {
                return (bool) base.GetValue(IsDraggingProperty);
            }
            protected set
            {
                this.SetValue(IsDraggingPropertyKey, value);
            }
        }

        public bool IsHighlighted
        {
            get
            {
                return (bool) base.GetValue(IsHighlightedProperty);
            }
            set
            {
                base.SetValue(IsHighlightedProperty, value);
            }
        }

        internal RadPaneGroup PaneHeaderGroup { get; set; }

        public RadPane SelectedPane
        {
            get
            {
                return (RadPane) base.GetValue(SelectedPaneProperty);
            }
            set
            {
                base.SetValue(SelectedPaneProperty, value);
            }
        }
    }
}

