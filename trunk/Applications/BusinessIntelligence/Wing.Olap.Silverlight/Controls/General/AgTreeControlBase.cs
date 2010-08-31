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
using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.General
{
    public class ItemEventArgs : EventArgs
    {
        public readonly InfoBase Info = null;

        public ItemEventArgs(InfoBase info)
        {
            Info = info;
        }
    }

    public class AgTreeControlBase : AgControlBase
    {
        public event EventHandler<ItemEventArgs> SelectedItemChanged;

        protected void Raise_SelectedItemChanged(InfoBase info)
        {
            EventHandler<ItemEventArgs> handler = SelectedItemChanged;
            if (handler != null)
            {
                handler(this, new ItemEventArgs(info));
            }
        }
    }
}
