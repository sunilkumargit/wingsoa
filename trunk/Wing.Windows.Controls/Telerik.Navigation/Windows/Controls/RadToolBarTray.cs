namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Telerik.Windows.Controls.ToolBar;

    [TemplatePart(Name="PART_ItemsPresenter", Type=typeof(ItemsPresenter)), TemplatePart(Name="PART_BandPanel", Type=typeof(RadToolBarTrayPanel)), DefaultProperty("Items")]
    public class RadToolBarTray : Telerik.Windows.Controls.ItemsControl
    {
        private bool areBandsDirty = true;
        public static readonly DependencyProperty IsLockedProperty = DependencyProperty.RegisterAttached("IsLocked", typeof(bool), typeof(RadToolBarTray), new PropertyMetadata(false));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(System.Windows.Controls.Orientation), typeof(RadToolBarTray), new PropertyMetadata(System.Windows.Controls.Orientation.Horizontal, new PropertyChangedCallback(RadToolBarTray.OnOrientationChanged)));

        public RadToolBarTray()
        {
            base.DefaultStyleKey = typeof(RadToolBarTray);
            this.ToolBars = new Collection<RadToolBar>();
            this.Bands = new List<BandInfo>();
        }

        internal Size ArrangeToolBars(Size finalSize)
        {
            bool isHoriz = this.IsHorizontal;
            Rect toolbarRect = new Rect();
            foreach (BandInfo band in this.Bands)
            {
                List<RadToolBar> toolBars = band.ToolBars;
                double thickness = band.Thickness;
                RadToolBar.SetOffset(isHoriz, 0.0, ref toolbarRect);
                foreach (RadToolBar toolBar in toolBars)
                {
                    toolbarRect.Width = isHoriz ? toolBar.DesiredSize.Width : thickness;
                    toolbarRect.Height = isHoriz ? thickness : toolBar.DesiredSize.Height;
                    toolBar.Arrange(toolbarRect);
                    RadToolBar.CorrectOffset(isHoriz, toolbarRect.Width, toolbarRect.Height, ref toolbarRect);
                }
                RadToolBar.CorrectOffset(!isHoriz, thickness, ref toolbarRect);
            }
            return finalSize;
        }

        private static double CalcMinLegth(List<RadToolBar> toolBars)
        {
            double legth = 0.0;
            foreach (RadToolBar toolBar in toolBars)
            {
                if (!toolBar.TemplateInitialized)
                {
                    toolBar.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }
                legth += toolBar.MinLength;
            }
            return legth;
        }

        private static Size CalcSize(List<RadToolBar> toolBars, bool isHoriz, double availableLength)
        {
            double bandThickness = 0.0;
            double bandLength = 0.0;
            Size measureSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            bandLength = 0.0;
            foreach (RadToolBar toolBar in toolBars)
            {
                if (toolBar.TemplateInitialized)
                {
                    if (availableLength < 0.0)
                    {
                        availableLength = 0.0;
                    }
                    availableLength += toolBar.MinLength;
                    RadToolBar.SetLength(isHoriz, availableLength, ref measureSize);
                    toolBar.Measure(measureSize);
                    double desiredThickness = RadToolBar.GetThickness(isHoriz, toolBar.DesiredSize);
                    bandThickness = Math.Max(bandThickness, desiredThickness);
                    double desiredLength = RadToolBar.GetLength(isHoriz, toolBar.DesiredSize);
                    bandLength += desiredLength;
                    availableLength -= desiredLength;
                }
            }
            return new Size(bandLength, bandThickness);
        }

        private void CallChildrensInitialMeasure()
        {
            if (this.WaitFirstMeasure)
            {
                this.WaitFirstMeasure = false;
                this.RepositionItems();
                foreach (RadToolBar toolBar in this.ToolBars)
                {
                    if (!toolBar.TemplateInitialized)
                    {
                        toolBar.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                }
            }
        }

        private bool CheckBandsOrder()
        {
            int count = 0;
            Collection<RadToolBar> toolBars = this.ToolBars;
            for (int i = 0; i < this.Bands.Count; i++)
            {
                BandInfo band = this.Bands[i];
                if (this.IsBandDirty(band, i))
                {
                    this.areBandsDirty = true;
                    return this.areBandsDirty;
                }
                count += band.ToolBars.Count;
            }
            this.areBandsDirty = count != toolBars.Count;
            return this.areBandsDirty;
        }

        private void GenerateBands()
        {
            if (this.AreBandsDirty)
            {
                this.Bands.Clear();
                foreach (RadToolBar toolBar in this.ToolBars)
                {
                    this.TryCreateBand(toolBar.Band);
                }
                for (int j = 0; j < this.Bands.Count; j++)
                {
                    List<RadToolBar> band = this.Bands[j].ToolBars;
                    for (int k = 0; k < band.Count; k++)
                    {
                        RadToolBar toolBar = band[k];
                        toolBar.Band = j;
                        toolBar.BandIndex = k;
                    }
                }
                this.areBandsDirty = false;
            }
        }

        public static bool GetIsLocked(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (bool) element.GetValue(IsLockedProperty);
        }

        private static FrameworkElement GetVisualChild(DependencyObject control, string name)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(control); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(control, i);
                FrameworkElement element = child as FrameworkElement;
                if ((element != null) && (string.Compare(element.Name, name, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    return element;
                }
                element = GetVisualChild(child, name);
                if (element != null)
                {
                    return element;
                }
            }
            return null;
        }

        private bool IsBandDirty(BandInfo band, int bandID)
        {
            int bandIndex = 0;
            foreach (RadToolBar item in band.ToolBars)
            {
                if (((item.Band != bandID) || (item.BandIndex != bandIndex)) || !this.ToolBars.Contains(item))
                {
                    return true;
                }
                bandIndex++;
            }
            return false;
        }

        private static bool IsValidType(object o)
        {
            return (o is RadToolBar);
        }

        internal Size MeasureToolBars(Size availableSize)
        {
            this.CallChildrensInitialMeasure();
            bool isHoriz = this.IsHorizontal;
            this.GenerateBands();
            Size finalSize = new Size(0.0, 0.0);
            for (int i = 0; i < this.Bands.Count; i++)
            {
                List<RadToolBar> toolBars = this.Bands[i].ToolBars;
                double availableLength = RadToolBar.GetLength(isHoriz, availableSize) - CalcMinLegth(toolBars);
                Size badnSize = CalcSize(toolBars, isHoriz, availableLength);
                double bandThickness = badnSize.Height;
                this.Bands[i].Thickness = bandThickness;
                RadToolBar.CorrectThickness(isHoriz, bandThickness, ref finalSize);
                double maxLenght = Math.Max(RadToolBar.GetLength(isHoriz, finalSize), badnSize.Width);
                RadToolBar.SetLength(isHoriz, maxLenght, ref finalSize);
            }
            return finalSize;
        }

        public override void OnApplyTemplate()
        {
            this.WaitFirstMeasure = false;
            this.BandPanel = null;
            base.OnApplyTemplate();
            this.PrepareBandPanel();
            this.IsTemplateApplied = base.Template != null;
            this.WaitFirstMeasure = true;
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            this.RepositionItems();
        }

        internal static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!RadToolBar.IsValidOrientation(e.NewValue))
            {
                throw new ArgumentException("Invalid Orientation Value", "e");
            }
            RadToolBarTray tray = sender as RadToolBarTray;
            if (tray != null)
            {
                tray.ChangeVisualState(true);
                foreach (RadToolBar toolbar in tray.ToolBars)
                {
                    toolbar.Orientation = tray.Orientation;
                }
                tray.InvalidateMeasure();
                tray.UpdateLayout();
            }
        }

        private void PrepareBandPanel()
        {
            if (this.BandPanel == null)
            {
                DependencyObject itemsPresenter = base.GetTemplateChild("PART_ItemsPresenter");
                if ((itemsPresenter != null) && (VisualTreeHelper.GetChildrenCount(itemsPresenter) > 0))
                {
                    DependencyObject firstChild = VisualTreeHelper.GetChild(itemsPresenter, 0);
                    this.BandPanel = firstChild as FrameworkElement;
                }
                if (this.BandPanel == null)
                {
                    this.BandPanel = GetVisualChild(this, "PART_BandPanel");
                }
                RadToolBarTrayPanel bandPanel = this.BandPanel as RadToolBarTrayPanel;
                if (bandPanel != null)
                {
                    bandPanel.HostTray = this;
                }
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            this.PrepareBandPanel();
        }

        internal void RepositionItems()
        {
            if (this.IsTemplateApplied)
            {
                this.ToolBars.Clear();
                foreach (object item in base.Items)
                {
                    if (IsValidType(item))
                    {
                        RadToolBar toolbar = item as RadToolBar;
                        if (toolbar != null)
                        {
                            toolbar.Orientation = this.Orientation;
                            this.ToolBars.Add(toolbar);
                        }
                    }
                    else
                    {
                        UIElement visual = item as UIElement;
                        if (visual != null)
                        {
                            visual.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                base.InvalidateMeasure();
            }
        }

        public static void SetIsLocked(DependencyObject element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(IsLockedProperty, value);
        }

        private void TryCreateBand(int newBandID)
        {
            for (int i = 0; i < this.Bands.Count; i++)
            {
                int bandID = this.Bands[i].ToolBars[0].Band;
                if (newBandID == bandID)
                {
                    return;
                }
                if (newBandID < bandID)
                {
                    this.Bands.Insert(i, BandInfo.CreateBand(newBandID, this.ToolBars));
                    return;
                }
            }
            this.Bands.Add(BandInfo.CreateBand(newBandID, this.ToolBars));
        }

        private bool AreBandsDirty
        {
            get
            {
                if (!this.areBandsDirty)
                {
                    return this.CheckBandsOrder();
                }
                return true;
            }
            set
            {
                this.areBandsDirty = value;
            }
        }

        private FrameworkElement BandPanel { get; set; }

        private List<BandInfo> Bands { get; set; }

        internal bool IsHorizontal
        {
            get
            {
                return (this.Orientation == System.Windows.Controls.Orientation.Horizontal);
            }
        }

        public bool IsLocked
        {
            get
            {
                return (bool) base.GetValue(IsLockedProperty);
            }
            set
            {
                base.SetValue(IsLockedProperty, value);
            }
        }

        private bool IsTemplateApplied { get; set; }

        [DefaultValue(1)]
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

        internal Collection<RadToolBar> ToolBars { get; set; }

        internal bool WaitFirstMeasure { get; private set; }

        internal class BandInfo
        {
            public BandInfo()
            {
                this.ToolBars = new List<RadToolBar>();
            }

            public static RadToolBarTray.BandInfo CreateBand(int bandID, Collection<RadToolBar> toolBars)
            {
                RadToolBarTray.BandInfo newBand = new RadToolBarTray.BandInfo();
                foreach (RadToolBar toolBar in toolBars)
                {
                    if (bandID == toolBar.Band)
                    {
                        newBand.InsertToolBar(toolBar);
                    }
                }
                return newBand;
            }

            internal void InsertToolBar(RadToolBar toolBar)
            {
                if (toolBar.BandIndex != -1)
                {
                    for (int i = 0; i < this.ToolBars.Count; i++)
                    {
                        if ((this.ToolBars[i].BandIndex != -1) && (toolBar.BandIndex <= this.ToolBars[i].BandIndex))
                        {
                            this.ToolBars.Insert(i, toolBar);
                            return;
                        }
                    }
                }
                this.ToolBars.Add(toolBar);
            }

            public double Thickness { get; set; }

            public List<RadToolBar> ToolBars { get; set; }
        }
    }
}

