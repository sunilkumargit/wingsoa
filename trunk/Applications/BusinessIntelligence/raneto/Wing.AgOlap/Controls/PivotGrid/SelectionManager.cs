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
using System.Collections.Generic;
using Ranet.AgOlap.Controls.PivotGrid.Controls;

namespace Ranet.AgOlap.Controls.PivotGrid
{
    public class SelectionManager<T>
    {
        public event EventHandler SelectionChanged;
        void Raise_SelectionChanged()
        {
            EventHandler handler = SelectionChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private IList<IList<T>> m_Selection = new List<IList<T>>();

        public void ClearSelection()
        {
            foreach (IList<T> list in m_Selection)
            {
                ChangeSelection(list, false);
            }
               
            this.m_Selection.Clear();
            selection = null;
            Raise_SelectionChanged();
        }

        IList<T> selection = null;
        public IList<T> GetSelection()
        {
            if (selection == null)
            {
                // debug DateTime start = DateTime.Now;

                selection = new List<T>();
                foreach (IList<T> val in this.m_Selection)
                {
                    if (val != null)
                    {
                        foreach (T item in val)
                        {
                            if (item != null && !selection.Contains(item))
                            {
                                selection.Add(item);
                            }
                        }
                    }
                }

                // debug DateTime stop = DateTime.Now;
                // debug System.Diagnostics.Debug.WriteLine("GetSelection: " + stop.ToString() + " Time: " + (stop - start).ToString());
            }
            return selection;
        }

        bool SlectionChanged = true;

        public IList<T> LastSelectionArea
        {
            get
            {
                if (this.m_Selection.Count > 0)
                {
                    return this.m_Selection[this.m_Selection.Count - 1];
                }
                return null;
            }
        }

        public int LastSelectionAreaSize
        {
            get
            {
                if (this.m_Selection.Count > 0)
                {
                    IList<T> list = this.m_Selection[this.m_Selection.Count - 1];
                    if (list != null)
                        return list.Count;
                }
                return 0;
            }
        }

        public void ChangeSelectionArea(IList<T> list)
        {
            if (list != null)
            {
                if (this.m_Selection.Count > 0)
                {
                    // Определяем какие ячейки из старого списка отсутствуют в новом
                    IList<T> old_list = this.m_Selection[this.m_Selection.Count - 1];
                    IList<T> delta = new List<T>();
                    if(old_list != null)
                    {
                        foreach (T item in old_list)
                        {
                            if(!list.Contains(item))
                            {
                                delta.Add(item);
                            }
                        }
                        ChangeSelection(delta, false);
                    }
                    this.m_Selection[this.m_Selection.Count - 1] = list;
                    ChangeSelection(list, true);
                }
                else
                {
                    this.m_Selection.Add(list);
                    ChangeSelection(list, true);
                }
                selection = null;
                Raise_SelectionChanged();
            }
        }

        public void AddSelectionArea(IList<T> list)
        {
            if (list != null)
            {
                this.m_Selection.Add(list);
                ChangeSelection(list, true);
                selection = null;
                Raise_SelectionChanged();
            }
        }

        private void ChangeSelection(IList<T> list, bool val)
        {
            if (list != null)
            {
                foreach (T item in list)
                {
                    CellControl cell = item as CellControl;
                    if (cell != null)
                    {
                        cell.IsSelected = val;
                    }
                }
            }
        }
    }
}
