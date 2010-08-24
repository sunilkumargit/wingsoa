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
using Ranet.AgOlap.Controls.General;
using System.Windows.Browser;
using Ranet.AgOlap.Controls.PivotGrid.Controls;
using System.Collections.Generic;
using Ranet.Olap.Core.Providers;
using Ranet.AgOlap.Controls.PivotGrid.Conditions;
using Ranet.AgOlap.Providers;
using System.Windows.Media.Imaging;
using System.Globalization;

namespace Ranet.AgOlap.Controls.PivotGrid.Controls
{
    public class CellClickEventArgs : EventArgs
    {
        public CellClickEventArgs(
            CellControl cell)
        {
            this.Cell = cell;
        }

        public CellClickEventArgs(
            CellControl cell, Point point)
            : this(cell)
        {
            this.Position = point;
        }

        public readonly CellControl Cell = null;
        public readonly Point Position = default(Point);
    }

    public class CellControl : PivotGridItem
    {
        CellInfo m_Cell = null;
        public CellInfo Cell
        {
            get { return m_Cell; }
            set {
                if (m_Cell != value)
                {
                    m_Cell = value;
                    // Ищем условия, которым может соответствовать ячейка
                    CalculateCustomCellCondidtions();

                    ApplySettings();
                }
            }
        }

        //public readonly int RowIndex = -1;
        //public readonly int ColumnIndex = -1;

        Grid m_LayoutPanel;
        TextBlock m_Caption_Text;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ApplySettings();
        }


        public CellControl(PivotGridControl owner)
            : base(owner)
        {
            DefaultStyleKey = typeof(CellControl);
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;

            this.Click += new RoutedEventHandler(CellControl_Click);

            m_LayoutPanel = new Grid();
            m_LayoutPanel.Margin = new Thickness(0);
            m_LayoutPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            m_LayoutPanel.ColumnDefinitions.Add(new ColumnDefinition());
            m_LayoutPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            resDoubleClickTimer = new Storyboard();
            resDoubleClickTimer.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250));
            resDoubleClickTimer.Completed += new EventHandler(Storyboard_Completed);
            m_LayoutPanel.Resources.Add("resDoubleClickTimer", resDoubleClickTimer);

            // Текст
            m_Caption_Text = new TextBlock();
            m_Caption_Text.FontSize = Owner.DefaultFontSize * Scale;
            m_Caption_Text.HorizontalAlignment = HorizontalAlignment.Stretch;
            m_Caption_Text.VerticalAlignment = VerticalAlignment.Center;
            m_Caption_Text.TextAlignment = TextAlignment.Right;
            m_Caption_Text.Margin = new Thickness(0, 0, 3 * Scale, 0);
            m_LayoutPanel.Children.Add(m_Caption_Text);
            Grid.SetColumn(m_Caption_Text, 1);

            this.Content = m_LayoutPanel;

        }

        void CellControl_Click(object sender, RoutedEventArgs e)
        {
            m_ClickCount++;
            m_LastArgs = e;
            resDoubleClickTimer.Begin();
        }

        Image m_Image = null;
        ProgressBar m_ProgressBar = null;

        Storyboard resDoubleClickTimer;
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            if (m_ClickCount > 1)
            {
                this.OnMouseDoubleClick(m_LastArgs);
            }
            m_ClickCount = 0;
        }

        public event EventHandler MouseDoubleClick;
        protected void OnMouseDoubleClick(EventArgs e)
        {
            EventHandler handler = MouseDoubleClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        int m_ClickCount = 0;
        RoutedEventArgs m_LastArgs;

        void ApplySettings()
        {
//            CalculateCustomCellCondidtions();

            ApplyBorderSettings();

            ApplyBackgroundSettings();

            ApplyValueSettings();

//            ApplyHintSettings();
        }

        /// <summary>
        /// Условия, под которые попадает ячейка
        /// </summary>
        IList<CellCondition> m_CustomCellCondidtions = null;
        /// <summary>
        /// Суммарные свойства ячейки с учетом наложения нескольких условий
        /// </summary>
        CellAppearanceObject m_CustomCellAppearance = null;

        void CalculateCustomCellCondidtions()
        {
            m_CustomCellCondidtions = null;
            m_CustomCellAppearance = null;

            if (Cell != null)
            {
                if (Owner != null && Owner.CustomCellsConditions != null && Owner.CustomCellsConditions.Count > 0)
                {
                    // Поиск условий, которые могут быть наложены на данную ячейку. 
                    // Перебираем всех предков по области столбцов и по их уникальному имени ищем
                    IList<MemberInfo> column_ascendants = this.Cell.ColumnMember.GetAncestors();
                    // column_ascendants - коллекция предков снизу вверх
                    foreach (MemberInfo member in column_ascendants)
                    {
                        // Ищем объект, содержащий условия для данного элемента по уникальному имени элемента
                        CellConditionsDescriptor conditions_descr = null;
                        foreach (CellConditionsDescriptor descr in Owner.CustomCellsConditions)
                        {
                            if (descr.MemberUniqueName == member.UniqueName)
                            {
                                conditions_descr = descr;
                                break;
                            }
                        }

                        List<CellCondition> usedConditions = null;
                        // Отбор условий, которым удовлетворяет значение ячейки
                        if (conditions_descr != null)
                        {
                            if (Cell.CellDescr != null &&
                                Cell.CellDescr.Value != null)
                            {
                                    try
                                    {
                                        double value = Cell.CellDescr.Value.Value != null ? Convert.ToDouble(Cell.CellDescr.Value.Value) : double.NaN;
                                        usedConditions = CellConditionsDescriptor.TestToConditions(value, conditions_descr);
                                    }
                                    catch (System.InvalidCastException)
                                    {
                                    }
                            }
                        }

                        if (usedConditions != null)
                        {
                            //Условий может быть несколько и они могут накладываться, например:
                            //1) > 0 и цвет фона - красный. 
                            //2) > 0 и шрифт жирный
                            //Поэтому для храненния накопленных условий использую m_CustomCellAppearance и потом буду использовать эту информацию при рисовании ячейки

                            //Проходим по списку условий и применяем настройки для ячейки
                            foreach (CellCondition cond in usedConditions)
                            {
                                if (m_CustomCellAppearance == null)
                                    m_CustomCellAppearance = new CellAppearanceObject();

                                if (cond.Appearance.Options.IgnoreAllOptions)
                                    continue;

                                if (cond.Appearance.Options.UseAllOptions ||
                                    cond.Appearance.Options.UseBackColor)
                                {
                                    m_CustomCellAppearance.Options.UseBackColor = true;
                                    m_CustomCellAppearance.BackColor = cond.Appearance.BackColor;
                                    //painter.CellArgs.StyleAppearance.BackColor2 =
                                    //    cond.Appearance.BackColor2;
                                    //painter.CellArgs.StyleAppearance.GradientMode =
                                    //    cond.Appearance.GradientMode;
                                }
                                if (cond.Appearance.Options.UseAllOptions ||
                                    cond.Appearance.Options.UseBorderColor)
                                {
                                    m_CustomCellAppearance.Options.UseBorderColor = true;
                                    m_CustomCellAppearance.BorderColor = cond.Appearance.BorderColor;
                                }
                                //if (cond.Appearance.Options.UseAllOptions ||
                                //    cond.Appearance.Options.UseFont)
                                //{
                                //    if (cond.Appearance.Font != null)
                                //    {
                                //        painter.CellArgs.StyleAppearance.Options.UseFont
                                //            = true;
                                //        painter.CellArgs.StyleAppearance.Font =
                                //            cond.Appearance.Font;
                                //    }
                                //}
                                if (cond.Appearance.Options.UseAllOptions ||
                                    cond.Appearance.Options.UseForeColor)
                                {
                                    m_CustomCellAppearance.Options.UseForeColor = true;
                                    m_CustomCellAppearance.ForeColor = cond.Appearance.ForeColor;
                                }

                                if (cond.Appearance.Options.IgnoreAllOptions ||
                                    !cond.Appearance.Options.ShowValue)
                                {
                                    m_CustomCellAppearance.Options.ShowValue = false;
                                }

                                //if (cond.Appearance.Options.UseAllOptions ||
                                //    cond.Appearance.Options.UseTextOptions)
                                //{
                                //    painter.CellArgs.StyleAppearance.Options.
                                //        UseTextOptions = true;

                                //    painter.CellArgs.StyleAppearance.TextOptions.
                                //        HAlignment =
                                //        cond.Appearance.TextOptions.HAlignment;
                                //    painter.CellArgs.StyleAppearance.TextOptions.
                                //        VAlignment =
                                //        cond.Appearance.TextOptions.VAlignment;
                                //    painter.CellArgs.StyleAppearance.TextOptions.
                                //        WordWrap = cond.Appearance.TextOptions.WordWrap;
                                //    painter.CellArgs.StyleAppearance.TextOptions.
                                //        Trimming = cond.Appearance.TextOptions.Trimming;
                                //}

                                if (cond.Appearance.Options.UseAllOptions ||
                                    cond.Appearance.Options.UseImage)
                                {
                                    m_CustomCellAppearance.Options.UseImage = true;
                                    m_CustomCellAppearance.CustomImage = cond.Appearance.CustomImage;
                                    m_CustomCellAppearance.CustomImageUri = cond.Appearance.CustomImageUri;
                                }

                                if (cond.Appearance.Options.UseAllOptions ||
                                    cond.Appearance.Options.UseProgressBar)
                                {
                                    m_CustomCellAppearance.Options.UseProgressBar = true;
                                    m_CustomCellAppearance.ProgressBarOptions = cond.Appearance.ProgressBarOptions;
                                }
                            }
                        }
                    }
                }
            }
        }
        
        void ApplyBackgroundSettings()
        {
            if (Cell == null)
            {
                Background = new SolidColorBrush(Colors.Transparent);
                return;
            }

            if (IsSelected && !IsFocused)
            {
                Background = new SolidColorBrush(Color.FromArgb(50, 0, 0, 225));
            }
            else
            {
                // Если ячейка редактируемая
                if (Owner != null && Owner.CanEdit && Cell.IsUpdateable)
                {
                    if (NotRecalculatedChange != null)
                    {
                        // Непересчитанные ячейки выделяются фоном Colors.Cyan
                        Background = new SolidColorBrush(Color.FromArgb(75, Colors.Cyan.R, Colors.Cyan.G, Colors.Cyan.B));
                    }
                    else
                    {
                        // Редактируемые ячейки - SystemColors.Info
                        Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 225));
                    }
                }
                else
                {
                    if (m_CustomCellAppearance != null && m_CustomCellAppearance.Options.UseBackColor)
                    {
                        // Фон из настроек условий для ячейки
                        Background = new SolidColorBrush(m_CustomCellAppearance.BackColor);
                    }
                    else
                    {
                        if (Cell != null)
                        {
                            int backColor = Cell.CellDescr.BackColor;
                            if (backColor != int.MinValue)
                            {
                                // Фон из OLAP (свойства ячейки)
                                Color color = ColorHelper.FromRgb(backColor);
                                Background = new SolidColorBrush(color);
                            }
                            else
                            {
                                // Фон по-умолчанию
                                if (Owner != null)
                                {
                                    if(Background != Owner.CellsBackground)
                                        Background = Owner.CellsBackground;
                                }
                                else
                                {
                                    Background = new SolidColorBrush(Colors.White);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void UndoChanges()
        {
            m_NotRecalculatedChange = null;
            ApplySettings();
        }

        //ToolTipControl m_ToolTip;

        private void ApplyBorderSettings()
        {
            if (Cell == null)
            {
                BorderBrush = new SolidColorBrush(Colors.Transparent);
                return;
            }

            if (IsFocused)
            {
                BorderBrush = new SolidColorBrush(Colors.Black);
                BorderThickness = new Thickness(1, 1, 1, 1);
                // Чтобы не сместилось содержимое при появлении левой и верхней границы
                m_LayoutPanel.Margin = new Thickness(-1, -1, 0, 0);
            }
            else
            {
                if (m_CustomCellAppearance != null && m_CustomCellAppearance.Options.UseBorderColor)
                {
                    // Цвет из настроек условий для ячейки
                    BorderBrush = new SolidColorBrush(m_CustomCellAppearance.BorderColor);
                    // Рамка полностью
                    BorderThickness = new Thickness(1, 1, 1, 1);
                    // Чтобы не сместилось содержимое при появлении левой и верхней границы
                    m_LayoutPanel.Margin = new Thickness(-1, -1, 0, 0);
                }
                else
                {
                    if (Owner != null)
                    {
                        if (BorderBrush != Owner.CellsBorder)
                            BorderBrush = Owner.CellsBorder;
                    }
                    else
                    {
                        BorderBrush = new SolidColorBrush(Colors.DarkGray);
                    }

                    int left_Thicknes = ShowLeftBorder == true ? 1 : 0;
                    int up_Thicknes = ShowUpBorder == true ? 1 : 0;

                    BorderThickness = new Thickness(left_Thicknes, up_Thicknes, 1, 1);
                    m_LayoutPanel.Margin = new Thickness(left_Thicknes * -1, up_Thicknes * -1, 0, 0);
                }
            }
        }

        public String Text
        {
            get {
                return m_Caption_Text.Text;
            }
        }

        private void ApplyValueSettings()
        {
            if (Cell == null)
            {
                m_Caption_Text.Visibility = Visibility.Collapsed;
                if (m_ProgressBar != null)
                {
                    m_ProgressBar.Visibility = Visibility.Collapsed;
                }
                if (m_Image != null)
                {
                    m_Image.Visibility = Visibility.Collapsed;
                }
                return;
            }

            // Если в ячейке отображается только значение, то оно должно быть выравнено по правому краю
            // Если в ячейке отображается только картинка, то она должна быть выравнена по центру
            // Если в ячейке отображается картинка и значение, то картинка должна быть выравнена по левому краю, а значение по правому

            bool show_Value = true;
            bool show_Image = false;
            bool show_ProgressBar = false;
            if (m_CustomCellAppearance != null)
            {
                show_Value = m_CustomCellAppearance.Options.ShowValue;
                show_Image = m_CustomCellAppearance.Options.UseImage && !String.IsNullOrEmpty(m_CustomCellAppearance.CustomImageUri);
                show_ProgressBar = m_CustomCellAppearance.Options.UseProgressBar;
            }
            if (show_ProgressBar == true)
                show_Image = false;

            // Прогрессбар из условия
            if (show_ProgressBar)
            {
                if (m_ProgressBar == null)
                {
                    m_ProgressBar = new ProgressBar();
                    m_ProgressBar.VerticalAlignment = VerticalAlignment.Stretch;
                    m_ProgressBar.HorizontalAlignment = HorizontalAlignment.Stretch;
                    m_LayoutPanel.Children.Add(m_ProgressBar);
                    Grid.SetColumnSpan(m_ProgressBar, 3);
                    
                    // Текстовое поле поверх прогресса
                    m_LayoutPanel.Children.Remove(m_Caption_Text);
                    m_LayoutPanel.Children.Add(m_Caption_Text);
                    Grid.SetColumn(m_Caption_Text, 1);
                }

                m_ProgressBar.Minimum = m_CustomCellAppearance.ProgressBarOptions.MinValue;
                m_ProgressBar.Maximum = m_CustomCellAppearance.ProgressBarOptions.MaxValue;

                double value = 0;
                if (Cell.CellDescr != null &&
                    Cell.CellDescr.Value != null &&
                    Cell.CellDescr.Value.Value != null)
                {
                    try
                    {
                        value = Convert.ToDouble(Cell.CellDescr.Value.Value);
                    }
                    catch (System.InvalidCastException)
                    {
                    }
                }
                m_ProgressBar.Value = value;
                GradientStopCollection stops = new GradientStopCollection();
                stops.Add(new GradientStop() { Color = m_CustomCellAppearance.ProgressBarOptions.StartColor });
                stops.Add(new GradientStop() { Color = m_CustomCellAppearance.ProgressBarOptions.EndColor, Offset = 1 });
                LinearGradientBrush brush = new LinearGradientBrush(stops, 0);
                m_ProgressBar.Foreground = brush;

                m_ProgressBar.Visibility = Visibility.Visible;

                // В прогрессбаре текст отображаем в центре
                m_Caption_Text.TextAlignment = TextAlignment.Center;
            }
            else
            {
                if(m_ProgressBar != null)
                    m_ProgressBar.Visibility = Visibility.Collapsed;
            }

            // Если никаких условий не задано, либо в условии прописано отображать значение
            if (show_Value)
            {
                m_Caption_Text.Visibility = Visibility.Visible;
                m_Caption_Text.FontWeight = FontWeights.Normal;

                if (NotRecalculatedChange != null)
                {
                    // Пытаемся отобразить модифицированное значение в том же формате, в котором оно будет отображаться пользователю когда запишется в куб
                    // Модифицированное значение пытаемся преобразовать в число. Если преобразование успешное, то пытаемся применить строку форматирования
                    // В случае, если преобразование в число не получится, то выводим модифицированное значение просто в строку
                    String text = NotRecalculatedChange.NewValue;
                    if (Cell.CellDescr != null && !String.IsNullOrEmpty(Cell.CellDescr.FormatString))
                    {
                        // Проверяем чтобы в качестве разделителя был допутимый символ (чтобы значение можно было конвертировать).
                        String modif = text;
                        modif = modif.Replace(".", System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
                        modif = modif.Replace(",", System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);

                        try
                        {
                            double value = Convert.ToDouble(modif);
                            String str = value.ToString(Cell.CellDescr.FormatString, CultureInfo.InvariantCulture);
                            if (str != Cell.CellDescr.FormatString) // Для случаев Currency и т.д.
                                text = str;
                        }
                        catch
                        {
                        }
                    }
                    m_Caption_Text.Text = text;
                }
                else
                {
                    if (Cell.CellDescr != null && Cell.CellDescr.Value != null)
                    {
                        m_Caption_Text.Text = Cell.CellDescr.Value.DisplayValue;
                    }
                    else
                    {
                        m_Caption_Text.Text = String.Empty;
                    }
                }

                if (m_CustomCellAppearance != null && m_CustomCellAppearance.Options.UseForeColor)
                {
                    // Цвет из настроек условий для ячейки
                    m_Caption_Text.Foreground = new SolidColorBrush(m_CustomCellAppearance.ForeColor);
                }
                else
                {
                    Brush brush = new SolidColorBrush(Colors.Black);

                    if (RecalculatedChange != null)
                    {
                        m_Caption_Text.FontWeight = FontWeights.Bold;
                        brush = new SolidColorBrush(Colors.Blue);
                    }
                    else
                    {
                        if (NotRecalculatedChange == null && Cell.CellDescr != null)
                        {
                            int foreColor = Cell.CellDescr.ForeColor;
                            if (foreColor != int.MinValue)
                            {
                                // Цвет из OLAP (свойства ячейки)
                                Color color = ColorHelper.FromRgb(foreColor);
                                brush = new SolidColorBrush(color);
                            }
                        }
                    }
                    m_Caption_Text.Foreground = brush;
                }
            }
            else
            {
                m_Caption_Text.Visibility = Visibility.Collapsed;
            }

            BitmapImage image = null;
            if (m_CustomCellAppearance != null && !String.IsNullOrEmpty(m_CustomCellAppearance.CustomImageUri))
            {
                try
                {
                    image = new BitmapImage(new Uri(m_CustomCellAppearance.CustomImageUri, UriKind.Relative));
                }
                catch { }
            }

            // Если ошибка при обновлении ячейки
            if (NotRecalculatedChange != null && NotRecalculatedChange.HasError)
            {
                show_Image = true;
                image = UriResources.Images.ErrorSmall16;
            }

            // Картинка из условия
            if (show_Image && image != null)
            {
                if (m_Image == null)
                {
                    m_Image = new Image();
                    m_Image.Margin = new Thickness(2, 0, 2, 0);
                    m_Image.Width = m_Image.Height = 16;
                    m_Image.Stretch = Stretch.Uniform;
                    m_LayoutPanel.Children.Add(m_Image);
                    Grid.SetColumn(m_Image, 0);
                }

                m_Image.Visibility = Visibility.Visible;
                m_Image.Source = image;
                if (image != null)
                {
                    if (image.PixelWidth == 0 || image.PixelHeight == 0)
                    {
                        image.ImageOpened += new EventHandler<RoutedEventArgs>(image_ImageOpened);
                    }
                    else
                    {
                        m_Image.Width = image.PixelWidth * Scale;
                        m_Image.Height = image.PixelHeight * Scale;
                    }
                }
                if (show_Value)
                {
                    // Отображается картинка и значение        
                    Grid.SetColumnSpan(m_Image, 1);
                }
                else
                {
                    // Отображается только картинка
                    Grid.SetColumnSpan(m_Image, 3);
                }
            }
            else
            {
                if (m_Image != null)
                {
                    m_Image.Visibility = Visibility.Collapsed;
                }
            }
        }

        void image_ImageOpened(object sender, RoutedEventArgs e)
        {
            var image = sender as BitmapImage;
            if (image != null && m_Image != null && m_Image.Visibility == Visibility.Visible)
            {
                image.ImageOpened -= new EventHandler<RoutedEventArgs>(image_ImageOpened);
                m_Image.Width = image.PixelWidth * Scale;
                m_Image.Height = image.PixelHeight * Scale;
            }
        }

        //void ApplyHintSettings()
        //{
        //    // Подсказка
        //    if (Owner.Cells_UseHint)
        //    {
        //        if (Cell != null)
        //        {
        //            // Собственный контрол для отображения подсказки
        //            m_ToolTip = new ToolTipControl();
        //            if (Cell.CellDescr != null && Cell.CellDescr.Value != null && Cell.CellDescr.Value.Value != null)
        //            {
        //                m_ToolTip.Caption = Cell.CellDescr.Value.DisplayValue;
        //            }
        //            else
        //            {
        //                m_ToolTip.Caption = "(null)";
        //            }
        //            m_ToolTip.Text = Cell.GetShortTupleToStr();
        //            ToolTipService.SetToolTip(this, m_ToolTip);
        //        }
        //        else
        //        {
        //            ToolTipService.SetToolTip(this, null);
        //        }
        //    }
        //}


        bool m_IsSelected = false;
        public bool IsSelected
        {
            set
            {
                if (m_IsSelected != value)
                {
                    m_IsSelected = value;
                    ApplyBackgroundSettings();
                }
            }
            get
            {
                return m_IsSelected;
            }
        }

        UpdateEntry m_NotRecalculatedChange;
        /// <summary>
        /// Change the cell, with not recalculated
        /// </summary>
        public UpdateEntry NotRecalculatedChange
        {
            get { return m_NotRecalculatedChange; }
            set
            {
                m_NotRecalculatedChange = value;
                ApplyBackgroundSettings();
                ApplyValueSettings();
            }
        }

        UpdateEntry m_RecalculatedChange;
        /// <summary>
        /// Change the cell, which already recalculated
        /// </summary>
        public UpdateEntry RecalculatedChange
        {
            get { return m_RecalculatedChange; }
            set
            {
                m_RecalculatedChange = value;
                ApplyBackgroundSettings();
                ApplyValueSettings();
            }
        }

        bool m_IsFocused = false;
        public new bool IsFocused
        {
            set
            {
                if (m_IsFocused != value)
                {
                    m_IsFocused = value;
                    ApplyBorderSettings();
                    ApplyBackgroundSettings();
                }
            }
            get
            {
                return m_IsFocused;
            }
        }

        bool m_IsEditing = false;
        public bool IsEditing
        {
            set
            {
                m_IsEditing = value;
            }
            get
            {
                return m_IsEditing;
            }
        }
    }
}
