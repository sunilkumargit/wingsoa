/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Olap.Core.Providers
{
    public interface IPivotGridDataProvider
    {
        IDictionary<string, object> Properties { get; }
        MemberInfoCollection Columns { get; }
        //IEnumerable<PivotAreaData> ColumnHeaders { get; }
        MemberInfoCollection Rows { get; }
        //IEnumerable<PivotAreaData> RowHeaders { get; }
        CellValueInfo GetValue(params int[] indexVector);
        IList<MemberInfo> GetInvisibleCoords(params int[] index);
        //IEnumerable<MemberInfo> GetFilterCoords();
    }
}
