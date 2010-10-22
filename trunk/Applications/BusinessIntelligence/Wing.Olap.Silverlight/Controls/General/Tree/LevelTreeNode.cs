/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.General.Tree
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
