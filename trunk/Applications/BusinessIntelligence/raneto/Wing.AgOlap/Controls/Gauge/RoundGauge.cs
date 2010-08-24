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
	public class RoundGauge : Canvas
	{
		// public static readonly DependencyProperty AutoRefreshProperty;
		public static readonly DependencyProperty MinValueProperty;
		public static readonly DependencyProperty LowValueProperty;
		public static readonly DependencyProperty CurrentValueProperty;
		public static readonly DependencyProperty HighValueProperty;
		public static readonly DependencyProperty MaxValueProperty;
		public static readonly DependencyProperty TextProperty;
		public static readonly DependencyProperty TooltipTextProperty;
		public static readonly DependencyProperty DivisionsFormatStringProperty;
		public static readonly DependencyProperty CurrentValueFormatStringProperty;
		public static readonly DependencyProperty HidePercentageInDivisionsProperty;
		public static readonly DependencyProperty HidePercentageInValueProperty;
		public static readonly DependencyProperty MeterMarginProperty;
		public static readonly DependencyProperty MeterBackgroundColorProperty;
		public static readonly DependencyProperty DivisionsCountProperty;
		public static readonly DependencyProperty SubDivisionsCountProperty;
		public static readonly DependencyProperty HighColorProperty;
		public static readonly DependencyProperty MiddleColorProperty;
		public static readonly DependencyProperty LowColorProperty;
		public static readonly DependencyProperty ForeColorProperty;
		public static readonly DependencyProperty BackgroundColorProperty;
		public static readonly DependencyProperty TextColorProperty;

		static RoundGauge()
		{
			ForeColorProperty = DependencyProperty.Register("ForeColor", typeof(Color), typeof(RoundGauge), new PropertyMetadata(Colors.Black, ValueChangedCallback));
			TextColorProperty = DependencyProperty.Register("TextColor", typeof(Color), typeof(RoundGauge), new PropertyMetadata(Colors.Blue, ValueChangedCallback));
			BackgroundColorProperty = DependencyProperty.Register("BackgroundColor", typeof(Color), typeof(RoundGauge), new PropertyMetadata(Colors.Transparent, ValueChangedCallback));

			// AutoRefreshProperty = DependencyProperty.Register("AutoRefresh", typeof(bool), typeof(RoundGauge), new PropertyMetadata(true, ValueChangedCallback));
			MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(RoundGauge), new PropertyMetadata((double)0.0, ValueChangedCallback));
			LowValueProperty = DependencyProperty.Register("LowValue", typeof(double), typeof(RoundGauge), new PropertyMetadata((double)10.0, ValueChangedCallback));
			CurrentValueProperty = DependencyProperty.Register("CurrentValue", typeof(double), typeof(RoundGauge), new PropertyMetadata((double)0.0, CurrentValueChangedCallback));
			HighValueProperty = DependencyProperty.Register("HighValue", typeof(double), typeof(RoundGauge), new PropertyMetadata((double)80.0, ValueChangedCallback));
			MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(RoundGauge), new PropertyMetadata((double)100.0, ValueChangedCallback));
			MeterMarginProperty = DependencyProperty.Register("MeterMargin", typeof(double), typeof(RoundGauge), new PropertyMetadata((double)0.0, ValueChangedCallback));
			MeterBackgroundColorProperty = DependencyProperty.Register("MeterBackgroundColor", typeof(Color), typeof(RoundGauge), new PropertyMetadata(Colors.Gray, ValueChangedCallback));
			TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(RoundGauge), new PropertyMetadata(string.Empty, TextChangedCallback));
			TooltipTextProperty = DependencyProperty.Register("TooltipText", typeof(string), typeof(RoundGauge), new PropertyMetadata(string.Empty, ValueChangedCallback));
			DivisionsFormatStringProperty = DependencyProperty.Register("DivisionsFormatString", typeof(string), typeof(RoundGauge), new PropertyMetadata("0.##", ValueChangedCallback));
			CurrentValueFormatStringProperty = DependencyProperty.Register("CurrentValueFormatString", typeof(string), typeof(RoundGauge), new PropertyMetadata("0.##", ValueChangedCallback));
			HidePercentageInDivisionsProperty = DependencyProperty.Register("HidePercentageInDivisions", typeof(bool), typeof(RoundGauge), new PropertyMetadata(false, ValueChangedCallback));
			HidePercentageInValueProperty = DependencyProperty.Register("HidePercentageInValue", typeof(bool), typeof(RoundGauge), new PropertyMetadata(false, ValueChangedCallback));
			DivisionsCountProperty = DependencyProperty.Register("DivisionsCount", typeof(int), typeof(RoundGauge), new PropertyMetadata((int)10, ValueChangedCallback));
			SubDivisionsCountProperty = DependencyProperty.Register("SubDivisionsCount", typeof(int), typeof(RoundGauge), new PropertyMetadata((int)5, ValueChangedCallback));
			HighColorProperty = DependencyProperty.Register("HighColor", typeof(Color), typeof(RoundGauge), new PropertyMetadata(Colors.Green, ValueChangedCallback));
			MiddleColorProperty = DependencyProperty.Register("MiddleColor", typeof(Color), typeof(RoundGauge), new PropertyMetadata(Colors.Yellow, ValueChangedCallback));
			LowColorProperty = DependencyProperty.Register("LowColor", typeof(Color), typeof(RoundGauge), new PropertyMetadata(Colors.Red, ValueChangedCallback));
		}
		static void TextChangedCallback(object obj, DependencyPropertyChangedEventArgs args)
		{
			var r = obj as RoundGauge;
			if (r == null)
				return;

			r.onTextChanged();
		}
		void onTextChanged()
		{
			tbCaptionText.Text = Text;
		}
		static void CurrentValueChangedCallback(object obj, DependencyPropertyChangedEventArgs args)
		{
			var r = obj as RoundGauge;
			if (r == null)
				return;

			r.onCurrentValueChanged();
		}
		static void ValueChangedCallback(object obj, DependencyPropertyChangedEventArgs args)
		{
			var r = obj as RoundGauge;
			if (r == null)
				return;

			if (r.Visibility == Visibility.Collapsed)
				return;

			r.DispatchRefresh();
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
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
		{
			return base.OnCreateAutomationPeer();
		}

		protected virtual CustomContextMenu CreateContextMenu()
		{
			CustomContextMenu contextMenu = new CustomContextMenu();
			return contextMenu;
		}

		void onMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			clickCount++;
			lastArgs = e;
			sbDoubleClick.Begin();
		}

		public event MouseEventHandler MouseDoubleClick;
		protected void OnMouseDoubleClick(MouseEventArgs e)
		{
			if (this.MouseDoubleClick != null)
			{
				this.MouseDoubleClick(this, e);
			}
		}


		void sbDoubleClick_Completed(object sender, EventArgs e)
		{
			if (clickCount > 1)
			{
				this.OnMouseDoubleClick(lastArgs);
			}
			clickCount = 0;
		}
		void onLoaded(object sender, RoutedEventArgs e)
		{
			DispatchRefresh();
		}
		void onSizeChanged(object sender, SizeChangedEventArgs e)
		{
			DispatchRefresh();
		}
		/// <summary>
		/// Current Value
		/// </summary>
		public double CurrentValue
		{
			get { return (double)GetValue(CurrentValueProperty); }
			set { SetValue(CurrentValueProperty, value); }
		}

		/// <summary>
		/// RoundedCurrentValue
		/// </summary>
		public double RoundedCurrentValue
		{
			get
			{
				double value = Math.Min(CurrentValue, MaxValue);
				value = Math.Max(value, MinValue);
				return value;
			}
		}

		/// <summary>
		/// Тект, который отображается в качестве подписи на циферблате
		/// </summary>
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		/// <summary>
		/// Divisions Format String
		/// </summary>
		public String DivisionsFormatString
		{
			get { return (string)GetValue(DivisionsFormatStringProperty); }
			set { SetValue(DivisionsFormatStringProperty, value); }
		}
		/// <summary>
		/// Current Value Format String
		/// </summary>
		public String CurrentValueFormatString
		{
			get { return (string)GetValue(CurrentValueFormatStringProperty); }
			set { SetValue(CurrentValueFormatStringProperty, value); }
		}

		/// <summary>
		/// High Value
		/// </summary>
		public double HighValue
		{
			get { return (double)GetValue(HighValueProperty); }
			set { SetValue(HighValueProperty, value); }
		}
		/// <summary>
		/// Low Value
		/// </summary>
		public double LowValue
		{
			get { return (double)GetValue(LowValueProperty); }
			set { SetValue(LowValueProperty, value); }
		}

		/// <summary>
		/// Hide Percentage In Main Divisions
		/// </summary>
		public bool HidePercentageInDivisions
		{
			get { return (bool)GetValue(HidePercentageInDivisionsProperty); }
			set { SetValue(HidePercentageInDivisionsProperty, value); }
		}

		/// <summary>
		/// Hide Percentage In Current Value
		/// </summary>
		public bool HidePercentageInValue
		{
			get { return (bool)GetValue(HidePercentageInValueProperty); }
			set { SetValue(HidePercentageInValueProperty, value); }
		}
		/// <summary>
		/// Min Value (Default=0.0)
		/// </summary>
		public double MinValue
		{
			get { return (double)GetValue(MinValueProperty); }
			set { SetValue(MinValueProperty, value); }
		}

		/// <summary>
		/// Max Value (Default=100.0)
		/// </summary>
		public double MaxValue
		{
			get { return (double)GetValue(MaxValueProperty); }
			set { SetValue(MaxValueProperty, value); }
		}

		/// <summary>
		/// Color for High to Max (Default=Colors.Green)
		/// </summary>
		public Color HighColor
		{
			get { return (Color)GetValue(HighColorProperty); }
			set { SetValue(HighColorProperty, value); }
		}
		/// <summary>
		/// Color for Low to High (Default=Colors.Yellow)
		/// </summary>
		public Color MiddleColor
		{
			get { return (Color)GetValue(MiddleColorProperty); }
			set { SetValue(MiddleColorProperty, value); }
		}
		/// <summary>
		/// Color for Min to Low (Default=Colors.Red)
		/// </summary>
		public Color LowColor
		{
			get { return (Color)GetValue(LowColorProperty); }
			set { SetValue(LowColorProperty, value); }
		}
		/// <summary>
		/// Foreground color for Text (Default=Blue)
		/// </summary>
		public Color TextColor
		{
			get { return (Color)GetValue(TextColorProperty); }
			set { SetValue(TextColorProperty, value); }
		}
		/// <summary>
		/// Foreground color for values
		/// </summary>
		public Color ForeColor
		{
			get { return (Color)GetValue(ForeColorProperty); }
			set { SetValue(ForeColorProperty, value); }
		}
		/// <summary>
		/// Background Color
		/// </summary>
		public Color BackgroundColor
		{
			get { return (Color)GetValue(BackgroundColorProperty); }
			set { SetValue(BackgroundColorProperty, value); }
		}
		/// <summary>
		/// Meter Margin in %, Default=0% (i.e. no margin)
		/// </summary>
		public double MeterMargin
		{
			get { return (double)GetValue(MeterMarginProperty); }
			set { SetValue(MeterMarginProperty, value); }
		}
		/// <summary>
		/// Meter Background Color (Default=Colors.Gray)
		/// </summary>
		public Color MeterBackgroundColor
		{
			get { return (Color)GetValue(MeterBackgroundColorProperty); }
			set { SetValue(MeterBackgroundColorProperty, value); }
		}
		/// <summary>
		/// Tooltip text
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

		double MeterMarginBounded()
		{
			double result = MeterMargin;
			if (result < 0.0)
			{
				result = 0.0;
				MeterMargin = result;
			}
			else if (result > 100.0)
			{
				result = 100.0;
				MeterMargin = result;
			}
			return result;
		}
		double MeterSize
		{
			get
			{
				double meter_size = Math.Min(this.ActualWidth, this.ActualHeight);
				meter_size = meter_size - (meter_size / 100.0) * MeterMarginBounded();
				meter_size = Math.Max(meter_size, 0);
				return meter_size;
			}
		}
		double MeterTop
		{
			get
			{
				double meter_top = this.ActualHeight / 2 - MeterSize / 2;
				meter_top = Math.Max(meter_top, 0);
				return meter_top;
			}
		}
		double MeterLeft
		{
			get
			{
				double meter_left = this.ActualWidth / 2 - MeterSize / 2;
				meter_left = Math.Max(meter_left, 0);
				return meter_left;
			}
		}

		readonly TextBlock tbCurrentValue = new TextBlock();
		readonly TextBlock tbCaptionText = new TextBlock();
		readonly Ellipse ellipseOuter = new Ellipse();
		readonly Ellipse ellipseCalibration = new Ellipse();
		readonly Path pathLow = new Path();
		readonly Path pathMiddle = new Path();
		readonly Path pathHigh = new Path();

		readonly Ellipse ellipseCenter = new Ellipse();
		readonly Border borderCurrentValue = new Border();
		readonly Line lineArrow1 = new Line();
		readonly Line lineArrow2 = new Line();
		readonly Storyboard sbDoubleClick = new Storyboard();
		readonly double fromAngle = 135.0;
		readonly double toAngle = 405.0;

		int clickCount = 0;
		MouseButtonEventArgs lastArgs;
		// Толщина границы спидометра
		double outerSize = 0;


		public RoundGauge()
		{
			this.Children.Add(ellipseOuter);

			this.Children.Add(ellipseCalibration);

			this.Children.Add(pathLow);
			this.Children.Add(pathMiddle);
			this.Children.Add(pathHigh);

			this.ellipseCenter.Effect = new System.Windows.Media.Effects.DropShadowEffect();
			this.Children.Add(ellipseCenter);

			this.Children.Add(borderCurrentValue);

			this.Children.Add(tbCaptionText);

			// Рисуем стрелки
			lineArrow1.Effect = new System.Windows.Media.Effects.DropShadowEffect();
			lineArrow1.Stroke = new SolidColorBrush(Colors.Black);
			lineArrow1.StrokeEndLineCap = PenLineCap.Flat;
			lineArrow2.Effect = lineArrow1.Effect;
			lineArrow2.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xF7, 0x36, 0x31));
			lineArrow2.StrokeEndLineCap = PenLineCap.Round;

			this.Children.Add(lineArrow1);
			this.Children.Add(lineArrow2);



			this.sbDoubleClick.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250));
			this.sbDoubleClick.Completed += new EventHandler(sbDoubleClick_Completed);
			this.Resources.Add("sbDoubleClick", sbDoubleClick);

			this.SizeChanged += new SizeChangedEventHandler(onSizeChanged);
			this.Loaded += new RoutedEventHandler(onLoaded);

			this.MouseLeftButtonDown += new MouseButtonEventHandler(onMouseLeftButtonDown);
			this.MouseLeave += new MouseEventHandler(OnMouseLeave);
			this.MouseEnter += new MouseEventHandler(OnMouseEnter);
		}

		volatile object LockObj = new object();
		volatile int NeedRefresh = 0;
		public void DispatchRefresh()
		{
			Refresh();
			
			//lock (LockObj)
			//{
			//  NeedRefresh|=1;
			//  if ((NeedRefresh & 2) == 0)
			//    Dispatcher.BeginInvoke(RefreshInternal);
			//}
		}
		void RefreshInternal()
		{
			do
			{
				lock (LockObj)
				{
					NeedRefresh|=2;
					if ((NeedRefresh & 1) == 0)
					{	
						NeedRefresh=0;
						return;
					}
				}
				Refresh();
				lock (LockObj)
				{
					NeedRefresh&=1;
				}
			}
			while (true);
		}

		public virtual void Refresh()
		{
			var color = BackgroundColor;
			// if (color != Colors.Transparent)
			this.Background = new SolidColorBrush(color);

			// Граница спидометра
			// Внешний контур
			ellipseOuter.Height = MeterSize;
			ellipseOuter.Width = MeterSize;

			ellipseOuter.Stroke = new SolidColorBrush(Colors.Black);
			ellipseOuter.StrokeThickness = MeterSize * 0.005;
			ellipseOuter.SetValue(Canvas.TopProperty, MeterTop);
			ellipseOuter.SetValue(Canvas.LeftProperty, MeterLeft);

			outerSize = ellipseOuter.StrokeThickness; // outer_ellipse1.StrokeThickness + outer_ellipse2.StrokeThickness;

			// Область под циферблат
			ellipseCalibration.Height = MeterSize - outerSize * 2 + 2;
			ellipseCalibration.Width = MeterSize - outerSize * 2 + 2;

			LinearGradientBrush calibration_brush1 = new LinearGradientBrush();
			calibration_brush1.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF) });
			calibration_brush1.GradientStops.Add(new GradientStop() { Color = MeterBackgroundColor, Offset = 1 });
			ellipseCalibration.Fill = calibration_brush1;
			ellipseCalibration.SetValue(Canvas.TopProperty, MeterTop + outerSize - 1);
			ellipseCalibration.SetValue(Canvas.LeftProperty, MeterLeft + outerSize - 1);

			// Центр спидометра
			double cX = MeterSize / 2;
			double cY = MeterSize / 2;
			double center_size = MeterSize * 0.1;

			ellipseCenter.Height = center_size;
			ellipseCenter.Width = center_size;

			ellipseCenter.Fill = new SolidColorBrush(Colors.Black);
			ellipseCenter.SetValue(Canvas.TopProperty, MeterTop + cX - center_size / 2);
			ellipseCenter.SetValue(Canvas.LeftProperty, MeterLeft + cY - center_size / 2);

			// Рисуем значения
			DrawCalibration();

			// Цвет текущего значения
			tbCurrentValue.Foreground = new SolidColorBrush(this.ForeColor);

			borderCurrentValue.BorderBrush = new SolidColorBrush(Colors.Black);
			borderCurrentValue.BorderThickness = new Thickness(MeterSize * 0.0025);

			tbCurrentValue.Width = MeterSize * 0.25;
			tbCurrentValue.Height = MeterSize * 0.06;
			tbCurrentValue.FontSize = tbCurrentValue.Height * 0.75;
			tbCurrentValue.TextWrapping = TextWrapping.NoWrap;
			tbCurrentValue.TextAlignment = TextAlignment.Right;

			borderCurrentValue.Child = tbCurrentValue;
			borderCurrentValue.Padding = new Thickness(tbCurrentValue.Height * 0.1);

			borderCurrentValue.SetValue(Canvas.TopProperty, MeterTop + cY + center_size + MeterSize * 0.15);
			borderCurrentValue.SetValue(Canvas.LeftProperty, MeterLeft + cX - tbCurrentValue.Width / 2);
			borderCurrentValue.Background = new SolidColorBrush(Color.FromArgb(25, 0x33, 0x33, 0x33));

			// Рисуем название
			tbCaptionText.FontSize = MeterSize * 0.04;
			tbCaptionText.Foreground = new SolidColorBrush(TextColor);
			tbCaptionText.SetValue(Canvas.TopProperty, MeterTop + cY + center_size + MeterSize * 0.05);
			tbCaptionText.SetValue(Canvas.LeftProperty, MeterLeft + cX - tbCaptionText.ActualWidth / 2);

			onTextChanged();
			onCurrentValueChanged();
		}

		void onCurrentValueChanged()
		{
			string num = string.Empty;
			try
			{
				num = CurrentValue.ToString(this.CurrentValueFormatString);
			}
			catch
			{
				num = CurrentValue.ToString();
			}

			if (HidePercentageInValue)
			{
				num = num.Replace("%", String.Empty);
			}
			tbCurrentValue.Text = num;

			// Рисуем стрелку
			DrawArrow(RoundedCurrentValue);
		}
		void DrawArrow(double value)
		{
			// Центр спидометра
			double cX = MeterSize / 2;
			double cY = MeterSize / 2;
			double arrow_length = MeterSize * 0.1;
			double arrow_end_length = MeterSize * 0.15;

			// Рисуем стрелку1
			lineArrow1.StrokeThickness = MeterSize * 0.01;
			lineArrow1.X1 = MeterLeft + cX;
			lineArrow1.Y1 = MeterTop + cY;
			lineArrow1.X2 = (MeterLeft + cX + arrow_length * Math.Cos(GetRadian(GetAngleByValue(value))));
			lineArrow1.Y2 = (MeterTop + cY + arrow_length * Math.Sin(GetRadian(GetAngleByValue(value))));

			// Рисуем стрелку2
			lineArrow2.StrokeThickness = lineArrow1.StrokeThickness;
			lineArrow2.X1 = (MeterLeft + cX + arrow_length * Math.Cos(GetRadian(GetAngleByValue(value))));
			lineArrow2.Y1 = (MeterTop + cY + arrow_length * Math.Sin(GetRadian(GetAngleByValue(value))));
			lineArrow2.X2 = (MeterLeft + cX + (arrow_length + arrow_end_length) * Math.Cos(GetRadian(GetAngleByValue(value))));
			lineArrow2.Y2 = (MeterTop + cY + (arrow_length + arrow_end_length) * Math.Sin(GetRadian(GetAngleByValue(value))));
		}

		/// <summary>
		/// Main Divisions Count (Default=10)
		/// </summary>
		public int DivisionsCount
		{
			get { return (int)GetValue(DivisionsCountProperty); }
			set { SetValue(DivisionsCountProperty, value); }
		}

		/// <summary>
		/// Sub Divisions Count (Default=5)
		/// </summary>
		public int SubDivisionsCount
		{
			get { return (int)GetValue(SubDivisionsCountProperty); }
			set { SetValue(SubDivisionsCountProperty, value); }
		}
		Point CalculateRotation(Point center, double angle, double radius)
		{
			double x = (center.X + radius * Math.Cos(GetRadian(angle)));
			double y = (center.Y + radius * Math.Sin(GetRadian(angle)));
			return new Point(x, y);
		}

		double GetAngleByValue(double value)
		{
			value = Math.Min(value, MaxValue);
			value = Math.Max(value, MinValue);

			double angle = fromAngle;
			if ((MaxValue - MinValue) > 0)
			{
				// Отношение:
				// m_MaxValue - m_MinValue  => m_ToAngle - m_FromAngle
				// value - m_MinValue       => X - m_FromAngle
				angle = ((toAngle - fromAngle) * (value - MinValue)) / (MaxValue - MinValue) + fromAngle;
			}
			return angle;
		}

		Border[] divisions = null;
		Border[] subdivisions = null;
		TextBlock[] valueTexts = null;
		/// <summary>
		/// Draws the Ruler
		/// </summary>
		private void DrawCalibration()
		{
			if (divisions != null)
			{
				foreach (var b in divisions)
				{
					this.Children.Remove(b);
				}
				foreach (var b in subdivisions)
				{
					this.Children.Remove(b);
				}
				foreach (var vt in valueTexts)
				{
					this.Children.Remove(vt);
				}
				divisions = null;
				subdivisions = null;
				valueTexts = null;
			}

			if (MeterSize <= 0)
				return;


			int noOfParts = this.DivisionsCount + 1;
			int noOfIntermediates = this.SubDivisionsCount;
			double currentAngle_rad = GetRadian(fromAngle);
			double currentAngle = fromAngle;

			// отступ от края
			double offset = outerSize * 4;
			// Длина риски для основного значения
			double division_length = MeterSize / 40;
			// Внешний радиус для риски
			double division_outer_radius = MeterSize / 2 - offset;

			// Сектор в котором отображаются значения
			double totalAngle = toAngle - fromAngle;
			// Угол между соседними рисками (основными или промежуточными)
			double incr = ((totalAngle) / ((noOfParts - 1) * (noOfIntermediates + 1)));
			double incr_rad = GetRadian(((totalAngle) / ((noOfParts - 1) * (noOfIntermediates + 1))));

			Brush divisionsBrush = new SolidColorBrush(Colors.Black);
			Brush subdivisionsBrush = new SolidColorBrush(Colors.Black);

			double rulerValue = MinValue;

			// Центр спидометра
			double cX = MeterSize / 2;
			double cY = MeterSize / 2;
			Point center = new Point(cX, cY);

			if (LowValue != double.MinValue && HighValue != double.MinValue && LowValue <= HighValue)
			{
				// Толщина линий
				double line_size = offset;
				// Внешний радиус для рисок - Смещаем риски ближе к центру
				division_outer_radius -= offset / 2;
				// Дуги для допустимых значений
				double arc_radius = division_outer_radius;

				SolidColorBrush lowBrush;
				SolidColorBrush hiBrush;
				SolidColorBrush midBrush;

				lowBrush = new SolidColorBrush(LowColor);
				midBrush = new SolidColorBrush(MiddleColor);
				hiBrush = new SolidColorBrush(HighColor);

				// Дуга между минимумом и нижним значением
				pathLow.StrokeThickness = line_size;
				pathLow.Stroke = lowBrush;
				PathGeometry low_geometry = new PathGeometry();
				pathLow.Data = low_geometry;
				PathFigure low_Figure = new PathFigure();
				low_Figure.StartPoint = CalculateRotation(center, GetAngleByValue(MinValue), arc_radius);
				low_geometry.Figures.Add(low_Figure);

				ArcSegment low_arc = new ArcSegment();
				double low_angle = GetAngleByValue(LowValue);
				low_arc.Point = CalculateRotation(center, low_angle, arc_radius);
				low_arc.Size = new Size(arc_radius, arc_radius);
				low_arc.SweepDirection = SweepDirection.Clockwise;
				low_arc.IsLargeArc = (low_angle - fromAngle) > 180;
				low_Figure.Segments.Add(low_arc);

				// Дуга между нижним значением и верхним значением
				pathMiddle.StrokeThickness = line_size;
				pathMiddle.Stroke = midBrush;
				PathGeometry mid_geometry = new PathGeometry();
				pathMiddle.Data = mid_geometry;
				PathFigure mid_Figure = new PathFigure();
				mid_Figure.StartPoint = low_arc.Point;
				mid_geometry.Figures.Add(mid_Figure);

				ArcSegment mid_arc = new ArcSegment();
				double hi_angle = GetAngleByValue(HighValue);
				mid_arc.Point = CalculateRotation(center, hi_angle, arc_radius);
				mid_arc.Size = new Size(arc_radius, arc_radius);
				mid_arc.SweepDirection = SweepDirection.Clockwise;
				mid_arc.IsLargeArc = (hi_angle - low_angle) > 180;
				mid_Figure.Segments.Add(mid_arc);

				// Дуга между верхним значением и максимумом
				pathHigh.StrokeThickness = line_size;
				pathHigh.Stroke = hiBrush;
				PathGeometry hi_geometry = new PathGeometry();
				pathHigh.Data = hi_geometry;
				PathFigure hi_Figure = new PathFigure();
				hi_Figure.StartPoint = mid_arc.Point;
				hi_geometry.Figures.Add(hi_Figure);

				ArcSegment hi_arc = new ArcSegment();
				hi_arc.Point = CalculateRotation(center, toAngle, arc_radius);
				hi_arc.Size = new Size(arc_radius, arc_radius);
				hi_arc.SweepDirection = SweepDirection.Clockwise;
				hi_arc.IsLargeArc = (toAngle - hi_angle) > 180;
				hi_Figure.Segments.Add(hi_arc);

				pathLow.SetValue(Canvas.TopProperty, MeterTop);
				pathLow.SetValue(Canvas.LeftProperty, MeterLeft);
				pathMiddle.SetValue(Canvas.TopProperty, MeterTop);
				pathMiddle.SetValue(Canvas.LeftProperty, MeterLeft);
				pathHigh.SetValue(Canvas.TopProperty, MeterTop);
				pathHigh.SetValue(Canvas.LeftProperty, MeterLeft);
				pathLow.Effect = new System.Windows.Media.Effects.BlurEffect();
				pathMiddle.Effect = new System.Windows.Media.Effects.BlurEffect();
				pathHigh.Effect = new System.Windows.Media.Effects.BlurEffect();
			}

			// Внутренний радиус для риски
			double division_inner_radius = division_outer_radius - division_length;
			double angle = fromAngle;
			divisions = new Border[noOfParts];
			valueTexts = new TextBlock[noOfParts];
			subdivisions = new Border[(noOfIntermediates + 1) * noOfParts];
			var ForeBrush = new SolidColorBrush(this.ForeColor);
			for (int i = 0; i < noOfParts; i++)
			{
				// Рисуем риску
				Border border = new Border();
				border.BorderThickness = new Thickness(2);
				border.BorderBrush = divisionsBrush;
				border.Width = division_outer_radius - division_inner_radius;
				border.Height = 2;
				border.SetValue(Canvas.TopProperty, MeterTop + cY + division_outer_radius * Math.Sin(currentAngle_rad));
				border.SetValue(Canvas.LeftProperty, MeterLeft + cX + division_outer_radius * Math.Cos(currentAngle_rad));
				border.RenderTransform = new RotateTransform() { Angle = currentAngle + 180 };
				border.Effect = new System.Windows.Media.Effects.DropShadowEffect();
				this.Children.Add(border);
				divisions[i] = border;

				//Line divisionLine = new Line();
				//divisionLine.Stroke = divisionsBrush;
				//divisionLine.StrokeThickness = 2;
				//divisionLine.X1 = (m_MeterLeft + cX + division_outer_radius * Math.Cos(currentAngle));
				//divisionLine.Y1 = (m_MeterTop + cY + division_outer_radius * Math.Sin(currentAngle));
				//divisionLine.X2 = (m_MeterLeft + cX + division_inner_radius * Math.Cos(currentAngle));
				//divisionLine.Y2 = (m_MeterTop + cY + division_inner_radius * Math.Sin(currentAngle));
				//divisionLine.Effect = new System.Windows.Media.Effects.DropShadowEffect();
				//this.Children.Add(divisionLine);

				// Рисуем значение
				double text_radius = division_inner_radius - MeterSize * 0.08;
				double tx = (cX + text_radius * Math.Cos(currentAngle_rad));
				double ty = (cY + text_radius * Math.Sin(currentAngle_rad));

				if (i > 0)
					rulerValue += (double)((MaxValue - MinValue) / (noOfParts - 1));
				double value = Math.Round(rulerValue, 2);

				TextBlock valueText = new TextBlock();
				valueText.Foreground = ForeBrush;
				valueText.FontSize = MeterSize * 0.08;
				String num = rulerValue.ToString(DivisionsFormatString);
				if (HidePercentageInDivisions)
				{
					num = num.Replace("%", String.Empty);
				}
				valueText.Text = num;

				this.Children.Add(valueText);
				valueTexts[i] = valueText;

				// Центрируем значение - чтобы центр текста попал в вычисленную координату
				if (valueText.ActualWidth.CompareTo(double.NaN) != 0)
				{
					valueText.SetValue(Canvas.LeftProperty, MeterLeft + tx - valueText.ActualWidth / 2);
				}
				else
				{
					valueText.SetValue(Canvas.LeftProperty, MeterLeft + tx);
				}
				if (valueText.ActualHeight.CompareTo(double.NaN) != 0)
				{
					valueText.SetValue(Canvas.TopProperty, MeterTop + ty - valueText.ActualHeight / 2);
				}
				else
				{
					valueText.SetValue(Canvas.TopProperty, MeterTop + ty);
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
					sub_border.SetValue(Canvas.TopProperty, MeterTop + cY + subdivision_outer_radius * Math.Sin(currentAngle_rad));
					sub_border.SetValue(Canvas.LeftProperty, MeterLeft + cX + subdivision_outer_radius * Math.Cos(currentAngle_rad));
					sub_border.RenderTransform = new RotateTransform() { Angle = currentAngle + 180 };
					sub_border.Effect = new System.Windows.Media.Effects.DropShadowEffect();
					this.Children.Add(sub_border);
					subdivisions[(noOfIntermediates + 1) * i + j] = sub_border;

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
					//this.Children.Add(subivisionLine);
				}
			}
		}
		/// <summary>
		/// Converts the given degree to radian.
		/// </summary>
		/// <param name="theta"></param>
		/// <returns></returns>
		static double GetRadian(double theta)
		{
			return theta * Math.PI / 180F;
		}
	}
}
