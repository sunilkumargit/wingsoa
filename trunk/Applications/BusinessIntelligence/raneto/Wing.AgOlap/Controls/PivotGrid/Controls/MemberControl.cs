/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
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
using Ranet.AgOlap.Controls.PivotGrid.Data;
using System.Windows.Browser;
using System.Collections.Generic;
using System.Text;
using Ranet.AgOlap.Controls.General;
using Ranet.Olap.Core.Providers;
using Ranet.AgOlap.Controls.ContextMenu;
using Ranet.Olap.Core.Providers.ClientServer;
using System.Windows.Media.Imaging;
using Ranet.Olap.Core.Data;
using Ranet.AgOlap.Providers;

namespace Ranet.AgOlap.Controls.PivotGrid.Controls
{
    public class MemberActionEventArgs : EventArgs
    {
        public MemberActionEventArgs(
            int axisNum,
            MemberInfo member,
            MemberActionType action)
        {
            this.Axis = axisNum;
            this.Member = member;
            this.Action = action;
        }

        public readonly int Axis = 0;
        public readonly MemberInfo Member = null;
        public MemberActionType Action = MemberActionType.Expand;
    }

    public class MemberClickEventArgs : EventArgs
    {
        public MemberClickEventArgs(
            MemberInfo member)
        {
            this.Member = member;
        }

        public MemberClickEventArgs(
            MemberInfo member, Point point)
            : this(member)
        {
            this.Position = point;
        }

        public readonly MemberInfo Member = null;
        public readonly Point Position = default(Point);
        public CustomContextMenu ContextMenu;
    }

    public abstract class MemberControl : PivotGridItem
    {
        MemberInfo m_Member = null;
        public MemberInfo Member
        {
            get { return m_Member; }
        }

        protected virtual bool IsInteractive
        {
            get { return true; }
        }

        protected virtual bool UseHint
        {
            get { return true; }
        }

        /// <summary>
        /// Признак возможности использования команд "+" и "-" для элемента
        /// </summary>
        public readonly bool UseExpandingCommands = false;

        public MemberControl(PivotGridControl owner, MemberInfo info)
            : base(owner)
        {
            BorderThickness = new Thickness(0, 0, 1, 1);

            DefaultStyleKey = typeof(MemberControl);
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Top;

            if(info == null)
                throw new ArgumentNullException("info");
            
            m_Member = info;

            m_LayoutRoot = new Grid();
            m_LayoutRoot.Margin = new Thickness(2, 2 * Scale, 0, 0);
            m_LayoutRoot.VerticalAlignment = VerticalAlignment.Top;
            m_LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            m_LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            ColumnDefinition column02 = new ColumnDefinition();
            column02.MaxWidth = 0; /* чтобы при сжимании иконка надвигалась на текст макс. ширину будем далее задавать жестко*/
            m_LayoutRoot.ColumnDefinitions.Add(column02);
            m_LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            // Колонка для отображения режима сортировки
            m_LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            if (IsInteractive)
            {
                if (Member.ChildCount > 0 && !Member.IsCalculated && !Member.IsDublicate)
                {
                    var expander = new PlusMinusButton();
                    if (Member.DrilledDown)
                        expander.IsExpanded = true;
                    expander.CheckedChanged += new EventHandler(expander_CheckedChanged);
                    UseExpandingCommands = true;
                    expander.Height = expander.Width = Math.Max(5, 9 * Scale);
                    m_LayoutRoot.Children.Add(expander);
                }
                else
                {
                    ListMemberControl ctrl = new ListMemberControl();
                    ctrl.Opacity = 0.35;
                    ctrl.Height = ctrl.Width = Math.Max(5, 9 * Scale);
                    m_LayoutRoot.Children.Add(ctrl);
                }
            }

            // Название элемента
            m_LayoutRoot.Children.Add(CaptionText);
            Grid.SetColumn(CaptionText, 1);

            // Визуализация DATAMEMBER, UNKNOWNMEMBER,CUSTOM_ROLLUP и UNARY_OPERATOR
            if (Member != null)
            {
                BitmapImage custom_image = null;

                if (Member.UniqueName.Trim().EndsWith("DATAMEMBER"))
                {
                    custom_image = UriResources.Images.DataMember16;
                }

                if (Member.UniqueName.Trim().EndsWith("UNKNOWNMEMBER"))
                {
                    custom_image = UriResources.Images.UnknownMember16;
                }

                // CUSTOM_ROLLUP отображается своей иконкой.
                // UNARY_OPERATOR - каждый своей иконкой.
                // Если оба свойства установлены, то отображаем скомбинированную иконку
                if(String.IsNullOrEmpty(Member.Unary_Operator))
                {
                    // Только CUSTOM_ROLLUP
                    if (Member.HasCustomRollup)
                    {
                        custom_image = UriResources.Images.CalcFunction16;
                    }
                }
                else
                {
                    // UNARY_OPERATOR
                    if (custom_image == null && Member.Unary_Operator.Trim() == "+")
                    {
                        if (Member.HasCustomRollup)
                        {
                            custom_image = UriResources.Images.CalcFunctionPlus16;
                        }
                        else
                        {
                            custom_image = UriResources.Images.CalcPlus16;
                        }
                    }
                    if (custom_image == null && Member.Unary_Operator.Trim() == "-")
                    {
                        if (Member.HasCustomRollup)
                        {
                            custom_image = UriResources.Images.CalcFunctionMinus16;
                        }
                        else
                        {
                            custom_image = UriResources.Images.CalcMinus16;
                        }
                    }
                    if (custom_image == null && Member.Unary_Operator.Trim() == "*")
                    {
                        if (Member.HasCustomRollup)
                        {
                            custom_image = UriResources.Images.CalcFunctionMultiply16;
                        }
                        else
                        {
                            custom_image = UriResources.Images.CalcMultiply16;
                        }
                    }
                    if (custom_image == null && Member.Unary_Operator.Trim() == "/")
                    {
                        if (Member.HasCustomRollup)
                        {
                            custom_image = UriResources.Images.CalcFunctionDivide16;
                        }
                        else
                        {
                            custom_image = UriResources.Images.CalcDivide16;
                        }
                    }
                    if (custom_image == null && Member.Unary_Operator.Trim() == "~")
                    {
                        if (Member.HasCustomRollup)
                        {
                            custom_image = UriResources.Images.CalcFunctionTilde16;
                        }
                        else
                        {
                            custom_image = UriResources.Images.CalcTilde16;
                        }
                    }
                    if (custom_image == null && Member.Unary_Operator.Trim() == "=")
                    {
                        if (Member.HasCustomRollup)
                        {
                            custom_image = UriResources.Images.CalcFunctionEqual16;
                        }
                        else
                        {
                            custom_image = UriResources.Images.CalcEqual16;
                        }
                    }
                    if (custom_image == null)
                    {
                        if (Member.HasCustomRollup)
                        {
                            custom_image = UriResources.Images.CalcFunctionPercent16;
                        }
                        else
                        {
                            custom_image = UriResources.Images.CalcPercent16;
                        }
                    }
                }

                if (custom_image != null)
                {
                    Image image1 = new Image() { Margin = new Thickness(0, 0, 2, 0) };
                    image1.Width = Math.Max(8, 16 * Scale);
                    image1.Height = Math.Max(8, 16 * Scale);
                    image1.Source = custom_image;
                    m_LayoutRoot.Children.Add(image1);
                    Grid.SetColumn(image1, 3);
                }
            }

            m_EllipsisText = new TextBlock() { Text = "..." };
            m_EllipsisText.FontSize = Owner.DefaultFontSize * Scale;
            m_LayoutRoot.Children.Add(m_EllipsisText);
            m_EllipsisText.Margin = new Thickness(-1, 0, 0, 0);
            m_EllipsisText.TextAlignment = TextAlignment.Left;
            m_EllipsisText.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(m_EllipsisText, 2);
            m_EllipsisText.Visibility = Visibility.Collapsed;

            this.SizeChanged += new SizeChangedEventHandler(MemberControl_SizeChanged);
            this.Content = m_LayoutRoot;
            this.Click += new RoutedEventHandler(MemberControl_Click);

            m_SortByValueImage = new Image() { Width = 16, Height = 16 };
            m_SortByValueImage.Visibility = Visibility.Collapsed;
            //var m_SortSelector = new SortTypeSelector() { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            m_LayoutRoot.Children.Add(m_SortByValueImage);
            Grid.SetColumn(m_SortByValueImage, 4); 
        }

        Image m_SortByValueImage;

        protected virtual BitmapImage SortAsc_Image
        {
            get { return UriResources.Images.ColumnSortAsc16; }
        }

        protected virtual BitmapImage SortDesc_Image
        {
            get { return UriResources.Images.ColumnSortDesc16; }
        }

        SortTypes m_SortByValueType = SortTypes.None;
        public SortTypes SortByValueType
        {
            get { return m_SortByValueType; }
            set
            {
                m_SortByValueType = value;
                switch (value)
                {
                    case SortTypes.None:
                        m_SortByValueImage.Visibility = Visibility.Collapsed;
                        break;
                    case SortTypes.Ascending:
                        m_SortByValueImage.Visibility = Visibility.Visible;
                        m_SortByValueImage.Source = SortAsc_Image;
                        break;
                    case SortTypes.Descending:
                        m_SortByValueImage.Visibility = Visibility.Visible;
                        m_SortByValueImage.Source = SortDesc_Image;
                        break;
                }
            }
        }

        void MemberControl_Click(object sender, RoutedEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.None)
            {
                Raise_ExecuteMemberAction(MemberActionType.Default);
            }
        }
        
        void expander_CheckedChanged(object sender, EventArgs e)
        {
            MemberActionType action = MemberActionType.Expand;
            if (Member.DrilledDown)
                action = MemberActionType.Collapse;

            PlusMinusButton expander = sender as PlusMinusButton;
            if (expander != null)
            {
                expander.CheckedChanged -= new EventHandler(expander_CheckedChanged);
                expander.IsChecked = new bool?(!expander.IsChecked.Value);
                expander.CheckedChanged += new EventHandler(expander_CheckedChanged);
            }

            Raise_ExecuteMemberAction(action);
        }

        TextBlock m_EllipsisText = null;

        void MemberControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.CaptionText.ActualWidth > (this.CaptionText.DesiredSize.Width + m_LayoutRoot.ColumnDefinitions[2].MaxWidth))
            {
                m_EllipsisText.Visibility = Visibility.Visible;
                m_LayoutRoot.ColumnDefinitions[2].MaxWidth = 12 * Scale;
            }
            else
            {
                m_EllipsisText.Visibility = Visibility.Collapsed;
                m_LayoutRoot.ColumnDefinitions[2].MaxWidth = 0;
            }
        }

        public void RotateCaption(bool rotate)
        {
            if (rotate)
                m_LayoutRoot.Visibility = Visibility.Collapsed;
            else
                m_LayoutRoot.Visibility = Visibility.Visible;
            //RootPanel.RenderTransform = new RotateTransform() { Angle = -90  /*CenterY = -20*/ };
            //RootPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            //RootPanel.VerticalAlignment = VerticalAlignment.Stretch;
            //RootPanel.Orientation = Orientation.Vertical;
            //RootPanel.Children.Clear();

            //m_CaptionText.RenderTransform = new RotateTransform() { Angle = -90  /*CenterY = -20*/ };
            //RootPanel.Children.Add(m_CaptionText);

        }

        TextBlock m_CaptionText = null;
        Run m_Run0;
        Run m_Run1;
        public TextBlock CaptionText
        {
            get
            {
                if (m_CaptionText == null)
                {
                    m_CaptionText = new TextBlock();
                    m_CaptionText.Margin = new Thickness(2, 0, 3, 0);
                    m_CaptionText.TextAlignment = TextAlignment.Left;
                    m_CaptionText.VerticalAlignment = VerticalAlignment.Center;
                    m_CaptionText.HorizontalAlignment = HorizontalAlignment.Stretch;

                    m_Run0 = new Run();
                    m_Run0.FontWeight = FontWeights.Bold;
                    m_Run1 = new Run();

                    m_CaptionText.Inlines.Add(m_Run0);
                    m_CaptionText.Inlines.Add(m_Run1);
                }

                // Определяем сортированы ли элементы данной иерархии по свойству
                SortTypes sortType = SortTypes.None;
                // Тип визуализации заголовка
                MemberVisualizationTypes memberVisualizationType = MemberVisualizationTypes.Caption;
                if (Owner != null)
                {
                    memberVisualizationType = Owner.MemberVisualizationType;
                    SortDescriptor sortDescr = Owner.GetAxisPropertySort(this);
                    if(sortDescr != null)
                        sortType = sortDescr.Type;
                }

                String text = Member.GetText(memberVisualizationType);
                m_Run0.Text = String.Empty;
                m_Run1.Text = String.Empty;
                if (!String.IsNullOrEmpty(text))
                {
                    switch (sortType)
                    {
                        case SortTypes.None:
                            m_Run0.Text = String.Empty;
                            m_Run1.Text = text;
                            break;
                        case SortTypes.Ascending:
                            m_Run0.Foreground = new SolidColorBrush(Colors.Blue);
                            m_Run0.Text = text.Length > 0 ? text.Substring(0, 1) : String.Empty;
                            m_Run1.Text = text.Length > 1 ? text.Substring(1, text.Length - 1) : String.Empty;
                            break;
                        case SortTypes.Descending:
                            m_Run0.Foreground = new SolidColorBrush(Colors.Red);
                            m_Run0.Text = text.Length > 0 ? text.Substring(0, 1) : String.Empty;
                            m_Run1.Text = text.Length > 1 ? text.Substring(1, text.Length - 1) : String.Empty;
                            break;
                    }
                }

                double font_Scaled = Owner.DefaultFontSize * Scale;
                m_CaptionText.FontSize = font_Scaled;
                if (m_EllipsisText != null)
                {
                    m_EllipsisText.FontSize = font_Scaled;
                }
                return m_CaptionText;
            }
        }

        Grid m_LayoutRoot;
       
        #region События
        public event MemberActionEventHandler ExecuteMemberAction;
        public void Raise_ExecuteMemberAction(MemberActionType action)
        {
            MemberActionEventHandler handler = this.ExecuteMemberAction;
            if (handler != null)
            {
                if(this is ColumnMemberControl)
                    handler(this, new MemberActionEventArgs(0, this.Member, action));
                if (this is RowMemberControl)
                    handler(this, new MemberActionEventArgs(1, this.Member, action));
            }
        }
        #endregion События
    }
}
