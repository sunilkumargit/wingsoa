/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
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
