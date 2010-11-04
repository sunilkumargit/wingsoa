namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Primitives;

    [TemplateVisualState(Name="Unselected", GroupName="SelectionStates"), TemplateVisualState(Name="Selected", GroupName="SelectionStates"), DefaultEvent("Selected"), TemplatePart(Name="Header", Type=typeof(FrameworkElement)), TemplatePart(Name="transformationRoot", Type=typeof(LayoutTransformControl)), TemplateVisualState(Name="MouseOver", GroupName="CommonStates"), TemplateVisualState(Name="MouseOut", GroupName="CommonStates"), DefaultProperty("Header"), TemplateVisualState(Name="Expanded", GroupName="ExpandStates"), TemplateVisualState(Name="Collapsed", GroupName="ExpandStates")]
    public class RadPanelBarItem : RadTreeViewItem
    {
        public static readonly DependencyProperty ChildItemsTemplateProperty = DependencyProperty.Register("ChildItemsTemplate", typeof(ControlTemplate), typeof(RadPanelBarItem), null);
        private ContentPresenter headerElementPresenter;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private static readonly DependencyPropertyKey IsMouseOverPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsMouseOver", typeof(bool), typeof(RadPanelBarItem), null);
        public static readonly DependencyProperty IsMouseOverProperty = IsMouseOverPropertyKey.DependencyProperty;
        private bool isTemplateApplied;
        private FrameworkElement itemHeader;
        private static readonly DependencyPropertyKey LevelPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("Level", typeof(int), typeof(RadPanelBarItem), null);
        public static readonly DependencyProperty LevelProperty = LevelPropertyKey.DependencyProperty;

        public RadPanelBarItem()
        {
            
            base.DefaultStyleKey = typeof(RadPanelBarItem);
        }

        internal void AdjustOrientation(Orientation o)
        {
            LayoutTransformControl transformRoot = base.GetTemplateChild("transformationRoot") as LayoutTransformControl;
            if ((transformRoot != null) && (this.Level == 1))
            {
                double a = (o == Orientation.Horizontal) ? ((double) 90) : ((double) 0);
                transformRoot.LayoutTransform = null;
                transformRoot.LayoutTransform = new RotateTransform { Angle = a };
            }
        }

        protected internal override void ChangeVisualState(bool useTransitions)
        {
            if (!base.IsEnabled)
            {
                this.GoToState(new string[] { "Disabled" });
            }
            else
            {
                if ((!this.IsMouseOver && !base.IsSelected) && (!base.IsExpanded && base.IsEnabled))
                {
                    this.GoToState(new string[] { "Normal" });
                }
                if (base.IsSelected)
                {
                    this.GoToState(new string[] { "Selected" });
                }
                else
                {
                    this.GoToState(new string[] { "Unselected" });
                    if (this.IsMouseOver)
                    {
                        this.GoToState(new string[] { "MouseOver" });
                    }
                    else
                    {
                        this.GoToState(new string[] { "MouseOut" });
                    }
                }
                if (base.IsFocused)
                {
                    this.GoToState(new string[] { "Focused" });
                }
                else
                {
                    this.GoToState(new string[] { "Unfocused" });
                }
                if (base.IsExpanded)
                {
                    this.GoToState(new string[] { "Expanded" });
                }
                else
                {
                    this.GoToState(new string[] { "Collapsed" });
                }
            }
        }

        internal void CleanContentPresenters()
        {
            if (this.headerElementPresenter != null)
            {
                this.headerElementPresenter.ContentTemplate = null;
                this.headerElementPresenter.Content = null;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RadPanelBarItem();
        }

        internal void GoToState(params string[] stateNames)
        {
            if (stateNames != null)
            {
                foreach (string str in stateNames)
                {
                    VisualStateManager.GoToState(this, str, true);
                }
            }
        }

        private void HeaderElementMouseEnter(object sender, MouseEventArgs e)
        {
            this.IsMouseOver = true;
            this.ChangeVisualState();
        }

        private void HeaderElementMouseLeave(object sender, MouseEventArgs e)
        {
            this.IsMouseOver = false;
            this.ChangeVisualState();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is RadPanelBarItem);
        }

        public override void OnApplyTemplate()
        {
            this.CleanContentPresenters();
            base.OnApplyTemplate();
            if (this.itemHeader != null)
            {
                this.itemHeader.MouseLeave -= new MouseEventHandler(this.HeaderElementMouseLeave);
                this.itemHeader.MouseEnter -= new MouseEventHandler(this.HeaderElementMouseEnter);
            }
            this.itemHeader = base.GetTemplateChild("HeaderRow") as FrameworkElement;
            if (this.itemHeader != null)
            {
                this.itemHeader.MouseEnter += new MouseEventHandler(this.HeaderElementMouseEnter);
                this.itemHeader.MouseLeave += new MouseEventHandler(this.HeaderElementMouseLeave);
            }
            this.headerElementPresenter = base.GetTemplateChild("Header") as ContentPresenter;
            this.UpdateHeaderPresenterContent();
            this.StretchItemContent();
            this.ChangeVisualState();
            if (this.ParentPanelBar != null)
            {
                this.AdjustOrientation(this.ParentPanelBar.Orientation);
            }
            this.isTemplateApplied = true;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadPanelBarItemAutomationPeer(this);
        }

        protected override void OnHeaderChanged(object oldHeader, object newHeader)
        {
            base.OnHeaderChanged(oldHeader, newHeader);
            this.UpdateHeaderPresenterContent();
        }

        protected internal override void OnIsExpandedChanged(bool oldValue, bool newValue)
        {
            base.OnIsExpandedChanged(oldValue, newValue);
            if (base.IsExpanded)
            {
                PanelBarPanel.SetDesiredHeight(this, new GridLength(1.0, GridUnitType.Star));
            }
            else
            {
                PanelBarPanel.SetDesiredHeight(this, GridLength.Auto);
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if ((this.ParentPanelBar != null) && this.isTemplateApplied)
            {
                this.StretchItemContent();
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            RadPanelBarItem panelBarItem = element as RadPanelBarItem;
            if (panelBarItem != null)
            {
                panelBarItem.Level = this.Level + 1;
            }
            base.PrepareContainerForItemOverride(element, item);
            StyleManager.SetThemeFromParent(element, this);
            RadPanelBarItem parentItem = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(element) as RadPanelBarItem;
            if (((parentItem != null) && (parentItem.ChildItemsTemplate != null)) && (panelBarItem.ReadLocalValue(Control.TemplateProperty) == DependencyProperty.UnsetValue))
            {
                panelBarItem.Template = this.ChildItemsTemplate;
            }
        }

        private void StretchItemContent()
        {
            if (((base.Item is UIElement) && !(base.Item is RadPanelBarItem)) && (base.Items.Count == 0))
            {
                Grid root = base.GetTemplateChild("RootElement") as Grid;
                if ((root != null) && (root.RowDefinitions.Count > 1))
                {
                    RowDefinition headerRow = root.RowDefinitions[0];
                    RowDefinition contentRow = root.RowDefinitions[1];
                    if ((headerRow != null) && (contentRow != null))
                    {
                        headerRow.Height = new GridLength(1.0, GridUnitType.Star);
                        contentRow.Height = GridLength.Auto;
                        if (base.Items.Count == 0)
                        {
                            PanelBarPanel.SetDesiredHeight(this, new GridLength(1.0, GridUnitType.Star));
                        }
                    }
                }
            }
            else if (base.Items.Count > 0)
            {
                Grid root = base.GetTemplateChild("RootElement") as Grid;
                if ((root != null) && (root.RowDefinitions.Count > 1))
                {
                    RowDefinition headerRow = root.RowDefinitions[0];
                    RowDefinition contentRow = root.RowDefinitions[1];
                    if ((headerRow != null) && (contentRow != null))
                    {
                        headerRow.Height = GridLength.Auto;
                        contentRow.Height = new GridLength(1.0, GridUnitType.Star);
                    }
                }
            }
        }

        internal void UpdateHeaderPresenterContent()
        {
            if (this.headerElementPresenter != null)
            {
                this.headerElementPresenter.Content = base.Header;
            }
        }

        public ControlTemplate ChildItemsTemplate
        {
            get
            {
                return (ControlTemplate) base.GetValue(ChildItemsTemplateProperty);
            }
            set
            {
                base.SetValue(ChildItemsTemplateProperty, value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMouseOver
        {
            get
            {
                return (bool) base.GetValue(IsMouseOverProperty);
            }
            internal set
            {
                this.SetValue(IsMouseOverPropertyKey, value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Level
        {
            get
            {
                return (int) base.GetValue(LevelProperty);
            }
            internal set
            {
                this.SetValue(LevelPropertyKey, value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue((string) null), Category("Behavior"), Browsable(false), Description("Gets the parent PanelBarItem of the current item.")]
        public RadPanelBarItem ParentItem
        {
            get
            {
                return (base.ParentItem as RadPanelBarItem);
            }
        }

        internal RadPanelBar ParentPanelBar
        {
            get
            {
                return (base.ParentTreeView as RadPanelBar);
            }
        }
    }
}

