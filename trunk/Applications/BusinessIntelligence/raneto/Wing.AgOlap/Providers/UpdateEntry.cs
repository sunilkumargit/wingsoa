/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
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
using Ranet.Olap.Core.Providers;

namespace Ranet.AgOlap.Providers
{
    /// <summary>
    /// Класс, задающий правило обновления для ячейки
    /// </summary> 
    public class UpdateEntry : Ranet.AgOlap.Controls.General.ICloneable
    {
        public UpdateEntry()
        {
        }

        public UpdateEntry(string newValue)
        {
            this.NewValue = newValue;
        }

        public UpdateEntry(CellInfo cell)
        {
            if (cell != null)
            {
                Tuple = cell.Tuple;
            }
        }

        public UpdateEntry(CellInfo cell, String newValue)
            : this(cell)
        {
            this.NewValue = newValue;
        }

        /// <summary>
        /// Координата ячейки
        /// </summary>
        public readonly Dictionary<String, String> Tuple = new Dictionary<String, String>();

        /// <summary>
        /// Новое значение ячейки
        /// </summary>
        public string NewValue = string.Empty;

        /// <summary>
        /// Старое значение ячейки
        /// </summary>
        public string OldValue = string.Empty;

        /// <summary>
        /// Ошибка при попытке записи в куб
        /// </summary>
        public String Error = string.Empty;

        public bool HasError
        {
            get { return !String.IsNullOrEmpty(Error); }
        }

        #region ICloneable Members

        public object Clone()
        {
            var clone = new UpdateEntry();
            clone.Error = Error;
            clone.NewValue = NewValue;
            clone.OldValue = OldValue;
            foreach(var t in Tuple)
            {
                clone.Tuple.Add(t.Key, t.Value);
            }
            return clone;
        }

        #endregion
    }
}
