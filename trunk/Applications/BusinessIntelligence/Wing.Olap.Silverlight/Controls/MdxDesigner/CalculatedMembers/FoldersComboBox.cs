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
using Ranet.AgOlap.Controls.General.ItemControls;
using System.Collections.Generic;
using Ranet.AgOlap.Controls.General;

namespace Ranet.AgOlap.Controls.MdxDesigner.CalculatedMembers
{
    public class FoldersComboBox : UserControl
    {
        const String NONE = "<NONE>";
        const String CREATE_NEW = "<CREATE_NEW>";

        ComboBoxEx m_ComboBox;
        TextBox m_TextBox;

        public FoldersComboBox()
        {
            Grid LayoutRoot = new Grid();
            Height = 22;

            m_ComboBox = new ComboBoxEx();
            LayoutRoot.Children.Add(m_ComboBox);

            m_TextBox = new TextBox();
            LayoutRoot.Children.Add(m_TextBox);
            m_TextBox.Visibility = Visibility.Collapsed;
            m_TextBox.KeyUp += new KeyEventHandler(m_TextBox_KeyUp);
            m_TextBox.LostFocus += new RoutedEventHandler(m_TextBox_LostFocus);

            m_ComboBox.Combo.Items.Add(new ItemControlBase(false) { Text = Localization.ComboBoxItem_None, Tag = NONE });
            m_ComboBox.Combo.Items.Add(new ItemControlBase(false) { Text = Localization.ComboBoxItem_New, Tag = CREATE_NEW });
            m_ComboBox.Combo.SelectedIndex = 0;

            m_ComboBox.SelectionChanged += new SelectionChangedEventHandler(ComboBox_SelectionChanged);

            this.Content = LayoutRoot;
        }

        void m_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (m_TextBox.Visibility == Visibility.Visible)
                EndEdit();
        }

        public event EventHandler EditStart;
        void Raise_EditStart()
        {
            EventHandler handler = EditStart;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
        public event EventHandler EditEnd;
        void Raise_EditEnd()
        {
            EventHandler handler = EditEnd;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        void m_TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                EndEdit();
                return;
            }
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                CancelEdit();
                return;
            }
        }

        void BeginEdit()
        {
            Raise_EditStart();
            m_ComboBox.Visibility = Visibility.Collapsed;
            m_TextBox.Visibility = Visibility.Visible;
            m_TextBox.Text = Localization.FolderComboBox_NewFolder;
            m_TextBox.Focus();
            m_TextBox.SelectionStart = m_TextBox.Text.Length;
        }

        void EndEdit()
        {
            if (!List.ContainsKey(m_TextBox.Text))
            {
                FolderInfo info = new FolderInfo(m_TextBox.Text, m_TextBox.Text, true);
                List.Add(info.Name, info);
                FolderItemControl itemControl = new FolderItemControl(info);
                m_ComboBox.Combo.Items.Add(itemControl);
                m_ComboBox.Combo.SelectedItem = itemControl;
            }
            m_ComboBox.Visibility = Visibility.Visible;
            m_TextBox.Visibility = Visibility.Collapsed;
            m_ComboBox.Focus();

            Raise_EditEnd();
        }

        void CancelEdit()
        {
            Raise_EditEnd();
            m_ComboBox.Combo.SelectedIndex = 0;
            m_ComboBox.Visibility = Visibility.Visible;
            m_TextBox.Visibility = Visibility.Collapsed;
            m_ComboBox.Focus();
        }

        void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentItem != null && CurrentItem.Tag != null && CurrentItem.Tag.ToString() == CREATE_NEW)
            {
                BeginEdit();
            }
            else
            {
                Raise_SelectionChanged();
            }
        }

        public event EventHandler SelectionChanged;
        void Raise_SelectionChanged()
        {
            EventHandler handler = SelectionChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public ItemControlBase CurrentItem
        {
            get
            {
                ItemControlBase ctrl = m_ComboBox.Combo.SelectedItem as ItemControlBase;
                return ctrl;
            }
        }

        protected virtual ItemControlBase GetItemControl(String key, String value)
        {
            return new ItemControlBase(false) { Text = value };
        }

        Dictionary<String, FolderInfo> m_List;
        Dictionary<String, FolderInfo> List
        {
            get {
                if (m_List == null)
                    m_List = new Dictionary<string, FolderInfo>();
                return m_List;
            }
        }

        public void Initialize(Dictionary<String, FolderInfo> list)
        {
            Initialize(list, String.Empty);
        }

        public void Initialize(Dictionary<String, FolderInfo> list, String nameToSelect)
        {
            m_List = list;
            m_ComboBox.Clear();
            m_ComboBox.Combo.Items.Add(new ItemControlBase(false) { Text = Localization.ComboBoxItem_None, Tag = NONE });
            m_ComboBox.Combo.Items.Add(new ItemControlBase(false) { Text = Localization.ComboBoxItem_New, Tag = CREATE_NEW });

            foreach (FolderInfo item in List.Values)
            {
                m_ComboBox.Combo.Items.Add(new FolderItemControl(item));
            }

            SelectItem(nameToSelect);
        }

        public void SelectItem(String name)
        {
            int i = 0;
            foreach (ItemControlBase item in m_ComboBox.Combo.Items)
            {
                FolderItemControl ctrl = item as FolderItemControl;
                if(ctrl != null && ctrl.Text == name)
                {
                    m_ComboBox.Combo.SelectedIndex = i;
                    return;
                }
                i++;
            }

            if (m_ComboBox.Combo.Items.Count > 0)
                m_ComboBox.Combo.SelectedIndex = 0;
            else
                m_ComboBox.Combo.SelectedIndex = -1;
        }
    }
}
