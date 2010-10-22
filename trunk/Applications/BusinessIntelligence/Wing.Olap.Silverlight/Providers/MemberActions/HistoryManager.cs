/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Collections.Generic;

namespace Wing.Olap.Providers.MemberActions
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
