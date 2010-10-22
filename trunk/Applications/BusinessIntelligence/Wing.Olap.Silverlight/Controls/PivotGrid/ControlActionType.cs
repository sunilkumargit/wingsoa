/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/


namespace Wing.Olap.Controls.PivotGrid
{
    public enum ControlActionType
    {
        None,
        Copy,
        Paste,
        ShowAttributes,
        ShowMDX,
        ShowSettings,
        ShowProperties,
        ToolBar,
        ValueDelivery,
        ValueCopy,
        DrillThrough,
        SortingByProperty,
        SortingAxisByMeasure,
        SortingByValue,
        ClearAxisSorting,
        AutoWidth,
        DataReorganizationType,
        DataReorganizationType_None,
        DataReorganizationType_MergeNeighbors,
        DataReorganizationType_HitchToParent
    }
}
