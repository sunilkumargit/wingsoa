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
using Wing.AgOlap.Controls.PivotGrid.Data;
using System.Collections.Generic;
using Wing.Olap.Core.Providers;
using Wing.AgOlap.Providers;

namespace Wing.AgOlap.Controls.PivotGrid.Controls
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
