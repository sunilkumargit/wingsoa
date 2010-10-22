/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using System.Windows.Controls;
using Wing.Olap.Controls.General;
using Wing.Olap.Controls.MdxDesigner.Wrappers;
using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.MdxDesigner.Filters
{
    public class FilterBuilderControl : UserControl
    {
        MemberChoiceControl m_MemberChoice = null;
        FilterControl m_Filter = null;
        CheckedHeaderControl lblFilter;
        CheckedHeaderControl lblMemberChoice;
        CustomPanel filterPanel;
        Grid grdFilter_LayoutRoot;

        public FilterBuilderControl()
        {
            Grid grdLayoutRoot = new Grid();

            //LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            grdLayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grdLayoutRoot.RowDefinitions.Add(new RowDefinition());

            // Фильтр
            grdFilter_LayoutRoot = new Grid();
            grdFilter_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grdFilter_LayoutRoot.RowDefinitions.Add(new RowDefinition());

            // Заголовок
            //HeaderControl lblFilter = new HeaderControl(UriResources.Images.ChangeFilter16, Localization.FilterControl_Caption) { Margin = new Thickness(0, 0, 0, 3) };
            lblFilter = new CheckedHeaderControl(false, Localization.FilterControl_Caption) { Margin = new Thickness(0, 0, 0, 3) };
            lblFilter.CheckedChanged += new EventHandler(lblFilter_CheckedChanged);
            grdFilter_LayoutRoot.Children.Add(lblFilter);
            // Фильтр
            filterPanel = new CustomPanel();
            m_Filter = new FilterControl() { Margin = new Thickness(5)};
            filterPanel.Content = m_Filter;
            grdFilter_LayoutRoot.Children.Add(filterPanel);
            Grid.SetRow(filterPanel, 1);

            // Выбор элементов в Set
            Grid grdMemberChoice_LayoutRoot = new Grid();
            grdMemberChoice_LayoutRoot.Margin = new Thickness(0, 5, 0, 0);
            grdMemberChoice_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto } );
            grdMemberChoice_LayoutRoot.RowDefinitions.Add(new RowDefinition());
            
            // Заголовок
            //HeaderControl lblMemberChoice = new HeaderControl(UriResources.Images.MemberChoice16, Localization.MemberChoice_Caption) { Margin = new Thickness(0, 0, 0, 3) };
            lblMemberChoice = new CheckedHeaderControl(false, Localization.MemberChoice_Caption) { Margin = new Thickness(0, 0, 0, 3) };
            lblMemberChoice.CheckedChanged += new EventHandler(lblMemberChoice_CheckedChanged);
            grdMemberChoice_LayoutRoot.Children.Add(lblMemberChoice);
            // Выбор элементов измерения
            m_MemberChoice = new MemberChoiceControl();
            grdMemberChoice_LayoutRoot.Children.Add(m_MemberChoice);
            Grid.SetRow(m_MemberChoice, 1);

            grdLayoutRoot.Children.Add(grdFilter_LayoutRoot);
            Grid.SetRow(grdFilter_LayoutRoot, 0);
            grdLayoutRoot.Children.Add(grdMemberChoice_LayoutRoot);
            Grid.SetRow(grdMemberChoice_LayoutRoot, 1);
            this.Content = grdLayoutRoot;

            Refresh();
        }

        void lblMemberChoice_CheckedChanged(object sender, EventArgs e)
        {
            if (lblMemberChoice.IsChecked)
            {
                if (!MemberChoiceIsInitialized)
                {
                    m_MemberChoice.Initialize();
                    MemberChoiceIsInitialized = true;
                }
            }

            Refresh();
        }

        CubeDefInfo m_CubeInfo = null;
        public CubeDefInfo CubeInfo
        {
          get { return m_CubeInfo; }
          set { 
              m_CubeInfo = value;
              m_Filter.Initialize(m_CubeInfo);
          }
        }

        bool m_UseFilterControl = true;
        /// <summary>
        /// Использовать контрол для построения фильтра (Top, Value, Label). Этот контрол не нужен для элементов из области фильтров
        /// </summary>
        public bool UseFilterControl
        {
            get {
                return m_UseFilterControl;
            }
            set {
                m_UseFilterControl = value;
                Refresh();
            }
        }

        public bool MemberChoiceIsInitialized = false;

        public void Initialize(Composite_FilterWrapper filter)
        {
            m_MemberChoice.SelectedInfo = filter.MembersFilter.SelectedInfo;
            if (filter.MembersFilter.IsUsed)
            {
                if (!MemberChoiceIsInitialized)
                {
                    m_MemberChoice.Initialize();
                    MemberChoiceIsInitialized = true;
                }
            }
            m_Filter.Initialize(filter.Filter);

            lblFilter.IsChecked = filter.Filter.IsUsed;
            lblMemberChoice.IsChecked = filter.MembersFilter.IsUsed;

            Refresh();
        }

        void lblFilter_CheckedChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        void Refresh()
        {
            if (UseFilterControl)
                grdFilter_LayoutRoot.Visibility = Visibility.Visible;
            else
                grdFilter_LayoutRoot.Visibility = Visibility.Collapsed;

            filterPanel.IsEnabled = lblFilter.IsChecked;
            m_MemberChoice.IsEnabled = lblMemberChoice.IsChecked;
        }

        public MemberChoiceControl ChoiceControl
        {
            get
            {
                return m_MemberChoice;
            }
        }

        public Composite_FilterWrapper CompositeFilter
        {
            get { 
                Composite_FilterWrapper composite_wrapper = new Composite_FilterWrapper();
                composite_wrapper.Filter = m_Filter.Filter;
                composite_wrapper.Filter.IsUsed = lblFilter.IsChecked;

                Members_FilterWrapper members_wrapper = new Members_FilterWrapper();
                //String str = m_MemberChoice.SelectedSet.Trim();
                //if (str == "{}")
                //    str = String.Empty;
                members_wrapper.FilterSet = m_MemberChoice.SelectedSet;
                members_wrapper.SelectedInfo = m_MemberChoice.SelectedInfo;
                composite_wrapper.MembersFilter = members_wrapper;
                composite_wrapper.MembersFilter.IsUsed = lblMemberChoice.IsChecked;
                
                return composite_wrapper;
            }
        }
    }
}
