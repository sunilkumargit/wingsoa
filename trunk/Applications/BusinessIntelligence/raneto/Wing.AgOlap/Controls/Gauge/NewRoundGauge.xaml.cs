using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Ranet.AgOlap.Controls.Gauge
{
    public partial class NewRoundGauge : UserControl
    {
        public static readonly DependencyProperty MinValueProperty;
        public static readonly DependencyProperty CurrentValueProperty;
        public static readonly DependencyProperty MaxValueProperty;
        public static readonly DependencyProperty TextProperty;
        public static readonly DependencyProperty TooltipTextProperty;
        public static readonly DependencyProperty DivisionsFormatStringProperty;
        public static readonly DependencyProperty CurrentValueFormatStringProperty;
        public static readonly DependencyProperty DivisionsCountProperty;
        public static readonly DependencyProperty SubDivisionsCountProperty;
        public static readonly DependencyProperty HighColorProperty;
        public static readonly DependencyProperty MiddleColorProperty;
        public static readonly DependencyProperty LowColorProperty;
        public static readonly DependencyProperty ForeColorProperty;
        public static readonly DependencyProperty BackgroundColorProperty;
        public static readonly DependencyProperty TextColorProperty;

        static NewRoundGauge()
        {
            ForeColorProperty = DependencyProperty.Register("ForeColor", typeof(Color), typeof(RoundGauge), new PropertyMetadata(Colors.Black, ValueChangedCallback));
            TextColorProperty = DependencyProperty.Register("TextColor", typeof(Color), typeof(RoundGauge), new PropertyMetadata(Colors.Blue, ValueChangedCallback));
            BackgroundColorProperty = DependencyProperty.Register("BackgroundColor", typeof(Color), typeof(RoundGauge), new PropertyMetadata(Colors.Transparent, ValueChangedCallback));
            MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(RoundGauge), new PropertyMetadata((double)0.0, ValueChangedCallback));
            CurrentValueProperty = DependencyProperty.Register("CurrentValue", typeof(double), typeof(RoundGauge), new PropertyMetadata((double)0.0, CurrentValueChangedCallback));
            MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(RoundGauge), new PropertyMetadata((double)100.0, ValueChangedCallback));
            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(RoundGauge), new PropertyMetadata(string.Empty, TextChangedCallback));
            TooltipTextProperty = DependencyProperty.Register("TooltipText", typeof(string), typeof(RoundGauge), new PropertyMetadata(string.Empty, ValueChangedCallback));
            DivisionsFormatStringProperty = DependencyProperty.Register("DivisionsFormatString", typeof(string), typeof(RoundGauge), new PropertyMetadata("0.##", ValueChangedCallback));
            CurrentValueFormatStringProperty = DependencyProperty.Register("CurrentValueFormatString", typeof(string), typeof(RoundGauge), new PropertyMetadata("0.##", ValueChangedCallback));
            DivisionsCountProperty = DependencyProperty.Register("DivisionsCount", typeof(int), typeof(RoundGauge), new PropertyMetadata((int)10, ValueChangedCallback));
            SubDivisionsCountProperty = DependencyProperty.Register("SubDivisionsCount", typeof(int), typeof(RoundGauge), new PropertyMetadata((int)5, ValueChangedCallback));
            HighColorProperty = DependencyProperty.Register("HighColor", typeof(Color), typeof(RoundGauge), new PropertyMetadata(Colors.Green, ValueChangedCallback));
            MiddleColorProperty = DependencyProperty.Register("MiddleColor", typeof(Color), typeof(RoundGauge), new PropertyMetadata(Colors.Yellow, ValueChangedCallback));
            LowColorProperty = DependencyProperty.Register("LowColor", typeof(Color), typeof(RoundGauge), new PropertyMetadata(Colors.Red, ValueChangedCallback));
        }


        private bool isInitialValueSet = false;
        private int animatingSpeedFactor = 6;

        public NewRoundGauge()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(NewRoundGauge_Loaded);
            this.SizeChanged += new SizeChangedEventHandler(NewRoundGauge_SizeChanged);
            DrawScale();
            DrawRangeIndicator();

        }

        void NewRoundGauge_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //this.Refresh();
        }


        void NewRoundGauge_Loaded(object sender, RoutedEventArgs e)
        {
            //this.Refresh(); 
        }

        static void TextChangedCallback(object obj, DependencyPropertyChangedEventArgs args)
        {
            var r = obj as NewRoundGauge;
            if (r == null)
                return;

            r.onTextChanged();
        }
        void onTextChanged()
        {
            captionText.Text = Text;
        }
        static void CurrentValueChangedCallback(object obj, DependencyPropertyChangedEventArgs args)
        {
            var r = obj as NewRoundGauge;
            if (r == null)
                return;

            r.OnCurrentValueChanged(args);
        }
        static void ValueChangedCallback(object obj, DependencyPropertyChangedEventArgs args)
        {
            var r = obj as NewRoundGauge;
            if (r == null)
                return;

            if (r.Visibility == Visibility.Collapsed)
                return;

            //r.Refresh();
        }

        readonly double fromAngle = 135.0;
        readonly double toAngle = 405.0;
        private readonly double OptimalRangeStartValue = 300;
        private readonly double OptimalRangeEndValue = 700;
        private readonly double ScaleSweepAngle = 300;
        private readonly double Radius = 150;
        private readonly double RangeIndicatorRadius = 80;
        private readonly double RangeIndicatorThickness = 5;
        private readonly Color AboveOptimalRangeColor = Colors.Red;
        private readonly Color OptimalRangeColor = Colors.Green;
        private readonly Color BelowOptimalRangeColor = Colors.Yellow;
        private Double arcradius1;
        private Double arcradius2;

        public virtual void OnCurrentValueChanged(DependencyPropertyChangedEventArgs e)
        {
            //Validate and set the new value
            double newValue = (double)e.NewValue;
            double oldValue = (double)e.OldValue;

            if (newValue > this.MaxValue)
            {
                newValue = this.MaxValue;
            }
            else if (newValue < this.MinValue)
            {
                newValue = this.MinValue;
            }

            if (oldValue > this.MaxValue)
            {
                oldValue = this.MaxValue;
            }
            else if (oldValue < this.MinValue)
            {
                oldValue = this.MinValue;
            }

            if (Pointer != null)
            {
                double db1 = 0;
                Double oldcurr_realworldunit = 0;
                Double newcurr_realworldunit = 0;
                Double realworldunit = (ScaleSweepAngle / (MaxValue - MinValue));
                //Resetting the old value to min value the very first time.
                if (oldValue == 0 && !isInitialValueSet)
                {
                    oldValue = MinValue;
                    isInitialValueSet = true;

                }
                if (oldValue < 0)
                {
                    db1 = MinValue + Math.Abs(oldValue);
                    oldcurr_realworldunit = ((double)(Math.Abs(db1 * realworldunit)));
                }
                else
                {
                    db1 = Math.Abs(MinValue) + oldValue;
                    oldcurr_realworldunit = ((double)(db1 * realworldunit));
                }
                if (newValue < 0)
                {
                    db1 = MinValue + Math.Abs(newValue);
                    newcurr_realworldunit = ((double)(Math.Abs(db1 * realworldunit)));
                }
                else
                {
                    db1 = Math.Abs(MinValue) + newValue;
                    newcurr_realworldunit = ((double)(db1 * realworldunit));
                }

                Double oldcurrentvalueAngle = (fromAngle + oldcurr_realworldunit);
                Double newcurrentvalueAngle = (fromAngle + newcurr_realworldunit);

                //Animate the pointer from the old value to the new value
                AnimatePointer(oldcurrentvalueAngle, newcurrentvalueAngle);

            }

        }

        /// <summary>
        /// Animates the pointer to the current value to the new one
        /// </summary>
        /// <param name="oldcurrentvalueAngle"></param>
        /// <param name="newcurrentvalueAngle"></param>
        void AnimatePointer(double oldcurrentvalueAngle, double newcurrentvalueAngle)
        {
            if (Pointer != null)
            {
                DoubleAnimation da = new DoubleAnimation();
                da.From = oldcurrentvalueAngle;
                da.To = newcurrentvalueAngle;

                double animDuration = Math.Abs(oldcurrentvalueAngle - newcurrentvalueAngle) * animatingSpeedFactor;
                da.Duration = new Duration(TimeSpan.FromMilliseconds(animDuration));

                Storyboard sb = new Storyboard();
                sb.Completed += new EventHandler(sb_Completed);
                sb.Children.Add(da);
                Storyboard.SetTarget(da, Pointer);
                Storyboard.SetTargetProperty(da, new PropertyPath("(Path.RenderTransform).(TransformGroup.Children)[0].(RotateTransform.Angle)"));

                if (newcurrentvalueAngle != oldcurrentvalueAngle)
                {
                    sb.Begin();
                }
            }
        }


        /// <summary>
        /// Move pointer without animating
        /// </summary>
        /// <param name="angleValue"></param>
        void MovePointer(double angleValue)
        {
            if (Pointer != null)
            {
                TransformGroup tg = Pointer.RenderTransform as TransformGroup;
                RotateTransform rt = tg.Children[0] as RotateTransform;
                rt.Angle = angleValue;

            }
        }

        /// <summary>
        /// Switch on the Range indicator light after the pointer completes animating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void sb_Completed(object sender, EventArgs e)
        {
            if (this.CurrentValue > OptimalRangeEndValue)
            {
                RangeIndicatorLight.Fill = GetRangeIndicatorGradEffect(AboveOptimalRangeColor);

            }
            else if (this.CurrentValue <= OptimalRangeEndValue && this.CurrentValue >= OptimalRangeStartValue)
            {
                RangeIndicatorLight.Fill = GetRangeIndicatorGradEffect(OptimalRangeColor);

            }
            else if (this.CurrentValue < OptimalRangeStartValue)
            {
                RangeIndicatorLight.Fill = GetRangeIndicatorGradEffect(BelowOptimalRangeColor);
            }
        }

        /// <summary>
        /// Get gradient brush effect for the range indicator light
        /// </summary>
        /// <param name="gradientColor"></param>
        /// <returns></returns>
        private GradientBrush GetRangeIndicatorGradEffect(Color gradientColor)
        {

            LinearGradientBrush gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0, 0);
            gradient.EndPoint = new Point(1, 1);
            GradientStop color1 = new GradientStop();
            if (gradientColor == Colors.Transparent)
            {
                color1.Color = gradientColor;
            }
            else
                color1.Color = Colors.LightGray;

            color1.Offset = 0.2;
            gradient.GradientStops.Add(color1);
            GradientStop color2 = new GradientStop();
            color2.Color = gradientColor; color2.Offset = 0.5;
            gradient.GradientStops.Add(color2);
            GradientStop color3 = new GradientStop();
            color3.Color = gradientColor; color3.Offset = 0.8;
            gradient.GradientStops.Add(color3);
            return gradient;
        }

        private readonly double MajorDivisionsCount = 10;
        private readonly double MinorDivisionsCount = 5;
        private readonly int ScaleValuePrecision = 5;
        private readonly Color MajorTickColor = Colors.LightGray;
        private readonly double ScaleRadius = 100;
        private readonly double ScaleLabelRadius = 110;

        private void DrawScale()
        {
            //Calculate one major tick angle 
            Double majorTickUnitAngle = ScaleSweepAngle / MajorDivisionsCount;

            //Obtaining One minor tick angle 
            Double minorTickUnitAngle = ScaleSweepAngle / MinorDivisionsCount;

            //Obtaining One major ticks value
            Double majorTicksUnitValue = (MaxValue - MinValue) / MajorDivisionsCount;
            majorTicksUnitValue = Math.Round(majorTicksUnitValue, ScaleValuePrecision);

            Double minvalue = MinValue; ;

            // Drawing Major scale ticks
            for (Double i = fromAngle; i <= (fromAngle + ScaleSweepAngle); i = i + majorTickUnitAngle)
            {

                //Majortick is drawn as a rectangle 
                Rectangle majortickrect = new Rectangle();
                majortickrect.Height = 10;
                majortickrect.Width = 3;
                majortickrect.Fill = new SolidColorBrush(MajorTickColor);
                Point p = new Point(0.5, 0.5);
                majortickrect.RenderTransformOrigin = p;
                majortickrect.HorizontalAlignment = HorizontalAlignment.Center;
                majortickrect.VerticalAlignment = VerticalAlignment.Center;

                TransformGroup majortickgp = new TransformGroup();
                RotateTransform majortickrt = new RotateTransform();

                //Obtaining the angle in radians for calulating the points
                Double i_radian = (i * Math.PI) / 180;
                majortickrt.Angle = i;
                majortickgp.Children.Add(majortickrt);
                TranslateTransform majorticktt = new TranslateTransform();

                //Finding the point on the Scale where the major ticks are drawn
                //here drawing the points with center as (0,0)
                majorticktt.X = (int)((ScaleRadius) * Math.Cos(i_radian));
                majorticktt.Y = (int)((ScaleRadius) * Math.Sin(i_radian));

                //Points for the textblock which hold the scale value
                TranslateTransform majorscalevaluett = new TranslateTransform();
                //here drawing the points with center as (0,0)
                majorscalevaluett.X = (int)((ScaleLabelRadius) * Math.Cos(i_radian));
                majorscalevaluett.Y = (int)((ScaleLabelRadius) * Math.Sin(i_radian));

                //Defining the properties of the scale value textbox
                TextBlock tb = new TextBlock();

                tb.Height = 40;
                tb.Width = 2;
                tb.FontSize = 12;
                tb.Foreground = new SolidColorBrush(Colors.LightGray);
                tb.TextAlignment = TextAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.HorizontalAlignment = HorizontalAlignment.Center;

                //Writing and appending the scale value

                //checking minvalue < maxvalue w.r.t scale precion value
                if (Math.Round(minvalue, ScaleValuePrecision) <= Math.Round(MaxValue, ScaleValuePrecision))
                {
                    minvalue = Math.Round(minvalue, ScaleValuePrecision);
                    tb.Text = minvalue.ToString();
                    minvalue = minvalue + majorTicksUnitValue;

                }
                else
                {
                    break;
                }
                majortickgp.Children.Add(majorticktt);
                majortickrect.RenderTransform = majortickgp;
                tb.RenderTransform = majorscalevaluett;
                LayoutRoot.Children.Add(majortickrect);
                LayoutRoot.Children.Add(tb);


                //Drawing the minor axis ticks
                Double onedegree = ((i + majorTickUnitAngle) - i) / (MinorDivisionsCount);

                if ((i < (fromAngle + ScaleSweepAngle)) && (Math.Round(minvalue, ScaleValuePrecision) <= Math.Round(MaxValue, ScaleValuePrecision)))
                {
                    //Drawing the minor scale
                    for (Double mi = i + onedegree; mi < (i + majorTickUnitAngle); mi = mi + onedegree)
                    {
                        //here the minortick is drawn as a rectangle 
                        Rectangle mr = new Rectangle();
                        mr.Height = 3;
                        mr.Width = 1;
                        mr.Fill = new SolidColorBrush(Colors.LightGray);
                        mr.HorizontalAlignment = HorizontalAlignment.Center;
                        mr.VerticalAlignment = VerticalAlignment.Center;
                        Point p1 = new Point(0.5, 0.5);
                        mr.RenderTransformOrigin = p1;

                        TransformGroup minortickgp = new TransformGroup();
                        RotateTransform minortickrt = new RotateTransform();
                        minortickrt.Angle = mi;
                        minortickgp.Children.Add(minortickrt);
                        TranslateTransform minorticktt = new TranslateTransform();

                        //Obtaining the angle in radians for calulating the points
                        Double mi_radian = (mi * Math.PI) / 180;
                        //Finding the point on the Scale where the minor ticks are drawn
                        minorticktt.X = (int)((ScaleRadius) * Math.Cos(mi_radian));
                        minorticktt.Y = (int)((ScaleRadius) * Math.Sin(mi_radian));

                        minortickgp.Children.Add(minorticktt);
                        mr.RenderTransform = minortickgp;
                        LayoutRoot.Children.Add(mr);


                    }

                }

            }
        }
        /// <summary>
        /// Draw the range indicator
        /// </summary>
        private void DrawRangeIndicator()
        {
            Double realworldunit = (ScaleSweepAngle / (MaxValue - MinValue));
            Double optimalStartAngle;
            Double optimalEndAngle;
            double db;

            //Checking whether the  OptimalRangeStartvalue is -ve 
            if (OptimalRangeStartValue < 0)
            {
                db = MinValue + Math.Abs(OptimalRangeStartValue);
                optimalStartAngle = ((double)(Math.Abs(db * realworldunit)));
            }
            else
            {
                db = Math.Abs(MinValue) + OptimalRangeStartValue;
                optimalStartAngle = ((double)(db * realworldunit));
            }

            //Checking whether the  OptimalRangeEndvalue is -ve
            if (OptimalRangeEndValue < 0)
            {
                db = MinValue + Math.Abs(OptimalRangeEndValue);
                optimalEndAngle = ((double)(Math.Abs(db * realworldunit)));
            }
            else
            {
                db = Math.Abs(MinValue) + OptimalRangeEndValue;
                optimalEndAngle = ((double)(db * realworldunit));
            }
            // calculating the angle for optimal Start value

            Double optimalStartAngleFromStart = (fromAngle + optimalStartAngle);

            // calculating the angle for optimal Start value

            Double optimalEndAngleFromStart = (fromAngle + optimalEndAngle);

            //Calculating the Radius of the two arc for segment 
            arcradius1 = (RangeIndicatorRadius + RangeIndicatorThickness);
            arcradius2 = RangeIndicatorRadius;

            double endAngle = fromAngle + ScaleSweepAngle;

            // Calculating the Points for the below Optimal Range segment from the center of the gauge

            Point A = GetCircumferencePoint(fromAngle, arcradius1);
            Point B = GetCircumferencePoint(fromAngle, arcradius2);
            Point C = GetCircumferencePoint(optimalStartAngleFromStart, arcradius2);
            Point D = GetCircumferencePoint(optimalStartAngleFromStart, arcradius1);

            bool isReflexAngle = Math.Abs(optimalStartAngleFromStart - fromAngle) > 180.0;
            DrawSegment(A, B, C, D, isReflexAngle, BelowOptimalRangeColor);

            // Calculating the Points for the Optimal Range segment from the center of the gauge

            Point A1 = GetCircumferencePoint(optimalStartAngleFromStart, arcradius1);
            Point B1 = GetCircumferencePoint(optimalStartAngleFromStart, arcradius2);
            Point C1 = GetCircumferencePoint(optimalEndAngleFromStart, arcradius2);
            Point D1 = GetCircumferencePoint(optimalEndAngleFromStart, arcradius1);
            bool isReflexAngle1 = Math.Abs(optimalEndAngleFromStart - optimalStartAngleFromStart) > 180.0;
            DrawSegment(A1, B1, C1, D1, isReflexAngle1, OptimalRangeColor);

            // Calculating the Points for the Above Optimal Range segment from the center of the gauge

            Point A2 = GetCircumferencePoint(optimalEndAngleFromStart, arcradius1);
            Point B2 = GetCircumferencePoint(optimalEndAngleFromStart, arcradius2);
            Point C2 = GetCircumferencePoint(endAngle, arcradius2);
            Point D2 = GetCircumferencePoint(endAngle, arcradius1);
            bool isReflexAngle2 = Math.Abs(endAngle - optimalEndAngleFromStart) > 180.0;
            DrawSegment(A2, B2, C2, D2, isReflexAngle2, AboveOptimalRangeColor);
        }


        //Drawing the segment with two arc and two line
        private Path rangeIndicator;
        private void DrawSegment(Point p1, Point p2, Point p3, Point p4, bool reflexangle, Color clr)
        {

            // Segment Geometry
            PathSegmentCollection segments = new PathSegmentCollection();

            // First line segment from pt p1 - pt p2
            segments.Add(new LineSegment() { Point = p2 });

            //Arc drawn from pt p2 - pt p3 with the RangeIndicatorRadius 
            segments.Add(new ArcSegment()
            {
                Size = new Size(arcradius2, arcradius2),
                Point = p3,
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = reflexangle

            });

            // Second line segment from pt p3 - pt p4
            segments.Add(new LineSegment() { Point = p4 });

            //Arc drawn from pt p4 - pt p1 with the Radius of arcradius1 
            segments.Add(new ArcSegment()
            {
                Size = new Size(arcradius1, arcradius1),
                Point = p1,
                SweepDirection = SweepDirection.Counterclockwise,
                IsLargeArc = reflexangle

            });

            // Defining the segment path properties
            Color rangestrokecolor;
            if (clr == Colors.Transparent)
            {
                rangestrokecolor = clr;
            }
            else
                rangestrokecolor = Colors.White;



            rangeIndicator = new Path()
            {
                StrokeLineJoin = PenLineJoin.Round,
                Stroke = new SolidColorBrush(rangestrokecolor),
                //Color.FromArgb(0xFF, 0xF5, 0x9A, 0x86)
                Fill = new SolidColorBrush(clr),
                Opacity = 0.65,
                StrokeThickness = 0.25,
                Data = new PathGeometry()
                {
                    Figures = new PathFigureCollection()
                     {
                        new PathFigure()
                        {
                            IsClosed = true,
                            StartPoint = p1,
                            Segments = segments
                        }
                    }
                }
            };

            //Set Z index of range indicator
            rangeIndicator.SetValue(Canvas.ZIndexProperty, 150);
            // Adding the segment to the root grid 
            LayoutRoot.Children.Add(rangeIndicator);

        }

        /// <summary>
        /// Obtaining the Point (x,y) in the circumference 
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private Point GetCircumferencePoint(Double angle, Double radius)
        {
            Double angle_radian = (angle * Math.PI) / 180;
            //Radius-- is the Radius of the gauge
            Double X = (Double)((Radius) + (radius) * Math.Cos(angle_radian));
            Double Y = (Double)((Radius) + (radius) * Math.Sin(angle_radian));
            Point p = new Point(X, Y);
            return p;
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
