/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using Wing.Olap.Providers;

namespace Wing.Olap.Controls.PivotGrid.Controls
{
    public class CellValueChangedEventArgs : EventArgs
    {
        public readonly List<UpdateEntry> Changes;

        public CellValueChangedEventArgs(List<UpdateEntry> changes)
        {
            if (changes == null)
                throw new ArgumentNullException("changes");
            Changes = changes;
        }
    }

    public class FocusedCellChangedEventArgs : EventArgs
    {
        public object Owner = null;
        public readonly CellControl OldFocusedCell = null;
        public readonly CellControl NewFocusedCell = null;

        public FocusedCellChangedEventArgs(object owner,
            CellControl oldFocusedCell,
            CellControl newFocusedCell)
        {
            this.Owner = owner;
            this.OldFocusedCell = oldFocusedCell;
            this.NewFocusedCell = newFocusedCell;
        }
    }

}
