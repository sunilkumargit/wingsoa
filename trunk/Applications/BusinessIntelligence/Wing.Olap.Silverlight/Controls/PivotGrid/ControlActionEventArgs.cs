/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;

namespace Wing.Olap.Controls.PivotGrid
{
    public class ControlActionEventArgs<T> : EventArgs
    {
        public readonly ControlActionType Action = ControlActionType.None;
        public readonly T UserData = default(T);

        public ControlActionEventArgs(ControlActionType action, T userData)
        {
            Action = action;
            UserData = userData;
        }
    }
}
