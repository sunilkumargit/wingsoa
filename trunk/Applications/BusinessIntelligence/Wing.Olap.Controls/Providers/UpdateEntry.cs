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
using System.Collections.Generic;
using Wing.Olap.Core.Providers;

namespace Wing.AgOlap.Providers
{
    /// <summary>
    /// Класс, задающий правило обновления для ячейки
    /// </summary> 
    public class UpdateEntry : Wing.AgOlap.Controls.General.ICloneable
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
