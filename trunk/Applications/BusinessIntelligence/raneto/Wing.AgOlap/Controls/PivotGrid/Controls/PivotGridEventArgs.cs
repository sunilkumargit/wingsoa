/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
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
using Ranet.AgOlap.Controls.PivotGrid.Data;
using System.Collections.Generic;
using Ranet.Olap.Core.Providers;
using Ranet.AgOlap.Providers;

namespace Ranet.AgOlap.Controls.PivotGrid.Controls
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
