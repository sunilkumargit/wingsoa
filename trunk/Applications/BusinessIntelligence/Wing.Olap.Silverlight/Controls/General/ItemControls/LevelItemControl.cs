/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
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
using Wing.Olap.Controls.General;
using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.General.ItemControls
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
