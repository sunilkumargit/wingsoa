/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

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
