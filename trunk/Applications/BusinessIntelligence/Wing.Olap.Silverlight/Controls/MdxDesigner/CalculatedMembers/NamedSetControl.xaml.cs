using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Wing.Olap.Controls.General;

namespace Wing.Olap.Controls.MdxDesigner.CalculatedMembers
{
    public partial class NamedSetControl : UserControl
    {
        public NamedSetControl()
        {
            InitializeComponent();

            lblName.Text = Localization.CalcMemberEditor_NameLabel;
            txtExpression.Text = Localization.CalcMemberEditor_ExpressionLabel;

            txtExpression.TextChanged += new TextChangedEventHandler(txtScript_TextChanged);
            txtName.KeyDown += new KeyEventHandler(txtName_KeyDown);
        }

        void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Raise_EditEnd();
            }
        }

        /// <summary>
        /// Редактирование начато
        /// </summary>
        public event EventHandler EditStart;
        void Raise_EditStart()
        {
            EventHandler handler = EditStart;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Редактирование закончено
        /// </summary>
        public event EventHandler EditEnd;
        void Raise_EditEnd()
        {
            EventHandler handler = EditEnd;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public void HighlightDrop(Point point)
        {
            if (IsEnabled)
            {
                Rect expression_Bounds = AgControlBase.GetSLBounds(txtExpression);
                if (expression_Bounds.Contains(point))
                {
                    txtExpression.Effect = new System.Windows.Media.Effects.DropShadowEffect() { ShadowDepth = 1, Opacity = 0.8, Color = Colors.Blue };
                }
                else
                {
                    txtExpression.Effect = null;
                }

                Rect name_Bounds = AgControlBase.GetSLBounds(txtName);
                if (name_Bounds.Contains(point))
                {
                    txtName.Effect = new System.Windows.Media.Effects.DropShadowEffect() { ShadowDepth = 1, Opacity = 0.8, Color = Colors.Blue };
                }
                else
                {
                    txtName.Effect = null;
                }
            }
            else
            {
                txtName.Effect = null;
                txtExpression.Effect = null;
            }
        }

        public new void Drop(Point point, String str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                if (CanDropToName(point))
                {
                    txtName.Text += " " + str;
                    txtName.Effect = null;
                }
                if (CanDropToExpression(point))
                {
                    txtExpression.Text += " " + str;
                    txtExpression.Effect = null;
                }
            }
        }

        bool CanDropToName(Point point)
        {
            if (IsEnabled)
            {
                Rect name_Bounds = AgControlBase.GetSLBounds(txtName);
                if (name_Bounds.Contains(point))
                {
                    return true;
                }
            }
            return false;
        }

        bool CanDropToExpression(Point point)
        {
            if (IsEnabled)
            {
                Rect expression_Bounds = AgControlBase.GetSLBounds(txtExpression);
                if (expression_Bounds.Contains(point))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CanDrop(Point point)
        {
            return CanDropToName(point) | CanDropToExpression(point);
        }

        void txtScript_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (m_Set != null)
                m_Set.Expression = txtExpression.Text;
        }

        CalculatedNamedSetInfo m_Set = null;
        public CalculatedNamedSetInfo Set
        {
            get { return m_Set; }
        }

        public String NameText
        {
            get { return txtName.Text; }
        }

        public void Initialize(CalculatedNamedSetInfo info)
        {
            this.IsEnabled = info != null;

            m_Set = info;
            txtName.Text = info != null ? info.Name : String.Empty;
            txtExpression.Text = info != null ? info.Expression : String.Empty;
        }
    }
}
