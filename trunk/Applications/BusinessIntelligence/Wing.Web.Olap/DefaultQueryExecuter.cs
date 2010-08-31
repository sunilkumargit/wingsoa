/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.Data;

using Microsoft.AnalysisServices.AdomdClient;

namespace Wing.Web.Olap
{
    using System.Text;
    using Wing.Olap.Core.Data;
    using Wing.Olap.Core;
    using Wing.Olap.Core.Providers;
    using System.Xml;

    public class DefaultQueryExecuter : IMdxExecuter
    {
        ConnectionInfo m_Connection = null;
        public ConnectionInfo Connection
        {
            get {
                if (m_Connection == null)
                    m_Connection = new ConnectionInfo();
                return m_Connection; 
            }
        }

        public DefaultQueryExecuter(ConnectionInfo connection)
        {
            m_Connection = connection;
        }

        public bool Cancel()
        {
            // TODO: Lock
            if (m_CurrentCmd != null)
            {
                try
                {
                    m_CurrentCmd.Cancel();
                }
                catch (AdomdErrorResponseException)
                {
                }
                finally
                {
                    m_CurrentCmd = null;
                }
            }

            return true;
        }

        private AdomdCommand m_CurrentCmd;

        public int ExecuteNonQuery(String query)
        {
            String sessionId = String.Empty;
            return ExecuteNonQuery(query, ref sessionId);
        }

        public int ExecuteNonQuery(String query, ref String sessionId)
        {
            try
            {
                if (Connection != null)
                {
                    AdomdConnection conn = AdomdConnectionPool.GetConnection(Connection.ConnectionString, sessionId);
                    lock (conn)
                    {
                        sessionId = conn.SessionID;
                        using (AdomdCommand cmd = new AdomdCommand(query, conn))
                        {
                            return cmd.ExecuteNonQuery();
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("{0} ExecuteNonQuery ERROR: {1}\r\n Connection String: {2} \r\n Query: {3}\r\n",
                    DateTime.Now.ToString(), ex.ToString(), Connection.ConnectionString, query);
                throw;
            }
        }

        //public CellSetData ExecuteQuery(string query)
        //{
        //    try
        //    {
        //        if (Connection != null)
        //        {
        //            AdomdConnection conn = AdomdConnectionPool.GetConnection(Connection.ConnectionString);
        //            lock (conn)
        //            {
        //                using (AdomdCommand cmd = new AdomdCommand(query, conn))
        //                {
        //                    DateTime start = DateTime.Now;
        //                    CellSet cs = cmd.ExecuteCellSet();
        //                    System.Diagnostics.Debug.WriteLine("MDX query executimg time: " + (DateTime.Now - start).ToString());

        //                    CellSetDescriptionProvider provider = new CellSetDescriptionProvider(cs);
        //                    provider.CellSet.Connection.ConnectionString = Connection.ConnectionString;
        //                    provider.CellSet.Connection.ConnectionID = Connection.ConnectionID;
        //                    if (cs.OlapInfo != null &&
        //                        cs.OlapInfo.CubeInfo != null &&
        //                        cs.OlapInfo.CubeInfo.Cubes != null &&
        //                        cs.OlapInfo.CubeInfo.Cubes.Count > 0)
        //                    {
        //                        provider.CellSet.CubeName = cs.OlapInfo.CubeInfo.Cubes[0].CubeName;
        //                    }

        //                    return provider.CellSet;
        //                }
        //            }
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Trace.TraceError("{0} ExecuteQuery ERROR: {1}\r\n Connection String: {2} \r\n Query: {3}\r\n",
        //            DateTime.Now.ToString(), ex.ToString(), Connection.ConnectionString, query);
        //        throw ex;
        //    }
        //}

        public DataTable ExecuteReader(string query)
        {
            String sessionId = String.Empty;
            return ExecuteReader(query, ref sessionId);
        }

        public DataTable ExecuteReader(string query, ref String sessionId)
        {
            try
            {
                if (Connection != null)
                {
                    AdomdConnection conn = AdomdConnectionPool.GetConnection(Connection.ConnectionString, sessionId);
                    lock (conn)
                    {
                        sessionId = conn.SessionID;
                        using (AdomdCommand cmd = new AdomdCommand(query, conn))
                        {
                            
                            DateTime start = DateTime.Now;
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader != null)
                                {
                                    DataTable table = new DataTable();
                                    var metadata_table = reader.GetSchemaTable();
                                    if (metadata_table != null)
                                    {
                                        foreach (DataRow row in metadata_table.Rows)
                                        {
                                            table.Columns.Add(new DataColumn(row[0].ToString()));
                                        }
                                    }

                                    if (table.Columns.Count >= reader.FieldCount)
                                    {
                                        while (reader.Read())
                                        {
                                            var values = new object[reader.FieldCount];
                                            for (int i = 0; i < reader.FieldCount; i++)
                                            {
                                                values[i] = reader[i];
                                            }
                                            table.Rows.Add(values);
                                        }
                                    }
                                    System.Diagnostics.Debug.WriteLine("ExecuteReader executimg time: " + (DateTime.Now - start).ToString());
                                    return table;
                                }
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("{0} ExecuteReader ERROR: {1}\r\n Connection String: {2} \r\n Query: {3}\r\n",
                    DateTime.Now.ToString(), ex.ToString(), Connection.ConnectionString, query);
                throw ex;
            }
        }

        public CellSet ExecuteQuery(string query)
        {
            String sessionId = String.Empty;
            return ExecuteQuery(query, ref sessionId);
        }

        public CellSet ExecuteQuery(string query, ref String sessionId)
        {
            try
            {
                if (Connection != null)
                {
                    AdomdConnection conn = AdomdConnectionPool.GetConnection(Connection.ConnectionString, sessionId);
                    lock (conn)
                    {
                        sessionId = conn.SessionID;
                        using (AdomdCommand cmd = new AdomdCommand(query, conn))
                        {
                            DateTime start = DateTime.Now;
                            CellSet cs = cmd.ExecuteCellSet();
                            System.Diagnostics.Debug.WriteLine("MDX query executimg time: " + (DateTime.Now - start).ToString());

                            return cs;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("{0} ExecuteQuery ERROR: {1}\r\n Connection String: {2} \r\n Query: {3}\r\n",
                    DateTime.Now.ToString(), ex.ToString(), Connection.ConnectionString, query);
                throw ex;
            }
        }

        public String GetCellSetDescription(string query)
        {
            String sessionId = String.Empty;
            return GetCellSetDescription(query, ref sessionId);
        }

        public String GetCellSetDescription(string query, ref String sessionId)
        {
            try
            {
                CellSet cs = ExecuteQuery(query, ref sessionId);
                if (cs != null)
                {
                    CellSetDescriptionProvider provider = new CellSetDescriptionProvider(cs);
                    provider.Connection.ConnectionString = Connection.ConnectionString;
                    provider.Connection.ConnectionID = Connection.ConnectionID;

                    DateTime start1 = DateTime.Now;
                    var res = provider.Serialize();
                    System.Diagnostics.Debug.WriteLine("MDX query serialization time: " + (DateTime.Now - start1).ToString());
                    return res;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("{0} GetCellSetDescription ERROR: {1}\r\n Connection String: {2} \r\n Query: {3}\r\n",
                    DateTime.Now.ToString(), ex.ToString(), Connection.ConnectionString, query);
                throw;
            }
        }
    }
}
