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
* /

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ranet.Olap.Core.Providers.MemberActions
{
    public class HistoryManager
    {
        private IList<HistoryItemMDXQuery> m_History = new List<HistoryItemMDXQuery>();

        public void ClearHistory()
        {
            m_History.Clear();
            m_CurrentHistoryItem = null;
        }

        /// <summary>
        /// Удаляет все элементы истории, стоящие за текущим
        /// </summary>
        /// <param name="item"></param>
        public void CutRight()
        {
            if (CurrentHistoryItem != null)
            {
                int indx = m_History.IndexOf(CurrentHistoryItem);
                if(indx > -1)
                {
                    while(m_History.Count > indx + 1)
                    {
                        m_History.RemoveAt(indx + 1);
                    }
                }
            }
        }

        public void AddHistoryItem(HistoryItemMDXQuery item)
        {
            if (item != null)
            {
                m_History.Add(item);
                m_CurrentHistoryItem = item;
            }
        }

        public int Size
        {
            get {
                return m_History.Count;
            }
        }

        public int CurrentHistiryItemIndex
        {
            get
            {
                if (CurrentHistoryItem != null)
                {
                    int indx = m_History.IndexOf(CurrentHistoryItem);
                    if (indx >= 0)
                        return indx;
                }
                return -1;
            }
        }


        HistoryItemMDXQuery m_CurrentHistoryItem;
        /// <summary>
        /// Текущий элемент истории
        /// </summary>
        public HistoryItemMDXQuery CurrentHistoryItem
        {
            get
            {
                return m_CurrentHistoryItem;
            }
        }

        public void MoveBack()
        {
            if (CurrentHistoryItem != null && m_History.Count > 0)
            {
                int indx = m_History.IndexOf(CurrentHistoryItem);
                if (indx > 0)
                {
                    m_CurrentHistoryItem = m_History[indx - 1];
                }
            }
        }

        public void MoveNext()
        {
            if (CurrentHistoryItem != null && m_History.Count > 0)
            {
                int indx = m_History.IndexOf(CurrentHistoryItem);
                if (indx >= 0 && indx < m_History.Count - 1)
                {
                    m_CurrentHistoryItem = m_History[indx + 1];
                }
            }
        }

        public void ToBegin()
        {
            if (m_History.Count > 0)
            {
                m_CurrentHistoryItem = m_History[0];
            }
        }

        public void ToEnd()
        {
            if (m_History.Count > 0)
            {
                m_CurrentHistoryItem = m_History[m_History.Count - 1];
            }
        }

    }
}
// */