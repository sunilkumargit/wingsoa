using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Wing.Olap.Controls.Combo;
using Wing.Olap.Controls.General;
using Wing.Olap.Controls.General.ItemControls;
using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.MdxDesigner.CalculatedMembers
{
    public partial class CalcMemberControl : UserControl
    {
        IList<String> m_FormatStrings = null;
        internal IList<String> FormatStrings
        {
            get { return m_FormatStrings; }
            set
            {
                m_FormatStrings = value;
                // Инициализируем список строк форматирования
                comboFormatString.Initialize(value);
            }
        }

        public CalcMemberControl()
        {
            InitializeComponent();

            lblName.Text = lblName1.Text = Localization.CalcMemberEditor_NameLabel;
            lblScript.Text = Localization.CalcMemberEditor_ScriptLabel;
            lblExpression.Text = Localization.CalcMemberEditor_ExpressionLabel;
            tabMemberTab.Header = Localization.CalcMemberEditor_MemberTab;
            tabScriptTab.Header = Localization.CalcMemberEditor_ScriptTab;
            lblNonEmptyBehavior.Text = Localization.CalcMemberEditor_NonEmpty;
            lblFormatString.Text = Localization.CalcMemberEditor_FormatStringLabel;
            lblBackColor.Text = Localization.CalcMemberEditor_BackColorLabel;
            lblForeColor.Text = Localization.CalcMemberEditor_ForeColorLabel;

            comboFormatString.SelectionChanged += new EventHandler(comboFormatString_SelectionChanged);
            comboFormatString.EditStart += new EventHandler(comboBox_EditStart);
            comboFormatString.EditEnd += new EventHandler(FormatString_EditEnd);

            txtName.KeyDown += new KeyEventHandler(txtName_KeyDown);
            txtExpression.TextChanged += new TextChangedEventHandler(txtExpression_TextChanged);

            TabCtrl.SelectionChanged += new SelectionChangedEventHandler(TabCtrl_SelectionChanged);

            comboNonEmptyBehavior.ItemsSource = m_NonEmptyBehavoirSource;
            comboNonEmptyBehavior.DropDownOpened += new EventHandler(comboBox_EditStart);
            comboNonEmptyBehavior.DropDownClosed += new EventHandler(comboNonEmptyBehavior_DropDownClosed);

            comboBackColor.ColorsComboBox.DropDownOpened += new EventHandler(comboBox_EditStart);
            comboBackColor.ColorsComboBox.DropDownClosed += new EventHandler(BackColor_ColorsComboBox_DropDownClosed);

            comboForeColor.ColorsComboBox.DropDownOpened += new EventHandler(comboBox_EditStart);
            comboForeColor.ColorsComboBox.DropDownClosed += new EventHandler(ForeColor_ColorsComboBox_DropDownClosed);
        }

        void ForeColor_ColorsComboBox_DropDownClosed(object sender, EventArgs e)
        {
            Raise_InnerEditEnd();
            Member.ForeColor = comboForeColor.CurrentObject != null ? comboForeColor.CurrentObject.ColorValue : Colors.Transparent;
        }

        void BackColor_ColorsComboBox_DropDownClosed(object sender, EventArgs e)
        {
            Raise_InnerEditEnd();
            Member.BackColor = comboBackColor.CurrentObject != null ? comboBackColor.CurrentObject.ColorValue : Colors.Transparent;
        }

        void comboNonEmptyBehavior_DropDownClosed(object sender, EventArgs e)
        {
            Raise_InnerEditEnd();
            Member.NonEmptyBehavior.Clear();
            foreach (var item in comboNonEmptyBehavior.SelectedItems)
            {
                if(!String.IsNullOrEmpty(item.Text))
                    Member.NonEmptyBehavior.Add(item.Text);
            }
        }

        List<ComboBoxItemData> m_NonEmptyBehavoirSource = new List<ComboBoxItemData>();

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

        void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Raise_EditEnd();
            }
        }

        void comboFormatString_SelectionChanged(object sender, EventArgs e)
        {
            if (m_Member != null)
            {
                m_Member.FormatString = String.Empty;

                ItemControlBase ctrl = comboFormatString.CurrentItem;
                if (ctrl != null && ctrl.Tag == null)
                    m_Member.FormatString = ctrl.Text;
            } 
        }

        void TabCtrl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabCtrl.SelectedItem == tabScriptTab)
            {
                RefreshScriptTab();
            }
        }

        void RefreshScriptTab()
        {
            txtName1.Text = m_Member != null ? m_Member.Name : String.Empty;
            txtScript.Text = m_Member != null ? m_Member.GetScript() : String.Empty;
        }

        void txtExpression_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (m_Member != null)
                m_Member.Expression = txtExpression.Text;
        }

        void FormatString_EditEnd(object sender, EventArgs e)
        {
            Raise_InnerEditEnd();
        }

        void comboBox_EditStart(object sender, EventArgs e)
        {
            Raise_InnerEditStart();
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

        /// <summary>
        /// Начато вложенное редактирование
        /// </summary>
        public event EventHandler InnerEditStart;
        void Raise_InnerEditStart()
        {
            EventHandler handler = InnerEditStart;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Вложенное редактирование закончено
        /// </summary>
        public event EventHandler InnerEditEnd;
        void Raise_InnerEditEnd()
        {
            EventHandler handler = InnerEditEnd;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        CalcMemberInfo m_Member = null;
        public CalcMemberInfo Member
        {
            get { return m_Member; }
        }

        public String NameText
        {
            get { return txtName.Text; }
        }

        public void InitializeMetadata(CubeDefInfo cubeInfo)
        {
            comboNonEmptyBehavior.ItemsSource = null;
            m_NonEmptyBehavoirSource.Clear();
            if (cubeInfo != null)
            {
                foreach (var info in cubeInfo.Measures)
                {
                    // Не выводим вычисляемые меры
                    if (String.IsNullOrEmpty(info.Expression))
                    {
                        m_NonEmptyBehavoirSource.Add(new ComboBoxItemData() { Text = info.Name });
                    }
                }
            }
            comboNonEmptyBehavior.ItemsSource = m_NonEmptyBehavoirSource;
        }
        
        public void Initialize(CalcMemberInfo info)
        {
            foreach (var item in m_NonEmptyBehavoirSource)
            {
                item.IsChecked = false;
            }
            if (info != null)
            {
                foreach (var nonEmpty in info.NonEmptyBehavior)
                {
                    foreach (var item in m_NonEmptyBehavoirSource)
                    {
                        if (item.Text == nonEmpty)
                        {
                            item.IsChecked = true;
                        }
                    }
                }
            }

            this.IsEnabled = info != null;

            m_Member = info;
            txtName.Text = info != null ? info.Name : String.Empty;
            txtExpression.Text = info != null ? info.Expression : String.Empty;

            // Ищем соответствующую строку форматирования
            comboFormatString.SelectItem(info != null ? info.FormatString : String.Empty);
            comboForeColor.SelectItem(info != null ? info.ForeColor : Colors.Transparent);
            comboBackColor.SelectItem(info != null ? info.BackColor : Colors.Transparent);

            if (TabCtrl.SelectedItem == tabScriptTab)
            {
                RefreshScriptTab();
            }
        }
    }
}
