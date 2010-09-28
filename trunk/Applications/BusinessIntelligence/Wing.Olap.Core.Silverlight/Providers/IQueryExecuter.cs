/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using Wing.Olap.Core.Data;

namespace Wing.Olap.Core.Providers
{
    public interface IQueryExecuter
    {
        ConnectionInfo Connection { get; }
        CellSetData ExecuteQuery(string query);
        int ExecuteNonQuery(string query);
        DataTableWrapper ExecuteReader(string query);
    }
}
