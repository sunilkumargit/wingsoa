namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using Telerik.Windows;
    using Telerik.Windows.Controls.PanelBar;
    using Telerik.Windows.Controls.Primitives;

    [DefaultProperty("ExpandMode"), TemplatePart(Name="transformationRoot", Type=typeof(LayoutTransformControl)), DefaultEvent("Selected"), TemplateVisualState(Name="Horizontal", GroupName="OrientationStates"), TemplateVisualState(Name="Vertical", GroupName="OrientationStates")]
    public class RadPanelBar : RadTreeView
    {
        public static readonly DependencyProperty ExpandModeProperty = DependencyProperty.Register("ExpandMode", typeof(Telerik.Windows.Controls.ExpandMode), typeof(RadPanelBar), new System.Windows.PropertyMetadata(new PropertyChangedCallback(RadPanelBar.OnExpandModePropertyChanged)));
        public static readonly Telerik.Windows.RoutedEvent OrientationChangedEvent = EventManager.RegisterRoutedEvent("OrientationChanged", RoutingStrategy.Bubble, typeof(EventHandler<OrientationChangedEventArgs>), typeof(RadPanelBar));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(System.Windows.Controls.Orientation), typeof(RadPanelBar), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadPanelBar.OnOrientationPropertyChanged)));

        public event EventHandler<OrientationChangedEventArgs> OrientationChanged
        {
            add
            {
                this.AddHandler(OrientationChangedEvent, value);
            }
            remove
            {
                this.RemoveHandler(OrientationChangedEvent, value);
            }
        }

        public RadPanelBar()
        {
            base.DefaultStyleKey = typeof(RadPanelBar);
            
            base.IsExpandOnSingleClickEnabled = true;
            base.IsExpandOnDblClickEnabled = false;
            base.IsSingleExpandPath = true;
        }

        private void AdjustOrientation(RadPanelBarItem item)
        {
            item.AdjustOrientation(this.Orientation);
            for (int i = 0; i < item.Items.Count; i++)
            {
                RadPanelBarItem childContainer = item.ItemContainerGenerator.ContainerFromIndex(i) as RadPanelBarItem;
                if (childContainer != null)
                {
                    this.AdjustOrientation(childContainer);
                }
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            RadPanelBarItem container = element as RadPanelBarItem;
            if (container != null)
            {
                container.Level = -1;
            }
            base.ClearContainerForItemOverride(element, item);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RadPanelBarItem();
        }

        internal object GetDefaultStyleKey()
        {
            return base.DefaultStyleKey;
        }

        private bool GoToState(bool useTransitions, string stateName)
        {
            return VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        public bool HasSelectedItem()
        {
            return (base.SelectedItem != null);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is RadPanelBarItem);
        }

        public override void OnApplyTemplate()
        {
            this.ForEachContainerItem<RadPanelBarItem>(delegate (RadPanelBarItem panelBarItem) {
                panelBarItem.CleanContentPresenters();
            });
            base.OnApplyTemplate();
            this.UpdateVisualState(false);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadPanelBarAutomationPeer(this);
        }

        protected virtual void OnExpandModeChanged(Telerik.Windows.Controls.ExpandMode oldValue, Telerik.Windows.Controls.ExpandMode newValue)
        {
            base.IsSingleExpandPath = newValue == Telerik.Windows.Controls.ExpandMode.Single;
        }

        private static void OnExpandModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadPanelBar source = d as RadPanelBar;
            Telerik.Windows.Controls.ExpandMode newValue = (Telerik.Windows.Controls.ExpandMode) e.NewValue;
            Telerik.Windows.Controls.ExpandMode oldValue = (Telerik.Windows.Controls.ExpandMode) e.OldValue;
            if (source != null)
            {
                source.OnExpandModeChanged(oldValue, newValue);
            }
        }

        protected internal virtual void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadPanelBar panelBar = d as RadPanelBar;
            if (panelBar != null)
            {
                panelBar.OnOrientationChanged(new OrientationChangedEventArgs(panelBar, (System.Windows.Controls.Orientation) e.NewValue, OrientationChangedEvent));
                panelBar.UpdateVisualState(false);
                for (int i = 0; i < panelBar.Items.Count; i++)
                {
                    RadPanelBarItem container = panelBar.ItemContainerGenerator.ContainerFromIndex(i) as RadPanelBarItem;
                    if (container != null)
                    {
                        panelBar.AdjustOrientation(container);
                    }
                }
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            RadPanelBarItem panelBarItem = element as RadPanelBarItem;
            if (panelBarItem != null)
            {
                panelBarItem.Level = 1;
            }
            base.PrepareContainerForItemOverride(element, item);
            StyleManager.SetThemeFromParent(element, this);
            panelBarItem.UpdateHeaderPresenterContent();
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="useTransitions"), SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        internal void UpdateVisualState(bool useTransitions)
        {
            if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                this.GoToState(false, "Vertical");
            }
            else
            {
                this.GoToState(false, "Horizontal");
            }
        }

        [Browsable(true), Telerik.Windows.Controls.SRDescription("PanelBarExpandModePropertyDescription"), Category("Behavior"), DefaultValue(0)]
        public Telerik.Windows.Controls.ExpandMode ExpandMode
        {
            get
            {
                return (Telerik.Windows.Controls.ExpandMode) base.GetValue(ExpandModeProperty);
            }
            set
            {
                base.SetValue(ExpandModeProperty, value);
            }
        }

        public System.Windows.Controls.Orientation Orientation
        {
            get
            {
                return (System.Windows.Controls.Orientation) base.GetValue(OrientationProperty);
            }
            set
            {
                base.SetValue(OrientationProperty, value);
            }
        }
    }
}

