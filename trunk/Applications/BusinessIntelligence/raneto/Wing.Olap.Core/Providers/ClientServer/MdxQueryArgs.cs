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
using System.Net;
using System.Collections.Generic;


namespace Ranet.Olap.Core.Providers.ClientServer
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
