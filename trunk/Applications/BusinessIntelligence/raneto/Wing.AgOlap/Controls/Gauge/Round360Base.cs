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
using Ranet.AgOlap.Controls.ContextMenu;
using System.Windows.Browser;
using Ranet.AgOlap.Controls.General;

namespace Ranet.AgOlap.Controls.Gauge
{
    public class Round360Base : UserControl
    {
        Canvas LayoutRoot;
        public Round360Base()
        {
            LayoutRoot = new Canvas();
            this.Content = LayoutRoot;
            this.SizeChanged += new SizeChangedEventHandler(Round360Base_SizeChanged);
            this.Loaded += new RoutedEventHandler(Round360Base_Loaded);

            m_DoubleClick_Story = new Storyboard();
            m_DoubleClick_Story.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250));
            m_DoubleClick_Story.Completed += new EventHandler(m_DoubleClick_Story_Completed);
            LayoutRoot.Resources.Add("m_DoubleClick_Story", m_DoubleClick_Story);

            this.MouseLeftButtonDown += new MouseButtonEventHandler(Round360Base_MouseLeftButtonDown);

            this.MouseLeave += new MouseEventHandler(OnMouseLeave);
            this.MouseEnter += new MouseEventHandler(OnMouseEnter);
        }

        void OnMouseEnter(object sender, MouseEventArgs e)
        {
            HtmlPage.Document.AttachEvent("oncontextmenu", new EventHandler<HtmlEventArgs>(ContentMenu_EventHandler));
        }

        void OnMouseLeave(object sender, MouseEventArgs e)
        {
            HtmlPage.Document.DetachEvent("oncontextmenu", new EventHandler<HtmlEventArgs>(ContentMenu_EventHandler));
        }

        void ContentMenu_EventHandler(object sender, HtmlEventArgs e)
        {
            e.PreventDefault();
            e.StopPropagation();
            Point point = new Point(e.OffsetX, e.OffsetY);
            if (AgControlBase.GetSLBounds(this).Contains(point))
            {
                OnShowContextMenu(point);
            }
        }

        void OnShowContextMenu(Point position)
        {
            if (ContextMenu.IsDropDownOpen)
                ContextMenu.IsDropDownOpen = false;

            ContextMenu.SetLocation(position);
            ContextMenu.Tag = this;
            ContextMenu.IsDropDownOpen = true;
        }

        CustomContextMenu m_ContextMenu = null;
        public CustomContextMenu ContextMenu
        {
            get
            {
                if (m_ContextMenu == null)
                {
                    m_ContextMenu = CreateContextMenu();
                }
                if (m_ContextMenu == null)
                {
                    m_ContextMenu = new CustomContextMenu();
                }
                return m_ContextMenu;
            }
        }

        protected virtual CustomContextMenu CreateContextMenu()
        {
            CustomContextMenu contextMenu = new CustomContextMenu();
            return contextMenu;
        }

        void Round360Base_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_ClickCount++;
            m_LastArgs = e;
            m_DoubleClick_Story.Begin();
        }

        public event MouseEventHandler MouseDoubleClick;
        protected void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (this.MouseDoubleClick != null)
            {
                this.MouseDoubleClick(this, e);
            }
        }

        int m_ClickCount = 0;
        MouseButtonEventArgs m_LastArgs;
        void m_DoubleClick_Story_Completed(object sender, EventArgs e)
        {
            if (m_ClickCount > 1)
            {
                this.OnMouseDoubleClick(m_LastArgs);
            }
            m_ClickCount = 0;   
        }

        Storyboard m_DoubleClick_Story;

        void Round360Base_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        void Round360Base_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Refresh();
        }

        // Толщина границы спидометра
        double outerSize = 0;

        double m_CurrentValue = double.MinValue;
        /// <summary>
        /// Текущее значение
        /// </summary>
        public double CurrentValue
        {
            get
            {
                return m_CurrentValue;
            }
            set
            {
                m_CurrentValue = value;
                Refresh();
            }
        }

        /// <summary>
        /// Текущее значение, округленное до допустимых границ
        /// </summary>
        public double RoundedCurrentValue
        {
            get
            {
                double value = Math.Min(CurrentValue, m_MaxValue);
                value = Math.Max(value, m_MinValue);
                return value;
            }
        }

        TextBlock currentValueText;

        String m_Text = String.Empty;
        /// <summary>
        /// Тект, который отображается в качестве подписи на циферблате
        /// </summary>
        public String Text
        {
            get {
                return m_Text;
            }
            set {
                m_Text = value;
                Refresh();
            }
        }

        String m_DivisionsFormatString = "0.##";
        /// <summary>
        /// Строка форматирования для значений основных делений
        /// </summary>
        public String DivisionsFormatString
        {
            get
            {
                return m_DivisionsFormatString;
            }
            set
            {
                m_DivisionsFormatString = value;
                Refresh();
            }
        }

        String m_CurrentValueFormatString = "0.##";
        /// <summary>
        /// Строка форматирования для отображения текущего значения
        /// </summary>
        public String CurrentValueFormatString
        {
            get
            {
                return m_CurrentValueFormatString;
            }
            set
            {
                m_CurrentValueFormatString = value;
                Refresh();
            }
        }

        private double m_HeightValue = double.MinValue;
        /// <summary>
        /// Верхнее значение
        /// </summary>
        public double HighValue
        {
            get {
                return m_HeightValue;
            }
            set {
                m_HeightValue = value;
                Refresh();
            }
        }

        private double m_LowValue = double.MinValue;
        /// <summary>
        /// Нижнее значение
        /// </summary>
        public double LowValue
        {
            get
            {
                return m_LowValue;
            }
            set
            {
                m_LowValue = value;
                Refresh();
            }
        }

        bool m_HidePercentageInDivisions = false;
        /// <summary>
        /// Флаг, определяющий необходимость отображения % в значениях основных делений
        /// </summary>
        public bool HidePercentageInDivisions
        {
            get
            {
                return m_HidePercentageInDivisions;
            }
            set
            {
                m_HidePercentageInDivisions = value;
                Refresh();
            }
        }

        bool m_HidePercentageInValue = false;
        /// <summary>
        /// Флаг, определяющий необходимость отображения % в текущем значении
        /// </summary>
        public bool HidePercentageInValue
        {
            get
            {
                return m_HidePercentageInValue;
            }
            set
            {
                m_HidePercentageInValue = value;
                Refresh();
            }
        }

        /// <summary>
        /// Отступ спидометра от границ в процентах
        /// </summary>
        double m_MeterMargin = 0;
        public double MeterMargin
        {
            get {
                double margin = m_MeterMargin;
                margin = Math.Max(0, m_MeterMargin);
                margin = Math.Min(m_MeterMargin, 100);
                return m_MeterMargin; 
            }
            set { m_MeterMargin = value; }
        }

        double m_MeterSize
        {
            get {
                double meter_size = Math.Min(LayoutRoot.ActualWidth, LayoutRoot.ActualHeight);
                meter_size = meter_size - (meter_size / 100) * MeterMargin;
                meter_size = Math.Max(meter_size, 0);
                return meter_size;
            }
        }

        double m_MeterTop
        {
            get
            {
                double meter_top = LayoutRoot.ActualHeight / 2 - m_MeterSize / 2;
                meter_top = Math.Max(meter_top, 0);
                return meter_top;
            }
        }

        double m_MeterLeft
        {
            get
            {
                double meter_left = LayoutRoot.ActualWidth / 2 - m_MeterSize / 2;
                meter_left = Math.Max(meter_left, 0);
                return meter_left;
            }
        }

        protected void Refresh()
        {
            LayoutRoot.Children.Clear();

            LayoutRoot.Background = new SolidColorBrush(m_BackgroundColor);
            
            // Граница спидометра
            // Внешний контур
            Ellipse outer_ellipse1 = new Ellipse() { Height = m_MeterSize, Width = m_MeterSize };
            outer_ellipse1.Stroke = new SolidColorBrush(Colors.Black);
            //LinearGradientBrush oter_brush1 = new LinearGradientBrush();
            //oter_brush1.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF) });
            //oter_brush1.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x33, 0x33, 0x33), Offset = 1 });
            //outer_ellipse1.Stroke = oter_brush1;
            outer_ellipse1.StrokeThickness = m_MeterSize * 0.005;
            LayoutRoot.Children.Add(outer_ellipse1);
            outer_ellipse1.SetValue(Canvas.TopProperty, m_MeterTop);
            outer_ellipse1.SetValue(Canvas.LeftProperty, m_MeterLeft);
            //outer_ellipse1.Effect = new System.Windows.Media.Effects.DropShadowEffect();

            // Внутреннний контур
            //Ellipse outer_ellipse2 = new Ellipse() { Height = m_MeterSize - outer_ellipse1.StrokeThickness * 2 + 1, Width = m_MeterSize - outer_ellipse1.StrokeThickness * 2 + 1 };
            //LinearGradientBrush oter_brush2 = new LinearGradientBrush();
            //oter_brush2.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x33, 0x33, 0x33) });
            //oter_brush2.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), Offset = 1 });
            //outer_ellipse2.Stroke = oter_brush2;
            //outer_ellipse2.StrokeThickness = outer_ellipse1.StrokeThickness;
            //outer_ellipse2.SetValue(Canvas.TopProperty, m_MeterTop + outer_ellipse1.StrokeThickness);
            //outer_ellipse2.SetValue(Canvas.LeftProperty, m_MeterLeft + outer_ellipse1.StrokeThickness);
            //LayoutRoot.Children.Add(outer_ellipse2);

            outerSize = outer_ellipse1.StrokeThickness; // outer_ellipse1.StrokeThickness + outer_ellipse2.StrokeThickness;

            // Область под циферблат
            Ellipse calibration_ellipse1 = new Ellipse() { Height = m_MeterSize - outerSize*2 + 2, Width = m_MeterSize - outerSize*2 + 2};
            LinearGradientBrush calibration_brush1 = new LinearGradientBrush();
            calibration_brush1.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF) });
            calibration_brush1.GradientStops.Add(new GradientStop() { Color = m_RoundBackgroundColor, Offset = 1 });
            calibration_ellipse1.Fill = calibration_brush1;
            calibration_ellipse1.SetValue(Canvas.TopProperty, m_MeterTop + outerSize - 1);
            calibration_ellipse1.SetValue(Canvas.LeftProperty, m_MeterLeft + outerSize - 1);
            LayoutRoot.Children.Add(calibration_ellipse1);
            // calibration_ellipse1.Effect = new System.Windows.Media.Effects.DropShadowEffect();

            // Центр спидометра
            double cX = m_MeterSize / 2;
            double cY = m_MeterSize / 2;
            double center_size = m_MeterSize * 0.1;
            Ellipse center_ellipse1 = new Ellipse() { Height = center_size, Width = center_size};
            center_ellipse1.Fill = new SolidColorBrush(Colors.Black);
            center_ellipse1.SetValue(Canvas.TopProperty, m_MeterTop + cX - center_size / 2);
            center_ellipse1.SetValue(Canvas.LeftProperty, m_MeterLeft + cY - center_size / 2);
            center_ellipse1.Effect = new System.Windows.Media.Effects.DropShadowEffect();
            LayoutRoot.Children.Add(center_ellipse1);

            //double center_size2 = center_size * 0.75;
            //Ellipse center_ellipse2 = new Ellipse() { Height = center_size2, Width = center_size2 };
            //center_ellipse2.Fill = new SolidColorBrush(Colors.DarkGray);
            //center_ellipse2.SetValue(Canvas.TopProperty, cX - center_size2 / 2);
            //center_ellipse2.SetValue(Canvas.LeftProperty, cY - center_size2 / 2);
            //LayoutRoot.Children.Add(center_ellipse2);


            // Рисуем значения
            DrawCalibration();

            // Рисуем контрол для текущего значения
            currentValueText = new TextBlock();
            currentValueText.Foreground = new SolidColorBrush(m_ForeColor);

            string num = string.Empty;
            try
            {
                num = CurrentValue.ToString(this.CurrentValueFormatString);
            }
            catch { 
                num = CurrentValue.ToString(); 
            }

            if (HidePercentageInValue)
            { 
                num = num.Replace("%", String.Empty);
            }
            currentValueText.Text = num;
            currentValueText.Width = m_MeterSize * 0.25;
            currentValueText.Height = m_MeterSize * 0.06;
            currentValueText.FontSize = currentValueText.Height * 0.75;
            currentValueText.TextWrapping = TextWrapping.NoWrap;
            currentValueText.TextAlignment = TextAlignment.Right;


            Border value_border1 = new Border();
            //value_border1.BorderBrush = new SolidColorBrush(Color.FromArgb(100, 0x33, 0x33, 0x33));
            value_border1.BorderBrush = new SolidColorBrush(Colors.Black);
            value_border1.BorderThickness = new Thickness(m_MeterSize * 0.0025);
            //value_border1.Effect = new System.Windows.Media.Effects.DropShadowEffect();

            value_border1.Child = currentValueText;
            value_border1.Padding = new Thickness(currentValueText.Height * 0.1);

            LayoutRoot.Children.Add(value_border1);
            value_border1.SetValue(Canvas.TopProperty, m_MeterTop + cY + center_size + m_MeterSize * 0.15);
            value_border1.SetValue(Canvas.LeftProperty, m_MeterLeft + cX - currentValueText.Width / 2);
            value_border1.Background = new SolidColorBrush(Color.FromArgb(25, 0x33, 0x33, 0x33));

            // Рисуем название
            TextBlock captionText = new TextBlock();
            captionText.Text = m_Text;
            captionText.FontSize = m_MeterSize * 0.04;
            captionText.Foreground = new SolidColorBrush(Colors.Blue);
            LayoutRoot.Children.Add(captionText);
            captionText.SetValue(Canvas.TopProperty, m_MeterTop + cY + center_size + m_MeterSize * 0.05);
            captionText.SetValue(Canvas.LeftProperty, m_MeterLeft + cX - captionText.ActualWidth / 2);
            
            // Рисуем стрелку
            DrawArrow(RoundedCurrentValue);
        }

        Line arrow_Line1;
        Line arrow_Line2;
        void DrawArrow(double value)
        {
            // Центр спидометра
            double cX = m_MeterSize / 2;
            double cY = m_MeterSize / 2;

            if (arrow_Line1 != null)
            {
                LayoutRoot.Children.Remove(arrow_Line1);
            }
            if (arrow_Line2 != null)
            {
                LayoutRoot.Children.Remove(arrow_Line2);
            }

            double arrow_length = m_MeterSize * 0.1;
            double arrow_end_length = m_MeterSize * 0.15;

            // Рисуем стрелку
            arrow_Line1 = new Line();
            arrow_Line1.Effect = new System.Windows.Media.Effects.DropShadowEffect();
            arrow_Line1.Stroke = new SolidColorBrush(Colors.Black);
            arrow_Line1.StrokeThickness = m_MeterSize * 0.01;
            arrow_Line1.StrokeEndLineCap = PenLineCap.Flat;
            arrow_Line1.X1 = m_MeterLeft + cX;
            arrow_Line1.Y1 = m_MeterTop + cY;
            arrow_Line1.X2 = (m_MeterLeft + cX + arrow_length * Math.Cos(GetRadian(GetAngleByValue(value))));
            arrow_Line1.Y2 = (m_MeterTop + cY + arrow_length * Math.Sin(GetRadian(GetAngleByValue(value))));
            LayoutRoot.Children.Add(arrow_Line1);

            // Рисуем стрелку
            arrow_Line2 = new Line();
            arrow_Line2.Effect = new System.Windows.Media.Effects.DropShadowEffect();
            arrow_Line2.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xF7, 0x36, 0x31));
            arrow_Line2.StrokeThickness = arrow_Line1.StrokeThickness;
            arrow_Line2.StrokeEndLineCap = PenLineCap.Round;
            arrow_Line2.X1 = (m_MeterLeft + cX + arrow_length * Math.Cos(GetRadian(GetAngleByValue(value))));
            arrow_Line2.Y1 = (m_MeterTop + cY + arrow_length * Math.Sin(GetRadian(GetAngleByValue(value))));
            arrow_Line2.X2 = (m_MeterLeft + cX + (arrow_length + arrow_end_length) * Math.Cos(GetRadian(GetAngleByValue(value))));
            arrow_Line2.Y2 = (m_MeterTop + cY + (arrow_length + arrow_end_length) * Math.Sin(GetRadian(GetAngleByValue(value))));
            LayoutRoot.Children.Add(arrow_Line2);

        }

        int m_DivisionsCount = 10;
        /// <summary>
        /// Количество основных делений
        /// </summary>
        public int DivisionsCount
        {
            get {
                return m_DivisionsCount;
            }
            set {
                m_DivisionsCount = value;
                Refresh();
            }
        }

        int m_SubDivisionsCount = 5;
        /// <summary>
        /// Количество промежуточных делений
        /// </summary>
        public int SubDivisionsCount
        {
            get
            {
                return m_SubDivisionsCount;
            }
            set
            {
                m_SubDivisionsCount = value;
                Refresh();
            }
        }

        double m_FromAngle = 135F;
        double m_ToAngle = 405F;

        double m_MinValue = 0;
        public double MinValue
        {
            get
            {
                return m_MinValue;
            }
            set
            {
                m_MinValue = value;
                Refresh();
            }
        }

        double m_MaxValue = 100;
        public double MaxValue
        {
            get
            {
                return m_MaxValue;
            }
            set
            {
                m_MaxValue = value;
                Refresh();
            }
        }

        Color m_GoodColor = Colors.Green;
        /// <summary>
        /// Цвет для верхней области - между верхним и максимальным значениями
        /// </summary>
        public Color GoodColor
        {
            get { return m_GoodColor; }
            set
            {
                m_GoodColor = value;
                Refresh();
            }
        }

        /// <summary>
        /// Тект всплывающей подсказки
        /// </summary>
        public string ToolTipText
        {
            get
            {
                object tooltip = ToolTipService.GetToolTip(this);
                if (tooltip != null)
                    return tooltip.ToString();
                return String.Empty;
            }
            set
            {
                ToolTipService.SetToolTip(this, value);
            }
        }

        Color m_BackgroundColor = Colors.Transparent;
        /// <summary>
        /// Цвет фона 
        /// </summary>
        public Color BackgroundColor
        {
            get { return m_BackgroundColor; }
            set
            {
                m_BackgroundColor = value;
                Refresh();
            }
        }

        Color m_RoundBackgroundColor = Colors.Gray;
        public Color RoundBackgroundColor
        {
            get { return m_RoundBackgroundColor; }
            set
            {
                m_RoundBackgroundColor = value;
                Refresh();
            }
        }

        Color m_BadColor = Colors.Red;
        /// <summary>
        /// Цвет для нижней области - между минимальным и нижним значениями
        /// </summary>
        public Color BadColor
        {
            get { return m_BadColor; }
            set
            {
                m_BadColor = value;
                Refresh();
            }
        }

        Color m_MiddleColor = Colors.Yellow;
        /// <summary>
        /// Цвет средней области - между нижним и верхним значениями
        /// </summary>
        public Color MiddleColor
        {
            get { return m_MiddleColor; }
            set
            {
                m_MiddleColor = value;
                Refresh();
            }
        }

        bool m_LowToHigh = true;
        public bool LowToHigh
        {
            get { return m_LowToHigh; }
            set
            {
                m_LowToHigh = value;
                Refresh();
            }
        }

        Point CalculateRotation(Point center, double angle, double radius)
        {
            double x = (center.X + radius * Math.Cos(GetRadian(angle)));
            double y = (center.Y + radius * Math.Sin(GetRadian(angle)));
            return new Point(x, y);
        }

        double GetAngleByValue(double value)
        {
            value = Math.Min(value, m_MaxValue);
            value = Math.Max(value, m_MinValue);

            double angle = m_FromAngle;
            if ((m_MaxValue - m_MinValue) > 0)
            {
                // Отношение:
                // m_MaxValue - m_MinValue  => m_ToAngle - m_FromAngle
                // value - m_MinValue       => X - m_FromAngle
                angle = ((m_ToAngle - m_FromAngle) * (value - m_MinValue)) / (m_MaxValue - m_MinValue) + m_FromAngle; 
            }
            return angle;
        }

        Color m_ForeColor = Colors.Black;
        /// <summary>
        /// Цвет для значений циферблата
        /// </summary>
        public Color ForeColor
        {
            get { return m_ForeColor; }
            set
            {
                m_ForeColor = value;
                Refresh();
            }
        }

        /// <summary>
        /// Draws the Ruler
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="cX"></param>
        /// <param name="cY"></param>
        private void DrawCalibration()
        {
            if (m_MeterSize <= 0)
                return;

            int noOfParts = this.m_DivisionsCount + 1;
            int noOfIntermediates = this.m_SubDivisionsCount;
            double currentAngle_rad = GetRadian(m_FromAngle);
            double currentAngle = m_FromAngle;

            // отступ от края
            double offset = outerSize * 4;
            // Длина риски для основного значения
            double division_length = m_MeterSize / 40;
            // Внешний радиус для риски
            double division_outer_radius = m_MeterSize / 2 - offset;

            // Сектор в котором отображаются значения
            double totalAngle = m_ToAngle - m_FromAngle;
            // Угол между соседними рисками (основными или промежуточными)
            double incr = ((totalAngle) / ((noOfParts - 1) * (noOfIntermediates + 1)));
            double incr_rad = GetRadian(((totalAngle) / ((noOfParts - 1) * (noOfIntermediates + 1))));

            Brush divisionsBrush = new SolidColorBrush(Colors.Black);
            Brush subdivisionsBrush = new SolidColorBrush(Colors.Black);

            double rulerValue = m_MinValue;

            // Центр спидометра
            double cX = m_MeterSize / 2;
            double cY = m_MeterSize / 2;
            Point center = new Point(cX, cY);

            if (m_LowValue != double.MinValue && m_HeightValue != double.MinValue && m_LowValue <= m_HeightValue)
            {
                // Толщина линий
                double line_size = offset;
                // Внешний радиус для рисок - Смещаем риски ближе к центру
                division_outer_radius -= offset/2;
                // Дуги для допустимых значений
                double arc_radius = division_outer_radius;

                SolidColorBrush lowBrush;
                SolidColorBrush hiBrush;
                SolidColorBrush midBrush;
                // Дуга от MinValue до LoValue 
                if (this.m_LowToHigh)
                {
                    lowBrush = new SolidColorBrush(m_BadColor);
                    hiBrush = new SolidColorBrush(m_GoodColor);
                }
                else
                {
                    lowBrush = new SolidColorBrush(m_GoodColor);
                    hiBrush = new SolidColorBrush(m_BadColor);
                }
                midBrush = new SolidColorBrush(m_MiddleColor);

                // Дуга между минимумом и нижним значением
                Path low_path = new Path();
                low_path.StrokeThickness = line_size;
                low_path.Stroke = lowBrush;
                PathGeometry low_geometry = new PathGeometry();
                low_path.Data = low_geometry;
                PathFigure low_Figure = new PathFigure();
                low_Figure.StartPoint = CalculateRotation(center, GetAngleByValue(m_MinValue), arc_radius);
                low_geometry.Figures.Add(low_Figure);

                ArcSegment low_arc = new ArcSegment();
                double low_angle = GetAngleByValue(m_LowValue);
                low_arc.Point = CalculateRotation(center, low_angle, arc_radius);
                low_arc.Size = new Size(arc_radius, arc_radius);
                low_arc.SweepDirection = SweepDirection.Clockwise;
                low_arc.IsLargeArc = (low_angle - m_FromAngle) > 180;
                low_Figure.Segments.Add(low_arc);

                // Дуга между нижним значением и верхним значением
                Path mid_path = new Path();
                mid_path.StrokeThickness = line_size;
                mid_path.Stroke = midBrush;
                PathGeometry mid_geometry = new PathGeometry();
                mid_path.Data = mid_geometry;
                PathFigure mid_Figure = new PathFigure();
                mid_Figure.StartPoint = low_arc.Point;
                mid_geometry.Figures.Add(mid_Figure);

                ArcSegment mid_arc = new ArcSegment();
                double hi_angle = GetAngleByValue(m_HeightValue);
                mid_arc.Point = CalculateRotation(center, hi_angle, arc_radius);
                mid_arc.Size = new Size(arc_radius, arc_radius);
                mid_arc.SweepDirection = SweepDirection.Clockwise;
                mid_arc.IsLargeArc = (hi_angle - low_angle) > 180;
                mid_Figure.Segments.Add(mid_arc);

                // Дуга между верхним значением и максимумом
                Path hi_path = new Path();
                hi_path.StrokeThickness = line_size;
                hi_path.Stroke = hiBrush;
                PathGeometry hi_geometry = new PathGeometry();
                hi_path.Data = hi_geometry;
                PathFigure hi_Figure = new PathFigure();
                hi_Figure.StartPoint = mid_arc.Point;
                hi_geometry.Figures.Add(hi_Figure);

                ArcSegment hi_arc = new ArcSegment();
                hi_arc.Point = CalculateRotation(center, m_ToAngle, arc_radius);
                hi_arc.Size = new Size(arc_radius, arc_radius);
                hi_arc.SweepDirection = SweepDirection.Clockwise;
                hi_arc.IsLargeArc = (m_ToAngle - hi_angle) > 180;
                hi_Figure.Segments.Add(hi_arc);

                low_path.SetValue(Canvas.TopProperty, m_MeterTop);
                low_path.SetValue(Canvas.LeftProperty, m_MeterLeft);
                mid_path.SetValue(Canvas.TopProperty, m_MeterTop);
                mid_path.SetValue(Canvas.LeftProperty, m_MeterLeft);
                hi_path.SetValue(Canvas.TopProperty, m_MeterTop);
                hi_path.SetValue(Canvas.LeftProperty, m_MeterLeft);
                low_path.Effect = new System.Windows.Media.Effects.BlurEffect();
                mid_path.Effect = new System.Windows.Media.Effects.BlurEffect();
                hi_path.Effect = new System.Windows.Media.Effects.BlurEffect();
                LayoutRoot.Children.Add(low_path);
                LayoutRoot.Children.Add(mid_path);
                LayoutRoot.Children.Add(hi_path);
            }

            // Внутренний радиус для риски
            double division_inner_radius = division_outer_radius - division_length;
            double angle = m_FromAngle;
            for (int i = 0; i < noOfParts; i++)
            {
                // Рисуем риску
                Border border = new Border();
                border.BorderThickness = new Thickness(2);
                border.BorderBrush = divisionsBrush;
                border.Width = division_outer_radius - division_inner_radius;
                border.Height = 2;
                border.SetValue(Canvas.TopProperty, m_MeterTop + cY + division_outer_radius * Math.Sin(currentAngle_rad));
                border.SetValue(Canvas.LeftProperty, m_MeterLeft + cX + division_outer_radius * Math.Cos(currentAngle_rad));
                border.RenderTransform = new RotateTransform() { Angle = currentAngle + 180};
                border.Effect = new System.Windows.Media.Effects.DropShadowEffect();
                LayoutRoot.Children.Add(border);

                //Line divisionLine = new Line();
                //divisionLine.Stroke = divisionsBrush;
                //divisionLine.StrokeThickness = 2;
                //divisionLine.X1 = (m_MeterLeft + cX + division_outer_radius * Math.Cos(currentAngle));
                //divisionLine.Y1 = (m_MeterTop + cY + division_outer_radius * Math.Sin(currentAngle));
                //divisionLine.X2 = (m_MeterLeft + cX + division_inner_radius * Math.Cos(currentAngle));
                //divisionLine.Y2 = (m_MeterTop + cY + division_inner_radius * Math.Sin(currentAngle));
                //divisionLine.Effect = new System.Windows.Media.Effects.DropShadowEffect();
                //LayoutRoot.Children.Add(divisionLine);

                // Рисуем значение
                double text_radius = division_inner_radius - m_MeterSize * 0.08;
                double tx = (cX + text_radius * Math.Cos(currentAngle_rad));
                double ty = (cY + text_radius * Math.Sin(currentAngle_rad));

                if (i > 0)
                    rulerValue += (double)((m_MaxValue - m_MinValue) / (noOfParts - 1));
                double value = Math.Round(rulerValue, 2);

                TextBlock valueText = new TextBlock();
                valueText.Foreground = new SolidColorBrush(m_ForeColor);
                valueText.FontSize = m_MeterSize * 0.05;
                String num = rulerValue.ToString(DivisionsFormatString);
                if (HidePercentageInDivisions)
                {
                    num = num.Replace("%", String.Empty);
                }
                valueText.Text = num;

                LayoutRoot.Children.Add(valueText);
                // Центрируем значение - чтобы центр текста попал в вычисленную координату
                if (valueText.ActualWidth.CompareTo(double.NaN) != 0)
                {
                    valueText.SetValue(Canvas.LeftProperty, m_MeterLeft + tx - valueText.ActualWidth / 2);
                }
                else
                {
                    valueText.SetValue(Canvas.LeftProperty, m_MeterLeft + tx);
                }
                if (valueText.ActualHeight.CompareTo(double.NaN) != 0)
                {
                    valueText.SetValue(Canvas.TopProperty, m_MeterTop + ty - valueText.ActualHeight / 2);
                }
                else
                {
                    valueText.SetValue(Canvas.TopProperty, m_MeterTop + ty);
                }

                currentAngle_rad += incr_rad;
                currentAngle += incr;

                if (i == noOfParts - 1)
                    break;

                // Длина риски для промежуточного значения
                double subdivision_length = division_length / 2;
                // Внешний и внутренний радиус для риски
                double subdivision_outer_radius = division_outer_radius;
                double subdivision_inner_radius = subdivision_outer_radius - subdivision_length;

                // Рисуем промежуточные значения
                for (int j = 0; j <= noOfIntermediates; j++)
                {
                    if (j != 0)
                    {
                        currentAngle_rad += incr_rad;
                        currentAngle += incr;
                    }
                    
                    // Рисуем риску
                    Border sub_border = new Border();
                    sub_border.BorderThickness = new Thickness(2);
                    sub_border.BorderBrush = divisionsBrush;
                    sub_border.Width = subdivision_outer_radius - subdivision_inner_radius;
                    sub_border.Height = 2;
                    sub_border.SetValue(Canvas.TopProperty, m_MeterTop + cY + subdivision_outer_radius * Math.Sin(currentAngle_rad));
                    sub_border.SetValue(Canvas.LeftProperty, m_MeterLeft + cX + subdivision_outer_radius * Math.Cos(currentAngle_rad));
                    sub_border.RenderTransform = new RotateTransform() { Angle = currentAngle + 180 };
                    sub_border.Effect = new System.Windows.Media.Effects.DropShadowEffect();
                    LayoutRoot.Children.Add(sub_border);
                    
                    //// Рисуем риску
                    //Line subivisionLine = new Line();
                    //subivisionLine.Stroke = subdivisionsBrush;
                    //subivisionLine.StrokeEndLineCap = PenLineCap.Flat;
                    //subivisionLine.StrokeThickness = 2;
                    //subivisionLine.X1 = (m_MeterLeft + cX + subdivision_outer_radius * Math.Cos(currentAngle));
                    //subivisionLine.Y1 = (m_MeterTop + cY + subdivision_outer_radius * Math.Sin(currentAngle));
                    //subivisionLine.X2 = (m_MeterLeft + cX + subdivision_inner_radius * Math.Cos(currentAngle));
                    //subivisionLine.Y2 = (m_MeterTop + cY + subdivision_inner_radius * Math.Sin(currentAngle));
                    //subivisionLine.Effect = new System.Windows.Media.Effects.DropShadowEffect();
                    //LayoutRoot.Children.Add(subivisionLine);
                }
            }
        }

        /// <summary>
        /// Converts the given degree to radian.
        /// </summary>
        /// <param name="theta"></param>
        /// <returns></returns>
        public double GetRadian(double theta)
        {
            return theta * Math.PI / 180F;
        }
    }
}
