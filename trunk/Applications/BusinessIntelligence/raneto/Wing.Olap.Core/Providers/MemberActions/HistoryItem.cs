/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Wing.UILibrary.Olap
 
    Wing.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Wing.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Wing.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
* /

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Olap.Mdx;

namespace Wing.Olap.Core.Providers.MemberActions
{
	public class HistoryItemMDXQuery
	{
		public List<DrillActionContainer> ColumnsActionChain = new List<DrillActionContainer>();
		public List<DrillActionContainer> RowsActionChain = new List<DrillActionContainer>();

		public DrillActionContainer FindDrillActionContainer(IList<DrillActionContainer> actionChain, String memberId)
		{
			if (actionChain == null)
				return null;

			foreach (DrillActionContainer container in actionChain)
			{
				if (container.MemberUniqueName == memberId)
					return container;
				DrillActionContainer child = FindDrillActionContainer(container.Children, memberId);
				if (child != null)
					return child;
			}
			return null;
		}

		public IList<DrillActionContainer> FindDrillActionContainersByHierarchy(IList<DrillActionContainer> actionChain, String hierarchyUniqueName)
		{
			IList<DrillActionContainer> list = new List<DrillActionContainer>();
			if (actionChain == null)
				return list;

			foreach (DrillActionContainer container in actionChain)
			{
				if (container.HierarchyUniqueName == hierarchyUniqueName && !list.Contains(container))
					list.Add(container);

				IList<DrillActionContainer> childList = FindDrillActionContainersByHierarchy(container.Children, hierarchyUniqueName);
				if (childList != null)
				{
					foreach (DrillActionContainer childContainer in childList)
					{
						if (!list.Contains(childContainer))
						{
							list.Add(childContainer);
						}
					}
				}
			}
			return list;
		}

		#region ICloneable Members

		public object Clone()
		{
			HistoryItemMDXQuery item = new HistoryItemMDXQuery();
			foreach (DrillActionContainer cont in this.ColumnsActionChain)
			{
				item.ColumnsActionChain.Add((DrillActionContainer)cont.Clone());
			}
			foreach (DrillActionContainer cont in this.RowsActionChain)
			{
				item.RowsActionChain.Add((DrillActionContainer)cont.Clone());
			}
			return item;
		}

		#endregion
	}

}
// */