/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Text;
using System.Web;

using Microsoft.AnalysisServices.AdomdClient;

namespace Ranet.Web.Olap
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
    //public static class AdomdConnectionPool
    //{
    //    public static AdomdConnection GetConnection(string connectionString)
    //    {
    //        try
    //        {
    //            //AdomdConnection conn = new AdomdConnection(connectionString);
    //            //conn.Open();
    //            //return conn;

    //            AdomdConnection conn = null;
    //            if (HttpContext.Current != null)
    //            {
    //                if (HttpContext.Current.Session != null)
    //                {
    //                    conn = HttpContext.Current.Session[connectionString] as AdomdConnection;
    //                    if (conn != null)
    //                    {
    //                        lock (conn)
    //                        {
    //                            if (conn.State == ConnectionState.Closed)
    //                                conn.Open();
    //                        }
    //                    }
    //                }
    //                else
    //                {

    //                    if (HttpContext.Current.Application != null)
    //                    {
    //                        conn = HttpContext.Current.Application[connectionString] as AdomdConnection;
    //                        if (conn != null)
    //                        {
    //                            lock (conn)
    //                            {
    //                                if (conn.State == ConnectionState.Closed)
    //                                    conn.Open();
    //                            }
    //                        }
    //                    }
    //                }
    //            }

    //            if (conn == null)
    //            {
    //                conn = new AdomdConnection(connectionString);
    //                conn.Open();

    //                if (HttpContext.Current != null)
    //                {
    //                    if (HttpContext.Current.Session != null)
    //                    {
    //                        HttpContext.Current.Session[connectionString] = conn;
    //                    }
    //                    else
    //                    {
    //                        if (HttpContext.Current.Application != null)
    //                        {
    //                            HttpContext.Current.Application[connectionString] = conn;
    //                        }
    //                    }
    //                }
    //            }

    //            return conn;
    //        }
    //        catch (Exception ex)
    //        {
    //            System.Diagnostics.Trace.TraceError("{0} GetConnection ERROR: {1}\r\n Connection String: {2}\r\n",
    //                DateTime.Now.ToString(), ex.ToString(), connectionString);

    //            throw ex;
    //        }
    //    }

    //    public static void Clear()
    //    {
    //        foreach (object obj in HttpContext.Current.Session)
    //        {
    //            AdomdConnection conn = obj as AdomdConnection;
    //            if (conn != null)
    //            {
    //                conn.Close();
    //            }
    //        }
    //    }
    //}
}
