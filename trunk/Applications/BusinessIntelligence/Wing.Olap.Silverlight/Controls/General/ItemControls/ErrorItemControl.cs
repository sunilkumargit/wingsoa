/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;

namespace Wing.Olap.Controls.General.ItemControls
{
    public class ErrorItemControl : ItemControlBase
    {
        public readonly String NODE_TEXT = Localization.TreeNode_ErrorLoadingData;

        public ErrorItemControl()
        {
            Text = NODE_TEXT;
            Icon = UriResources.Images.Error16;
        }
   }
}