/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Data;
using System.Web;

using Microsoft.AnalysisServices.AdomdClient;

namespace Wing.Web.Olap
{
    public static class AdomdConnectionPool
    {
        public static AdomdConnection GetConnection(string connectionString, string sessionId)
        {
            HttpContext context = HttpContext.Current;
            if (context != null && context.Session != null)
            {
                AdomdConnection conn = context.Session[connectionString] as AdomdConnection;
                if (conn == null)
                {
                    conn = new AdomdConnection(connectionString);
                    AdomdConnectionPool.OpenConnection(conn, sessionId);
                    context.Session[connectionString] = conn;
                }
                if (conn.State == ConnectionState.Closed)
                {
                    AdomdConnectionPool.OpenConnection(conn, sessionId);
                }

                return conn;
            }

            var tmpConn = new AdomdConnection(connectionString);
            AdomdConnectionPool.OpenConnection(tmpConn, sessionId);

            return tmpConn;
        }

        public static void Clear()
        {
            HttpContext context = HttpContext.Current;
            if (context != null && context.Session != null)
            {
                foreach (object obj in context.Session)
                {
                    AdomdConnection conn = obj as AdomdConnection;
                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            }
        }

        public static void OpenConnection(AdomdConnection conn, string sessionId)
        {
            // В случае если сессия указана, то в случае ошибки открытия соединения открывать в новой сессии
            OpenConnection(conn, sessionId, !String.IsNullOrEmpty(sessionId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sessionId"></param>
        /// <param name="tryInNewSession">Пытаться ли открывать соединение в новой сессии, в случае если ссоедиенение с указанной сессией открыть не удалось</param>
        private static void OpenConnection(AdomdConnection conn, string sessionId, bool tryInNewSession)
        {
            if (!String.IsNullOrEmpty(sessionId))
            {
                conn.SessionID = sessionId;
            }
            try
            {
                conn.Open();
            }
            catch (AdomdException)
            {
                if (tryInNewSession)
                {
                    OpenConnection(conn, null, false);
                }
                else
                {
                    throw;
                }
            }
        }

    }
}
