/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows.Controls;

namespace Wing.Olap.Controls.PivotGrid.Editors
{
    public interface ICustomCellEditor
    {
        event EventHandler PivotGridEditorCancelEdit;
        event EventHandler PivotGridEditorEndEdit;

        object Value { get; set; }
        Control Editor { get; }
    }
}
