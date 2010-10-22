/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.General.ItemControls
{
    public class MeasureGroupItemControl : ItemControlBase
    {
        public MeasureGroupItemControl(MeasureGroupInfo info) 
            : base(false)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            m_Info = info;
            Text = info.Caption;
        }

        MeasureGroupInfo m_Info = null;
        public MeasureGroupInfo Info
        {
            get {
                return m_Info;
            }
        }
    }
}