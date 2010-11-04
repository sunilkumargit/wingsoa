namespace Telerik.Windows.Controls
{
    using System;
    using System.Globalization;
    using System.Resources;

    internal class SR
    {
        internal const string AppearanceCategory = "Appearance";
        internal const string BehaviorCategory = "Behavior";
        internal const string BrushesCategory = "Brushes";
        internal const string Cancel = "Cancel";
        internal const string ChartAxisLabelDateTimeFormat = "ChartAxisLabelDateTimeFormat";
        internal const string ChartAxisLabelFormat = "ChartAxisLabelFormat";
        internal const string ChartChartLegendHeader = "ChartChartLegendHeader";
        internal const string ChartChartLegendLinearSeriesFormat = "ChartChartLegendLinearSeriesFormat";
        internal const string ChartChartLegendRadialSeriesFormat = "ChartChartLegendRadialSeriesFormat";
        internal const string ChartChartTitle = "ChartChartTitle";
        internal const string ChartHorizontalAxisTitle = "ChartHorizontalAxisTitle";
        internal const string ChartItemLabelFormat = "ChartItemLabelFormat";
        internal const string ChartItemToolTipFormat = "ChartItemToolTipFormat";
        internal const string ChartNoDataMessage = "ChartNoDataMessage";
        internal const string ChartRangeItemToolTipFormat = "ChartRangeItemToolTipFormat";
        internal const string ChartStickItemToolTipFormat = "ChartStickItemToolTipFormat";
        internal const string ChartVerticalAxisTitle = "ChartVerticalAxisTitle";
        internal const string Close = "Close";
        internal const string CommonCategory = "Common Properties";
        internal const string Confirm = "Confirm";
        internal const string ContentCategory = "Content";
        private const string DefaultNamespace = "Telerik.Windows.Controls";
        internal const string LayoutCategory = "Layout";
        internal const string MapAerialCommand = "MapAerialCommand";
        internal const string MapBirdseyeCommand = "MapBirdseyeCommand";
        internal const string MapLabelsVisibleCommand = "MapLabelsVisibleCommand";
        internal const string MapMapScaleKilometerFormat = "MapMapScaleKilometerFormat";
        internal const string MapMapScaleMeterFormat = "MapMapScaleMeterFormat";
        internal const string MapMapScaleMileFormat = "MapMapScaleMileFormat";
        internal const string MapRoadCommand = "MapRoadCommand";
        internal const string Maximize = "Maximize";
        internal const string Minimize = "Minimize";
        internal const string MiscCategory = "Miscellaneous";
        internal const string ObservableCollectionReentrancyNotAllowed = "ObservableCollectionReentrancyNotAllowed";
        internal const string PanelBarExpandModePropertyDescription = "PanelBarExpandModePropertyDescription";
        internal const string PanelBarIsSelectedPropertyDescription = "PanelBarIsSelectedPropertyDescription";
        internal const string PanelBarItemOnApplyTemplateInvalidOperation = "PanelBarItemOnApplyTemplateInvalidOperation";
        internal const string PanelBarSecondLevelTemplatePropertyDescription = "PanelBarSecondLevelTemplatePropertyDescription";
        internal const string PanelBarSelectedValuePathPropertyDescription = "PanelBarSelectedValuePathPropertyDescription";
        internal const string PanelBarTopLevelTemplatePropertyDescription = "PanelBarTopLevelTemplatePropertyDescription";
        internal const string RepeatedGroupDescriptionNotAllowed = "RepeatedGroupDescriptionNotAllowed";
        internal const string Restore = "Restore";
        internal const string TextCategory = "Text";
        internal const string TransformCategory = "Transform";

        public static string Get(string name)
        {
            return SRLoader<Telerik.Windows.Controls.SR>.GetString("Telerik.Windows.Controls", name);
        }

        internal static string Get(string id, params object[] args)
        {
            string format = Resources.GetString(id);
            if (format == null)
            {
                return Resources.GetString("Unavailable");
            }
            if ((args != null) && (args.Length > 0))
            {
                format = string.Format(CultureInfo.CurrentCulture, format, args);
            }
            return format;
        }

        public static object GetObject(string name)
        {
            return SRLoader<Telerik.Windows.Controls.SR>.GetObject("Telerik.Windows.Controls", name);
        }

        public static string GetString(string name)
        {
            return SRLoader<Telerik.Windows.Controls.SR>.GetString("Telerik.Windows.Controls", name);
        }

        public static string GetString(string name, params object[] args)
        {
            return SRLoader<Telerik.Windows.Controls.SR>.GetString("Telerik.Windows.Controls", name, args);
        }

        public static ResourceManager Resources
        {
            get
            {
                return SRLoader<Telerik.Windows.Controls.SR>.GetResources("Telerik.Windows.Controls");
            }
        }
    }
}

