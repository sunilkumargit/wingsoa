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
using Wing.Olap.Controls.General;
using Wing.Olap.Controls.MemberChoice.ClientServer;
using Wing.Olap.Controls.General.ItemControls;
using Wing.Olap.Core.Providers;

namespace Wing.Olap.Controls.MemberChoice.Filter
{
    public class ConditionItemControl : ItemControlBase
    {
        public ConditionItemControl(ConditionTypes condition)
        {
            m_Condition = condition;
            Text = condition.ToString();

            switch (condition)
            { 
                case ConditionTypes.Equal:
                    Icon = UriResources.Images.Equal16;
                    Text = Localization.Filter_Equal;
                    break;
                case ConditionTypes.Contains:
                    Icon = UriResources.Images.Contain16;
                    Text = Localization.Filter_Contains;
                    break;
                case ConditionTypes.BeginWith:
                    Icon = UriResources.Images.BeginWith16;
                    Text = Localization.Filter_BeginWith;
                    break;
            }
        }

        ConditionTypes m_Condition = ConditionTypes.Equal;
        public ConditionTypes Condition
        {
            get {
                return m_Condition;
            }
        }
    }
}
