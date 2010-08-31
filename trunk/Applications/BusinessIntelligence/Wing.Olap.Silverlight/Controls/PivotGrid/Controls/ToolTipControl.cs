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
using System.Collections.Generic;
using Wing.Olap.Core.Providers;
using System.Text;

namespace Wing.Olap.Controls.PivotGrid.Controls
{
    public class ToolTipControl : UserControl
    {
        StackPanel LayoutRoot;
        TextBlock m_Caption;
        TextBlock m_Text;

        public ToolTipControl()
        {
            LayoutRoot = new StackPanel();
            LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 225));

            m_Caption = new TextBlock() { FontWeight = FontWeights.Bold, Margin = new Thickness(10, 3, 3, 3) };
            m_Text = new TextBlock() { TextWrapping = TextWrapping.NoWrap, Margin = new Thickness(10, 3, 3, 3) };

            LayoutRoot.Children.Add(m_Caption);
            LayoutRoot.Children.Add(m_Text);
            this.Content = LayoutRoot;
        }

        public void Initialize(PivotGridItem item)
        {
            if (item is MemberControl)
            {
                Initialize(item as MemberControl);
                return;
            }
            if (item is CellControl)
            {
                Initialize(item as CellControl);
                return;
            }
            Caption = String.Empty;
            Text = String.Empty;
        }

        public void Initialize(CellControl cell)
        {
            Text = String.Empty;
            if (cell != null && cell.Cell != null)
            {
                if (cell.NotRecalculatedChange != null && cell.NotRecalculatedChange.HasError)
                {
                    Caption = cell.NotRecalculatedChange.Error;
                }
                else
                {
                    if (cell.Cell.CellDescr != null && cell.Cell.CellDescr.Value != null && cell.Cell.CellDescr.Value.Value != null)
                    {
                        Caption = cell.Cell.CellDescr.Value.DisplayValue;
                    }
                    else
                    {
                        Caption = "(null)";
                    }
                }

                Text += cell.Cell.GetShortTupleToStr();
            }
            else
            {
                Caption = String.Empty;
                Text = String.Empty;
            }
        }

        public void Initialize(MemberControl member)
        {
            if (member != null && member.Member != null)
            {
                // Текст подсказки
                IList<MemberInfo> anc = member.Member.GetAncestors();
                StringBuilder sb = new StringBuilder();
                foreach (MemberInfo mv in anc)
                {
                    sb.AppendLine(mv.Caption + " : " + mv.UniqueName);
                }
                String tupleToStr = sb.ToString();
                tupleToStr = tupleToStr.TrimEnd('\n');
                tupleToStr = tupleToStr.TrimEnd('\r');

                Caption = member.Member.Caption;
                Text = tupleToStr;
            }
            else
            {
                Caption = String.Empty;
                Text = String.Empty;
            }
        }

        public String Caption
        {
            get {
                return m_Caption.Text;
            }
            set {
                m_Caption.Text = value;
            }
        }

        public String Text
        {
            get
            {
                return m_Text.Text;
            }
            set
            {
                m_Text.Text = value;
            }
        }
    }
}
