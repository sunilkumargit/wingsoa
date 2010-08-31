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