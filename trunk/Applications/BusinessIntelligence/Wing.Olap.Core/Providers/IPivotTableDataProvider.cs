/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Collections.Generic;

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
