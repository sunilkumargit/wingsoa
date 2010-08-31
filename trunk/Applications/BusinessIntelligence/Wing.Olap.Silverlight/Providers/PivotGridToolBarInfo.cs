/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;

namespace Wing.Olap.Providers
{
    public class PivotGridToolBarInfo
    {
        public PivotGridToolBarInfo()
        {
        }

        public int HistorySize = 0;
        public int CurrentHistoryIndex = -1;

				public bool HideEmptyRows = false;
				public bool HideEmptyColumns = false;
        public bool RotateAxes=false;
    }
}
