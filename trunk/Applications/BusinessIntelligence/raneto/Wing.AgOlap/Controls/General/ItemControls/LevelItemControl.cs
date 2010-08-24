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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using Ranet.AgOlap.Controls.General;
using Ranet.Olap.Core.Metadata;

namespace Ranet.AgOlap.Controls.General.ItemControls
{
    public class LevelItemControl : ItemControlBase
    {
        public LevelItemControl(LevelInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            
            m_Info = info;
            Text = info.Caption;

            UseAllLevelIcon = true;
        }

        LevelInfo m_Info = null;
        public LevelInfo Info
        {
            get {
                return m_Info;
            }
        }

        public bool UseAllLevelIcon
        {
            set
            {
                if (value)
                {
                    if (m_Info.LevelNumber > -1)
                    {
                        switch (m_Info.LevelNumber)
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
                    if (m_Info.LevelNumber > -1)
                    {
                        switch (m_Info.LevelNumber)
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
    }
}
