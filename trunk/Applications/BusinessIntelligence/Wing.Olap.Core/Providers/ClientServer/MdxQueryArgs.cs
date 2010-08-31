/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;


namespace Wing.Olap.Core.Providers.ClientServer
{
    public enum QueryTypes
    {
        Select,
        Update,
        CommitTransaction,
        RollbackTransaction,
        DrillThrough
    }
    
    public class MdxQueryArgs : OlapActionBase
    {
        public MdxQueryArgs()
        {
            ActionType = OlapActionTypes.ExecuteQuery;
        }

        /// <summary>
        /// Соединение
        /// </summary>
        public String Connection = String.Empty;
        /// <summary>
        /// ID сессии для MS AS
        /// </summary>
        public String SessionId = String.Empty;

        List<String> m_Queries;
        // TODO: Use own collection or simplified interface.
        public List<String> Queries
        {
            get
            {
                if (m_Queries == null) { m_Queries = new List<string>(); }
                return m_Queries;
            }
            set {
                m_Queries = value;
            }
        }

        public QueryTypes Type = QueryTypes.Select; 
    }
}
