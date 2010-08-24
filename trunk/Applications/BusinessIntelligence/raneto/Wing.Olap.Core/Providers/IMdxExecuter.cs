using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AnalysisServices.AdomdClient;
using System.Data;

namespace Ranet.Olap.Core.Providers
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
