/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using Ranet.Olap.Core.Metadata;
using System.Windows.Controls;

namespace Ranet.AgOlap.Controls.General.Tree
{
    public class CubeTreeNode : InfoBaseTreeNode
    {
        public CubeTreeNode(CubeDefInfo info) : base(info)
        {
            Icon = UriResources.Images.Cube16;
        }

        public override bool IsInitialized
        {
            get
            {
                //Если у элемента один дочерний и он "WaitNode", то значит данные не грузились
                if (IsWaiting)
                {
                    if(Items.Count == 1)
                        return false;

                    if (Items.Count == 3)
                    {
                        foreach (TreeViewItem item in Items)
                        {
                            MeasuresFolderTreeNode measuresNode = item as MeasuresFolderTreeNode;
                            if (measuresNode != null)
                                continue;
                            KPIsFolderTreeNode kpiNode = item as KPIsFolderTreeNode;
                            if (kpiNode != null)
                                continue;
                            WaitTreeNode waitNode = item as WaitTreeNode;
                            if (waitNode != null)
                                continue;
                            return true;
                        }
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
