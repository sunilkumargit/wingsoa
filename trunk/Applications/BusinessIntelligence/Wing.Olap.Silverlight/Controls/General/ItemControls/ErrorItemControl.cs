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