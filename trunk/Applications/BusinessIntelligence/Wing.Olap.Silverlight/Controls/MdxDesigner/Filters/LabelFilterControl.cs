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
using Wing.Olap.Controls.MdxDesigner.Wrappers;
using Wing.Olap.Controls.General;

namespace Wing.Olap.Controls.MdxDesigner.Filters
{
    public class LabelFilterControl : FilterControlBase
    {
        LevelPropertyCombo comboLevelProperty;
        LabelFilterTypeCombo comboFilterType;
        TextBox text_1;
        TextBlock lblAnd;
        TextBox text_2;

        public LabelFilterControl()
        {
            Grid LayoutRoot = new Grid();

            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());

            LayoutRoot.RowDefinitions.Add(new RowDefinition());
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(24) });
            LayoutRoot.RowDefinitions.Add(new RowDefinition());

            // Мера куба
            comboLevelProperty = new LevelPropertyCombo() { Margin = new Thickness(0) };
            LayoutRoot.Children.Add(comboLevelProperty);
            Grid.SetColumn(comboLevelProperty, 0);
            Grid.SetRow(comboLevelProperty, 1);

            // Тип фильтра
            comboFilterType = new LabelFilterTypeCombo() { Margin = new Thickness(5, 0, 0, 0) };
            comboFilterType.SelectionChanged += new SelectionChangedEventHandler(comboFilterType_SelectionChanged);
            LayoutRoot.Children.Add(comboFilterType);
            Grid.SetRow(comboFilterType, 1);
            Grid.SetColumn(comboFilterType, 1);

            // Текстовое поле
            text_1 = new Wing.Olap.Controls.General.RichTextBox() { Margin = new Thickness(5,0,0,0), Text = String.Empty };
            LayoutRoot.Children.Add(text_1);
            Grid.SetRow(text_1, 1);
            Grid.SetColumn(text_1, 2);

            // Текст "и"
            lblAnd = new TextBlock() { Text = Localization.Filter_Label_And, TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Bottom, Margin = new Thickness(5, 0, 0, 0) };
            LayoutRoot.Children.Add(lblAnd);
            Grid.SetColumn(lblAnd, 3);
            Grid.SetRow(lblAnd, 1);

            // Текстовое поле
            text_2 = new Wing.Olap.Controls.General.RichTextBox() { Margin = new Thickness(5, 0, 0, 0), Text = String.Empty };
            LayoutRoot.Children.Add(text_2);
            Grid.SetRow(text_2, 1);
            Grid.SetColumn(text_2, 4);

            this.Content = LayoutRoot;

            Refresh();
        }

        void Refresh()
        {
            if (comboFilterType.CurrentType == LabelFilterTypes.Between ||
                comboFilterType.CurrentType == LabelFilterTypes.NotBetween)
            {
                Grid.SetColumnSpan(text_1, 1);
                lblAnd.Visibility = Visibility.Visible;
                text_2.Visibility = Visibility.Visible;
            }
            else
            {
                Grid.SetColumnSpan(text_1, 3);
                lblAnd.Visibility = Visibility.Collapsed;
                text_2.Visibility = Visibility.Collapsed;
            }
        }

        void comboFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }

        Label_FilterWrapper m_Filter = new Label_FilterWrapper();
        public Label_FilterWrapper Filter
        {
            get
            {
                m_Filter.FilterType = comboFilterType.CurrentType;
                m_Filter.Text1 = text_1.Text;
                m_Filter.Text2 = text_2.Text;
                if (comboLevelProperty.CurrentProperty != null)
                    m_Filter.LevelPropertyName = comboLevelProperty.CurrentProperty;
                else
                    m_Filter.LevelPropertyName = String.Empty;
                return m_Filter;
            }
        }

        //public void Initialize(CubeDefInfo cubeInfo)
        //{
        //    comboMeasure.Initialize(cubeInfo);
        //}

        //public void Initialize(CubeDefInfo cubeInfo, Value_FilterWrapper wrapper)
        //{
        //    comboMeasure.Initialize(cubeInfo);
        //    Initialize(wrapper);
        //}

        public void Initialize(Label_FilterWrapper wrapper)
        {
            if (wrapper != null)
            {
                comboFilterType.SelectItem(wrapper.FilterType);
                text_1.Text = wrapper.Text1;
                text_2.Text = wrapper.Text2;
                comboLevelProperty.SelectItem(wrapper.LevelPropertyName);
            }
        }
    }
}
