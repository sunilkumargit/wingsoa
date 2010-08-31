using System;
using System.Data;
using Microsoft.AnalysisServices.AdomdClient;

namespace Wing.Olap.Core.Providers
{
    public interface IMdxExecuter
    {
        ConnectionInfo Connection { get; }
        CellSet ExecuteQuery(string query);
        DataTable ExecuteReader(string query);
        int ExecuteNonQuery(string query);
        String GetCellSetDescription(string query);
    }
}
