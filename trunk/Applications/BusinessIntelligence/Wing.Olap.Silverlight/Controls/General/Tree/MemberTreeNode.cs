/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using System.Windows.Media;
using Wing.Olap.Controls.MemberChoice.Info;
using Wing.Olap.Core.Data;

namespace Wing.Olap.Controls.General.Tree
{
    public class MemberTreeNode : CustomTreeNode
    {
        OlapMemberInfo m_MemberInfo = null;
        public OlapMemberInfo MemberInfo
        {
            get {
                return m_MemberInfo;
            }
        }

        public MemberTreeNode(OlapMemberInfo info, bool useMultiSelect)
            : base(useMultiSelect)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            m_MemberInfo = info;

            if (info.Info != null && info.Info != null)
                Text = MemberInfo.Info.Caption;
            else
                Text = String.Empty;

            // В случае множ. выбора клик на иконке используем для изменения состояния
            m_ItemCtrl.IconClick += new EventHandler(item_ctrl_IconClick);

            if (useMultiSelect)
            {
                UpdateNodeIcon();
            }

            info.StateChanged += new OlapMemberInfo.StateChangedEventHandler(info_StateChanged);
        }

        MemberVisualizationTypes m_MemberVisualizationType = MemberVisualizationTypes.Caption;
        public MemberVisualizationTypes MemberVisualizationType
        {
            get { return m_MemberVisualizationType; }
            set
            {
                m_MemberVisualizationType = value;
                // Определяем что именно нужно светить в контроле
                if (MemberInfo.Info != null && MemberInfo.Info != null)
                    Text = MemberInfo.Info.GetText(m_MemberVisualizationType);
                else
                    Text = String.Empty;
            }
        }

        void info_StateChanged(OlapMemberInfo sender)
        {
            UpdateNodeIcon();
        }

        void item_ctrl_IconClick(object sender, EventArgs e)
        {
            //Переводим в следующее состояние
            MemberInfo.SetNextState();

            //Генерим событие "Изменилось стоятояние выбранности"
            //Raise_SelectionStateChanged();
        }

        private void UpdateNodeIcon()
        {
            TreeItemControl item_ctrl = Header as TreeItemControl;
            if (item_ctrl != null)
            {
                item_ctrl.Icon = MemberChoiceControl.GetIconImage(MemberInfo);
            }
        }

        public bool IsPreloaded
        {
            set {
                if (value)
                {
                    m_ItemCtrl.ItemText.FontStyle = FontStyles.Italic;
                    m_ItemCtrl.ItemText.Foreground = new SolidColorBrush(Colors.DarkGray);
                }
                else
                {
                    m_ItemCtrl.ItemText.FontStyle = FontStyles.Normal;
                    m_ItemCtrl.ItemText.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
            get {
                if (m_ItemCtrl.ItemText.FontStyle != FontStyles.Normal)
                    return true;
                return false;
            }
        }
    }
}
