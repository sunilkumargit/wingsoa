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
using System.Linq;
using System.Text;

namespace Ranet.AgOlap.Providers.MemberActions
{
	public interface IHistoryItem<HistoryItem> where HistoryItem : class, new()
	{
		HistoryItem Clone();
	}
	public class HistoryManager<HistoryItem> where HistoryItem : class, IHistoryItem<HistoryItem>, new()
	{
		List<HistoryItem> m_History = new List<HistoryItem>();
		HistoryItem m_CurrentHistoryItem = new HistoryItem();

		public HistoryManager()
		{
			ClearHistory();
		}
		public virtual void ClearHistory()
		{
			m_History.Clear();
			m_History.Add(m_CurrentHistoryItem);
		}

		/// <summary>
		/// Удаляет все элементы истории, стоящие за текущим
		/// </summary>
		/// <param name="item"></param>
		public void CutRight()
		{
			int indx = m_History.IndexOf(CurrentHistoryItem) + 1;
			if (indx < 1)
				return;

			while (m_History.Count > indx)
			{
				m_History.RemoveAt(m_History.Count - 1);
			}
		}
		protected void AddHistoryItem(HistoryItem item)
		{
			if (item == null)
				return;

			int indx = m_History.IndexOf(item);
			if (indx >= 0)
				return;

			m_CurrentHistoryItem = item;
			m_History.Add(m_CurrentHistoryItem);
		}
		public int HistorySize
		{
			get
			{
				return m_History.Count;
			}
		}
		public int CurrentHistoryItemIndex
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
		/// <summary>
		/// Текущий элемент истории
		/// </summary>
		protected internal HistoryItem CurrentHistoryItem
		{
			get
			{
				return m_CurrentHistoryItem;
			}
		}

		public void MoveBack()
		{
			if (m_History.Count > 0)
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
			if (m_History.Count > 0)
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
		protected virtual void AddCurrentStateToHistory()
		{
			// Удаляем все элементы истории, стоящие за текущим
			this.CutRight();
			// Клонируем текущий элемент истории чтобы действие добавлялось уже в клон
			HistoryItem clone = this.CurrentHistoryItem.Clone();
			this.AddHistoryItem(clone);
		}
	}
}
