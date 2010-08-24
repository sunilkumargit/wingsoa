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
using System.Net;
using Ranet.Olap.Core.Metadata;

namespace Ranet.AgOlap.Controls.General.Tree
{
    public class LevelTreeNode : InfoBaseTreeNode
    {
        public bool UseAllLevelIcon
        {
            set
            {
                LevelInfo info = Info as LevelInfo;
                if (value)
                {
                    if (info.LevelNumber > -1)
                    {
                        switch (info.LevelNumber)
                        {
                            case 0:
                                Icon = UriResources.Images.Level_All_16;
                                break;
                            case 1:
                                Icon = UriResources.Images.Level_01_16;
                                break;
                            case 2:
                                Icon = UriResources.Images.Level_02_16;
                                break;
                            case 3:
                                Icon = UriResources.Images.Level_03_16;
                                break;
                            case 4:
                                Icon = UriResources.Images.Level_04_16;
                                break;
                            case 5:
                                Icon = UriResources.Images.Level_05_16;
                                break;
                            case 6:
                                Icon = UriResources.Images.Level_06_16;
                                break;
                            case 7:
                                Icon = UriResources.Images.Level_07_16;
                                break;
                            case 8:
                                Icon = UriResources.Images.Level_08_16;
                                break;
                            default:
                                Icon = UriResources.Images.Level_09_16;
                                break;
                        }
                    }
                }
                else
                {
                    if (info.LevelNumber > -1)
                    {
                        switch (info.LevelNumber)
                        {
                            case 0:
                                Icon = UriResources.Images.Level_01_16;
                                break;
                            case 1:
                                Icon = UriResources.Images.Level_02_16;
                                break;
                            case 2:
                                Icon = UriResources.Images.Level_03_16;
                                break;
                            case 3:
                                Icon = UriResources.Images.Level_04_16;
                                break;
                            case 4:
                                Icon = UriResources.Images.Level_05_16;
                                break;
                            case 5:
                                Icon = UriResources.Images.Level_06_16;
                                break;
                            case 6:
                                Icon = UriResources.Images.Level_07_16;
                                break;
                            case 7:
                                Icon = UriResources.Images.Level_08_16;
                                break;
                            default:
                                Icon = UriResources.Images.Level_09_16;
                                break;
                        }
                    }
                }
            }
        }

        public LevelTreeNode(LevelInfo info) :
            base(info)
        {
            UseAllLevelIcon = true;
        }
    }
}
