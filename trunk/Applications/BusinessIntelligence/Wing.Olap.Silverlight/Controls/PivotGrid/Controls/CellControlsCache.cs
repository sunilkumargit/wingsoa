/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Collections.Generic;
using Wing.Olap.Core.Providers;

namespace Wing.Olap.Controls.PivotGrid.Controls
{
    internal class CellControlsCache
    {
        IDictionary<CellInfo, CellControl> m_CellsViewCache = new Dictionary<CellInfo, CellControl>();
        public IDictionary<CellInfo, CellControl> CellsViewCache
        {
            get { return m_CellsViewCache; }
        }

        IDictionary<int, Dictionary<int, CellControl>> m_CellsCache = new Dictionary<int, Dictionary<int, CellControl>>();

        public CellControl this[CellInfo cellInfo]
        {
            get
            {
                CellControl res = null;
                if (this.m_CellsViewCache.ContainsKey(cellInfo))
                {
                    res = this.m_CellsViewCache[cellInfo];
                }
                return res;
            }
        }

        public CellControl this[
                int columnIndex,
                int rowIndex]
        {
            get
            {
                CellControl res = null;
                Dictionary<int, CellControl> columnDict = null;
                if (this.m_CellsCache.ContainsKey(rowIndex))
                {
                    columnDict = this.m_CellsCache[rowIndex];
                }
                if (columnDict != null)
                {
                    if (columnDict.ContainsKey(columnIndex))
                    {
                        res = columnDict[columnIndex];
                    }
                }

                return res;
            }
        }

        public void Add(CellControl cell,
            int columnIndex,
            int rowIndex)
        {
            if (cell != null && cell.Cell != null)
            {
                m_CellsViewCache[cell.Cell] = cell;
            }

            Dictionary<int, CellControl> columnDict = null;
            if (this.m_CellsCache.ContainsKey(rowIndex))
            {
                columnDict = this.m_CellsCache[rowIndex];
            }

            if (columnDict == null)
            {
                columnDict = new Dictionary<int, CellControl>();
                this.m_CellsCache.Add(rowIndex, columnDict);
            }

            if (!columnDict.ContainsKey(columnIndex))
            {
                columnDict.Add(columnIndex, cell);
            }
            else
            {
                columnDict[columnIndex] = cell;
            }
        }
    }
}
