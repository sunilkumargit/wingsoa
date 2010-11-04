namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Telerik.Windows;
    using Telerik.Windows.Controls;

    [TemplateVisualState(Name="TopIndicatorHidden", GroupName="TopIndicatorVisibilityStates"), TemplateVisualState(Name="Normal", GroupName="CommonStates"), TemplateVisualState(Name="TopIndicatorVisibile", GroupName="TopIndicatorVisibilityStates"), TemplateVisualState(Name="LeftIndicatorHidden", GroupName="LeftIndicatorVisibilityStates"), TemplateVisualState(Name="HighlightRightIndicator", GroupName="CommonStates"), TemplateVisualState(Name="RightIndicatorHidden", GroupName="RightIndicatorVisibilityStates"), TemplateVisualState(Name="BottomIndicatorVisibile", GroupName="BottomIndicatorVisibilityStates"), TemplateVisualState(Name="BottomIndicatorHidden", GroupName="BottomIndicatorVisibilityStates"), TemplateVisualState(Name="CenterIndicatorVisibile", GroupName="CenterIndicatorVisibilityStates"), TemplateVisualState(Name="CenterIndicatorHidden", GroupName="CenterIndicatorVisibilityStates"), TemplatePart(Name="PART_TopIndicator", Type=typeof(FrameworkElement)), TemplateVisualState(Name="HighlightTopIndicator", GroupName="CommonStates"), TemplateVisualState(Name="RightIndicatorVisibile", GroupName="RightIndicatorVisibilityStates"), TemplateVisualState(Name="HighlightBottomIndicator", GroupName="CommonStates"), TemplateVisualState(Name="HighlightCenterIndicator", GroupName="CommonStates"), TemplatePart(Name="PART_LeftIndicator", Type=typeof(FrameworkElement)), TemplateVisualState(Name="LeftIndicatorVisibile", GroupName="LeftIndicatorVisibilityStates"), TemplatePart(Name="PART_RightIndicator", Type=typeof(FrameworkElement)), TemplatePart(Name="PART_BottomIndicator", Type=typeof(FrameworkElement)), TemplatePart(Name="PART_CenterIndicator", Type=typeof(FrameworkElement)), TemplatePart(Name="PART_Center", Type=typeof(FrameworkElement)), TemplateVisualState(Name="HighlightLeftIndicator", GroupName="CommonStates")]
    public class Compass : Control
    {
        private FrameworkElement bottomIndicator;
        internal const string BOTTOMINDICATOR = "PART_BottomIndicator";
        internal const string BOTTOMINDICATORHIDDENSTATE = "BottomIndicatorHidden";
        internal const string BOTTOMINDICATORVISIBILESTATE = "BottomIndicatorVisibile";
        internal const string BOTTOMINDICATORVISIBILITYSTATESGROUP = "BottomIndicatorVisibilityStates";
        internal const string CENTER = "PART_Center";
        private FrameworkElement centerIndicator;
        internal const string CENTERINDICATOR = "PART_CenterIndicator";
        internal const string CENTERINDICATORHIDDENSTATE = "CenterIndicatorHidden";
        internal const string CENTERINDICATORVISIBILESTATE = "CenterIndicatorVisibile";
        internal const string CENTERINDICATORVISIBILITYSTATESGROUP = "CenterIndicatorVisibilityStates";
        internal const string COMMONSTATESGROUP = "CommonStates";
        private static readonly DependencyPropertyKey DockPositionPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("DockPosition", typeof(Telerik.Windows.Controls.Docking.DockPosition?), typeof(Compass), new Telerik.Windows.PropertyMetadata(null, new PropertyChangedCallback(Compass.OnStateChanged)));
        public static readonly DependencyProperty DockPositionProperty = DockPositionPropertyKey.DependencyProperty;
        internal const string HIGHLIGHTBOTTOMINDICATORSTATE = "HighlightBottomIndicator";
        internal const string HIGHLIGHTCENTERINDICATORSTATE = "HighlightCenterIndicator";
        internal const string HIGHLIGHTLEFTINDICATORSTATE = "HighlightLeftIndicator";
        internal const string HIGHLIGHTRIGHTINDICATORSTATE = "HighlightRightIndicator";
        internal const string HIGHLIGHTTOPINDICATORSTATE = "HighlightTopIndicator";
        public static readonly DependencyProperty IsBottomIndicatorVisibleProperty = DependencyProperty.Register("IsBottomIndicatorVisible", typeof(bool), typeof(Compass), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(Compass.OnStateChanged)));
        public static readonly DependencyProperty IsCenterIndicatorVisibleProperty = DependencyProperty.Register("IsCenterIndicatorVisible", typeof(bool), typeof(Compass), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(Compass.OnStateChanged)));
        public static readonly DependencyProperty IsLeftIndicatorVisibleProperty = DependencyProperty.Register("IsLeftIndicatorVisible", typeof(bool), typeof(Compass), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(Compass.OnStateChanged)));
        public static readonly DependencyProperty IsRightIndicatorVisibleProperty = DependencyProperty.Register("IsRightIndicatorVisible", typeof(bool), typeof(Compass), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(Compass.OnStateChanged)));
        public static readonly DependencyProperty IsTopIndicatorVisibleProperty = DependencyProperty.Register("IsTopIndicatorVisible", typeof(bool), typeof(Compass), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(Compass.OnStateChanged)));
        private FrameworkElement leftIndicator;
        internal const string LEFTINDICATOR = "PART_LeftIndicator";
        internal const string LEFTINDICATORHIDDENSTATE = "LeftIndicatorHidden";
        internal const string LEFTINDICATORVISIBILESTATE = "LeftIndicatorVisibile";
        internal const string LEFTINDICATORVISIBILITYSTATESGROUP = "LeftIndicatorVisibilityStates";
        internal const string NORMALSTATE = "Normal";
        private FrameworkElement rightIndicator;
        internal const string RIGHTINDICATOR = "PART_RightIndicator";
        internal const string RIGHTINDICATORHIDDENSTATE = "RightIndicatorHidden";
        internal const string RIGHTINDICATORVISIBILESTATE = "RightIndicatorVisibile";
        internal const string RIGHTINDICATORVISIBILITYSTATESGROUP = "RightIndicatorVisibilityStates";
        private FrameworkElement rootPart;
        private FrameworkElement topIndicator;
        internal const string TOPINDICATOR = "PART_TopIndicator";
        internal const string TOPINDICATORHIDDENSTATE = "TopIndicatorHidden";
        internal const string TOPINDICATORVISIBILESTATE = "TopIndicatorVisibile";
        internal const string TOPINDICATORVISIBILITYSTATESGROUP = "TopIndicatorVisibilityStates";

        public Compass()
        {
            base.DefaultStyleKey = typeof(Compass);
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="position")]
        internal void ChangeDockPosition(Point position)
        {
            this.DockPosition = (from e in (base.Parent as UIElement).GetElementsInHostCoordinates<FrameworkElement>(position)
                where e.IsHitTestVisible
                select this.GetCompassPosition(e)).FirstOrDefault<Telerik.Windows.Controls.Docking.DockPosition?>(dp => dp.HasValue);
        }

        private void ChangeVisualState()
        {
            VisualStateManager.GoToState(this, this.IsLeftIndicatorVisible ? "LeftIndicatorVisibile" : "LeftIndicatorHidden", true);
            VisualStateManager.GoToState(this, this.IsTopIndicatorVisible ? "TopIndicatorVisibile" : "TopIndicatorHidden", true);
            VisualStateManager.GoToState(this, this.IsRightIndicatorVisible ? "RightIndicatorVisibile" : "RightIndicatorHidden", true);
            VisualStateManager.GoToState(this, this.IsBottomIndicatorVisible ? "BottomIndicatorVisibile" : "BottomIndicatorHidden", true);
            VisualStateManager.GoToState(this, this.IsCenterIndicatorVisible ? "CenterIndicatorVisibile" : "CenterIndicatorHidden", true);
            if (this.DockPosition.HasValue)
            {
                Telerik.Windows.Controls.Docking.DockPosition dockPosition = this.DockPosition.GetValueOrDefault();
                if (this.DockPosition.HasValue)
                {
                    switch (dockPosition)
                    {
                        case Telerik.Windows.Controls.Docking.DockPosition.Top:
                            VisualStateManager.GoToState(this, "HighlightTopIndicator", true);
                            return;

                        case Telerik.Windows.Controls.Docking.DockPosition.Bottom:
                            VisualStateManager.GoToState(this, "HighlightBottomIndicator", true);
                            return;

                        case Telerik.Windows.Controls.Docking.DockPosition.Center:
                            VisualStateManager.GoToState(this, "HighlightCenterIndicator", true);
                            return;

                        case Telerik.Windows.Controls.Docking.DockPosition.Left:
                            VisualStateManager.GoToState(this, "HighlightLeftIndicator", true);
                            return;

                        case Telerik.Windows.Controls.Docking.DockPosition.Right:
                            VisualStateManager.GoToState(this, "HighlightRightIndicator", true);
                            return;
                    }
                }
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        internal void ClearCompassIndicators()
        {
            base.ClearValue(IsLeftIndicatorVisibleProperty);
            base.ClearValue(IsTopIndicatorVisibleProperty);
            base.ClearValue(IsRightIndicatorVisibleProperty);
            base.ClearValue(IsBottomIndicatorVisibleProperty);
            base.ClearValue(IsCenterIndicatorVisibleProperty);
        }

        private Telerik.Windows.Controls.Docking.DockPosition? GetCompassPosition(FrameworkElement element)
        {
            if (element == this.leftIndicator)
            {
                return Telerik.Windows.Controls.Docking.DockPosition.Left;
            }
            if (element == this.topIndicator)
            {
                return Telerik.Windows.Controls.Docking.DockPosition.Top;
            }
            if (element == this.rightIndicator)
            {
                return Telerik.Windows.Controls.Docking.DockPosition.Right;
            }
            if (element == this.bottomIndicator)
            {
                return Telerik.Windows.Controls.Docking.DockPosition.Bottom;
            }
            if ((element != this.rootPart) && (element != this.centerIndicator))
            {
                return null;
            }
            return Telerik.Windows.Controls.Docking.DockPosition.Center;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.leftIndicator = base.GetTemplateChild("PART_LeftIndicator") as FrameworkElement;
            this.topIndicator = base.GetTemplateChild("PART_TopIndicator") as FrameworkElement;
            this.rightIndicator = base.GetTemplateChild("PART_RightIndicator") as FrameworkElement;
            this.bottomIndicator = base.GetTemplateChild("PART_BottomIndicator") as FrameworkElement;
            this.centerIndicator = base.GetTemplateChild("PART_CenterIndicator") as FrameworkElement;
            this.rootPart = base.GetTemplateChild("PART_Center") as FrameworkElement;
            this.ChangeVisualState();
        }

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Compass).ChangeVisualState();
        }

        public Telerik.Windows.Controls.Docking.DockPosition? DockPosition
        {
            get
            {
                return (Telerik.Windows.Controls.Docking.DockPosition?) base.GetValue(DockPositionProperty);
            }
            internal set
            {
                this.SetValue(DockPositionPropertyKey, value);
            }
        }

        public bool IsBottomIndicatorVisible
        {
            get
            {
                return (bool) base.GetValue(IsBottomIndicatorVisibleProperty);
            }
            set
            {
                base.SetValue(IsBottomIndicatorVisibleProperty, value);
            }
        }

        public bool IsCenterIndicatorVisible
        {
            get
            {
                return (bool) base.GetValue(IsCenterIndicatorVisibleProperty);
            }
            set
            {
                base.SetValue(IsCenterIndicatorVisibleProperty, value);
            }
        }

        public bool IsLeftIndicatorVisible
        {
            get
            {
                return (bool) base.GetValue(IsLeftIndicatorVisibleProperty);
            }
            set
            {
                base.SetValue(IsLeftIndicatorVisibleProperty, value);
            }
        }

        public bool IsRightIndicatorVisible
        {
            get
            {
                return (bool) base.GetValue(IsRightIndicatorVisibleProperty);
            }
            set
            {
                base.SetValue(IsRightIndicatorVisibleProperty, value);
            }
        }

        public bool IsTopIndicatorVisible
        {
            get
            {
                return (bool) base.GetValue(IsTopIndicatorVisibleProperty);
            }
            set
            {
                base.SetValue(IsTopIndicatorVisibleProperty, value);
            }
        }
    }
}

