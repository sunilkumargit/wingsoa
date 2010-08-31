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
using Wing.AgOlap.Controls.PivotGrid.Controls;
using Wing.Olap.Core.Providers;
using Wing.AgOlap.Providers;

namespace Wing.AgOlap.Controls.PivotGrid
{
    public class CellChangesCache
    {
        List<UpdateEntry> m_CellChanges;

        /// <summary>
        /// Кэш измененных ячеек
        /// </summary>
        List<UpdateEntry> CellChanges
        {
            get
            {
                if (m_CellChanges == null)
                {
                    m_CellChanges = new List<UpdateEntry>();
                }
                return m_CellChanges;
            }
        }

        public int Count
        {
            get
            {
                return CellChanges.Count;
            }
        }

        public List<UpdateEntry> GetCellChanges()
        {
            var res = new List<UpdateEntry>();
            foreach (var entry in CellChanges)
            {
                res.Add(entry.Clone() as UpdateEntry);
            }
            return res;
        }

        public void Clear()
        {
            CellChanges.Clear();
        }

        public void Add(UpdateEntry args)
        {
            RemoveChange(args);
            CellChanges.Add(args);
        }

        /// <summary>
        /// Ищет в кэше изменений ячейку
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public UpdateEntry FindChange(UpdateEntry args)
        {
            foreach (UpdateEntry arg in CellChanges)
            {
                if (CompareTuples(arg.Tuple, args.Tuple))
                    return arg;
            }
            return null;
        }

        /// <summary>
        /// Ищет в кэше изменений ячейку
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public UpdateEntry FindChange(CellInfo cell)
        {
            if (cell != null)
            {
                UpdateEntry entry = new UpdateEntry(cell);
                return FindChange(entry);
            }
            return null;
        }

        public static bool CompareTuples(Dictionary<String, String> tuple1, Dictionary<String, String> tuple2)
        {
            if (tuple1 == null && tuple2 == null)
                return true;
            if (tuple1 == null || tuple2 == null)
                return false;

            if (tuple1.Count != tuple2.Count)
                return false;

            foreach (var key1 in tuple1.Keys)
            {
                if (!tuple2.ContainsKey(key1))
                    return false;
                if (tuple1[key1] != tuple2[key1])
                    return false;
            }
            return true;
        }

        public void RemoveChange(UpdateEntry args)
        {
            UpdateEntry change = FindChange(args);
            if (change != null)
            {
                CellChanges.Remove(change);
            }
        }

        public void RemoveChange(CellInfo cell)
        {
            if (cell != null)
            {
                UpdateEntry entry = new UpdateEntry(cell);
                RemoveChange(entry);
            }
        }

    }
}
