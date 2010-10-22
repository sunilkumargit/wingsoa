/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows;
using Wing.Olap.Controls.PivotGrid.Controls;

namespace Wing.Olap.Controls.MdxDesigner
{
    // Не используется. Добавлен для проведения работ по формированию команд DrillDown, Expand, Collapse самим MDX дизайнером.
    public class DesignerPivotGridControl : UpdateablePivotGridControl
    {
        public DesignerPivotGridControl()
            : base()
        {
            this.Loaded += new RoutedEventHandler(DesignerPivotGridControl_Loaded);
        }

        #region События
        public event MemberActionEventHandler OnPerformMemberAction;
        protected void Raise_PerformMemberAction(MemberActionEventArgs args)
        {
            MemberActionEventHandler handler = this.OnPerformMemberAction;
            if (handler != null)
            {
                handler(this, args);
            }
        }
        #endregion События

        protected override void PerformMemberAction(MemberActionEventArgs e)
        {
            base.PerformMemberAction(e);
        }

        void DesignerPivotGridControl_Loaded(object sender, RoutedEventArgs e)
        {
            ToBeginButton.Visibility = Visibility.Collapsed;
            ToEndButton.Visibility = Visibility.Collapsed;
            BackButton.Visibility = Visibility.Collapsed;
            ForwardButton.Visibility = Visibility.Collapsed;
            NavigationToolBarSplitter.Visibility = Visibility.Collapsed;
        }
    }
}
