namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Telerik.Windows;

    [DefaultProperty("SelectedIndex"), DefaultEvent("SelectedIndexChanged")]
    public class RadCube : Telerik.Windows.Controls.ItemsControl
    {
        private int count;
        private Collection<Face> faceArray;
        private FaceOrientation faceOrientation;
        private bool fading = true;
        private bool hasLayout;
        private bool isKeyPressed;
        public static readonly DependencyProperty IsRotateOnClickEnabledProperty = DependencyProperty.Register("IsRotateOnClickEnabled", typeof(bool), typeof(RadCube), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadCube.OnIsRotateOnClickEnabledPropertyChanged)));
        public static readonly DependencyProperty IsRotateOnStartProperty = DependencyProperty.Register("IsRotateOnStart", typeof(bool), typeof(RadCube), null);
        public static readonly DependencyProperty IsRotateXAxisOnlyProperty = DependencyProperty.Register("IsRotateXAxisOnly", typeof(bool), typeof(RadCube), null);
        public static readonly DependencyProperty IsRotateYAxisOnlyProperty = DependencyProperty.Register("IsRotateYAxisOnly", typeof(bool), typeof(RadCube), null);
        private double mouseX = 100.0;
        private double mouseY = 100.0;
        private Collection<Point3D> pointArray;
        private FrameworkElement rootElement;
        public static readonly DependencyProperty RotateSpeedProperty = DependencyProperty.Register("RotateSpeed", typeof(double), typeof(RadCube), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadCube.OnRotateSpeedPropertyChanged)));
        public static readonly DependencyProperty SelectedIndexProperty = DependencyPropertyExtensions.Register("SelectedIndex", typeof(int), typeof(RadCube), new Telerik.Windows.PropertyMetadata(0, new PropertyChangedCallback(RadCube.OnSelectedIndexPropertyChanged), new CoerceValueCallback(RadCube.SelectedIndexChangedConstrainValue)));
        public static readonly DependencyProperty Side1BackgroundProperty = DependencyProperty.Register("Side1Background", typeof(Brush), typeof(RadCube), null);
        public static readonly DependencyProperty Side2BackgroundProperty = DependencyProperty.Register("Side2Background", typeof(Brush), typeof(RadCube), null);
        public static readonly DependencyProperty Side3BackgroundProperty = DependencyProperty.Register("Side3Background", typeof(Brush), typeof(RadCube), null);
        public static readonly DependencyProperty Side4BackgroundProperty = DependencyProperty.Register("Side4Background", typeof(Brush), typeof(RadCube), null);
        public static readonly DependencyProperty Side5BackgroundProperty = DependencyProperty.Register("Side5Background", typeof(Brush), typeof(RadCube), null);
        public static readonly DependencyProperty Side6BackgroundProperty = DependencyProperty.Register("Side6Background", typeof(Brush), typeof(RadCube), null);
        private DispatcherTimer timer;
        public static readonly DependencyProperty XWidthProperty = DependencyProperty.Register("XWidth", typeof(double), typeof(RadCube), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadCube.OnXWidthPropertyChanged)));
        public static readonly DependencyProperty YWidthProperty = DependencyProperty.Register("YWidth", typeof(double), typeof(RadCube), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadCube.OnYWidthPropertyChanged)));
        public static readonly DependencyProperty ZWidthProperty = DependencyProperty.Register("ZWidth", typeof(double), typeof(RadCube), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadCube.OnZWidthPropertyChanged)));

        public event EventHandler SelectedIndexChanged;

        public RadCube()
        {
            
            base.DefaultStyleKey = typeof(RadCube);
            base.MouseMove += new MouseEventHandler(this.RadCube_MouseMove);
        }

        internal void AttachFaceAction()
        {
            if (this.IsRotateOnClickEnabled)
            {
                this.DetachFaceAction();
                for (int i = 0; i < 6; i++)
                {
                    Panel face = base.GetTemplateChild("side" + (i + 1)) as Panel;
                    if (face != null)
                    {
                        face.MouseLeftButtonDown += new MouseButtonEventHandler(this.Face_MouseLeftButtonDown);
                    }
                }
            }
        }

        internal void DetachFaceAction()
        {
            for (int i = 0; i < 6; i++)
            {
                Panel face = base.GetTemplateChild("side" + (i + 1)) as Panel;
                if (face != null)
                {
                    face.MouseLeftButtonDown -= new MouseButtonEventHandler(this.Face_MouseLeftButtonDown);
                }
            }
        }

        private void Face_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                Panel face = sender as Panel;
                int index = int.Parse(face.Name.Substring(4), CultureInfo.InvariantCulture.NumberFormat) - 1;
                if (!this.fading)
                {
                    this.SelectedIndex = index;
                }
                else
                {
                    this.fading = false;
                    this.SetEnterFrame(false);
                    this.SetMasksVisibility(Visibility.Visible);
                }
            }
        }

        private void FaceOnAnimation()
        {
            FaceOrientation fo = this.faceOrientation;
            this.count++;
            this.RotateCube(fo.Axis, fo.FaceRotate / 10.0);
            this.RotateCube(GetFaceCenter(fo.Reg), fo.CornerRotate / 10.0);
            if (this.count >= 10)
            {
                this.OnSelectedIndexChanged(EventArgs.Empty);
                if (this.timer != null)
                {
                    this.timer.Stop();
                }
            }
        }

        public Panel GetCubeFacePanel(int faceIndex)
        {
            if (faceIndex < 0)
            {
                faceIndex = 0;
            }
            if (faceIndex > 5)
            {
                faceIndex = 5;
            }
            return (base.GetTemplateChild("side" + (faceIndex + 1) + "Content") as Panel);
        }

        private static Point3D GetFaceCenter(Face reg)
        {
            return ThreeD.GetMiddlePoint(reg.TR, reg.BL);
        }

        private FaceOrientation GetFaceOrientation(Face reg)
        {
            Point3D endFaceCenter = new Point3D(0.0, 0.0, -(this.ZWidth / 2.0));
            Point3D currFaceCenter = GetFaceCenter(reg);
            Point3D axis = ThreeD.GetRotateAxis(currFaceCenter, endFaceCenter);
            double faceRotate = ThreeD.GetRotateAngle(axis, currFaceCenter, endFaceCenter);
            Point3D p = reg.TL;
            Point3D currCorner = new Point3D(p.XPosition, p.YPosition, p.ZPosition);
            currCorner = ThreeD.Rotate3D(currCorner, axis, faceRotate);
            Point3D endCorner = new Point3D(-(this.XWidth / 2.0), -(this.YWidth / 2.0), -(this.ZWidth / 2.0));
            Point3D v1 = ThreeD.Vector(endFaceCenter, currCorner);
            Point3D v2 = ThreeD.Vector(endFaceCenter, endCorner);
            return new FaceOrientation(reg, axis, faceRotate, ThreeD.GetRotateAngle(endFaceCenter, v1, v2));
        }

        private void InitCube()
        {
            double xw = this.XWidth / 2.0;
            double yw = this.YWidth / 2.0;
            double zw = this.ZWidth / 2.0;
            Point3D p0 = new Point3D(-xw, -yw, -zw);
            Point3D p1 = new Point3D(xw, -yw, -zw);
            Point3D p2 = new Point3D(xw, -yw, zw);
            Point3D p3 = new Point3D(-xw, -yw, zw);
            Point3D p4 = new Point3D(-xw, yw, -zw);
            Point3D p5 = new Point3D(xw, yw, -zw);
            Point3D p6 = new Point3D(xw, yw, zw);
            Point3D p7 = new Point3D(-xw, yw, zw);
            this.pointArray = new Collection<Point3D>();
            this.pointArray.Add(p0);
            this.pointArray.Add(p1);
            this.pointArray.Add(p2);
            this.pointArray.Add(p3);
            this.pointArray.Add(p4);
            this.pointArray.Add(p5);
            this.pointArray.Add(p6);
            this.pointArray.Add(p7);
            this.faceArray = new Collection<Face>();
            this.faceArray.Add(new Face(p3, p2, p0));
            this.faceArray.Add(new Face(p4, p5, p7));
            this.faceArray.Add(new Face(p0, p1, p4));
            this.faceArray.Add(new Face(p1, p2, p5));
            this.faceArray.Add(new Face(p2, p3, p6));
            this.faceArray.Add(new Face(p3, p0, p7));
        }

        private void InitScaling()
        {
            for (int i = 0; i < 6; i++)
            {
                ScaleTransform scaleTransform = base.GetTemplateChild(string.Format(CultureInfo.InvariantCulture, "side{0}ContentScaleTransform", new object[] { i + 1 })) as ScaleTransform;
                double scaleCoef = 100.0 / this.XWidth;
                if (!double.IsInfinity(scaleCoef))
                {
                    scaleTransform.SetValue(ScaleTransform.ScaleXProperty, scaleCoef);
                    scaleTransform.SetValue(ScaleTransform.ScaleYProperty, scaleCoef);
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.rootElement = base.GetTemplateChild("RootElement") as FrameworkElement;
            this.hasLayout = true;
            this.InitCube();
            this.InitScaling();
            this.AttachFaceAction();
            if (this.IsRotateOnStart)
            {
                this.fading = false;
                this.SetEnterFrame(false);
            }
            else
            {
                FaceOrientation fo = this.GetFaceOrientation(this.faceArray[0]);
                for (int i = 0; i < 10; i++)
                {
                    this.RotateCube(fo.Axis, fo.FaceRotate / 10.0);
                    this.RotateCube(GetFaceCenter(fo.Reg), fo.CornerRotate / 10.0);
                }
                this.SetMasksVisibility(Visibility.Collapsed);
            }
            this.PerformLayoutCore();
            if (this.SelectedIndex != 0)
            {
                this.SelectSide(this.SelectedIndex);
            }
        }

        private static void OnIsRotateOnClickEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            RadCube cube = d as RadCube;
            if (cube.IsRotateOnClickEnabled)
            {
                cube.AttachFaceAction();
            }
            else
            {
                cube.DetachFaceAction();
            }
        }

        private static void OnRotateSpeedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as RadCube).SetSpeed();
        }

        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            EventHandler handler = this.SelectedIndexChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private static void OnSelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            RadCube cube = d as RadCube;
            if (cube.hasLayout)
            {
                int faceIndex = (int) args.NewValue;
                cube.SelectSide(faceIndex);
            }
        }

        private void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            RadCube cube = d as RadCube;
            if (cube.hasLayout)
            {
                cube.InitCube();
                cube.InitScaling();
                cube.RenderFaces();
            }
        }

        private static void OnXWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as RadCube).OnWidthChanged(d, args);
        }

        private static void OnYWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as RadCube).OnWidthChanged(d, args);
        }

        private static void OnZWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as RadCube).OnWidthChanged(d, args);
        }

        private void PerformLayoutCore()
        {
            for (int i = 0; (i < base.Items.Count) && (i < 6); i++)
            {
                UIElement item = base.Items[i] as UIElement;
                (base.GetTemplateChild("side" + (i + 1) + "Content") as Panel).Children.Add(item);
            }
        }

        private void RadCube_MouseMove(object sender, MouseEventArgs e)
        {
            this.mouseX = e.GetPosition(this.rootElement).X;
            this.mouseY = e.GetPosition(this.rootElement).Y;
        }

        private void RenderFaces()
        {
            for (int k = 0; k < 6; k++)
            {
                Panel face = base.GetTemplateChild("side" + (k + 1)) as Panel;
                Panel faceHolder = base.GetTemplateChild("side" + (k + 1) + "Child") as Panel;
                Face reg = this.faceArray[k];
                Point3D faceDirection = GetFaceCenter(reg);
                double centerZ = faceDirection.ZPosition / ThreeD.GetLength(faceDirection);
                if (centerZ <= 0.0)
                {
                    face.Visibility = Visibility.Visible;
                    (base.GetTemplateChild("side" + (k + 1) + "Mask") as FrameworkElement).SetValue(UIElement.OpacityProperty, (1.0 + centerZ) - 0.2);
                    ThreeD.Skew(face, faceHolder, 100.0, 100.0, ShiftPoint(reg.TL), ShiftPoint(reg.BL), ShiftPoint(reg.TR));
                }
                else
                {
                    face.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void RotateByMouse()
        {
            double dx = this.mouseX;
            double dy = this.mouseY;
            double dz = 10.0;
            if (this.IsRotateXAxisOnly)
            {
                dy = 0.0;
                dz = 0.0;
            }
            if (this.IsRotateYAxisOnly)
            {
                dx = 0.0;
                dz = 0.0;
            }
            Point3D verticalAxis = new Point3D(-dy, dx, dz);
            double amp = Math.Sqrt((dx * dx) + (dy * dy));
            this.RotateCube(verticalAxis, amp / 1000.0);
        }

        private void RotateCube(Point3D verticalAxis, double angle)
        {
            this.pointArray = ThreeD.Rotate3DArray(this.pointArray, verticalAxis, angle);
            this.faceArray = new Collection<Face>();
            this.faceArray.Add(new Face(this.pointArray[3], this.pointArray[2], this.pointArray[0]));
            this.faceArray.Add(new Face(this.pointArray[4], this.pointArray[5], this.pointArray[7]));
            this.faceArray.Add(new Face(this.pointArray[0], this.pointArray[1], this.pointArray[4]));
            this.faceArray.Add(new Face(this.pointArray[1], this.pointArray[2], this.pointArray[5]));
            this.faceArray.Add(new Face(this.pointArray[2], this.pointArray[3], this.pointArray[6]));
            this.faceArray.Add(new Face(this.pointArray[3], this.pointArray[0], this.pointArray[7]));
            this.RenderFaces();
        }

        private static object SelectedIndexChangedConstrainValue(DependencyObject d, object newValue)
        {
            int faceIndex = (int) newValue;
            if (faceIndex < 0)
            {
                faceIndex = 0;
            }
            if (faceIndex > 5)
            {
                faceIndex = 5;
            }
            return faceIndex;
        }

        private void SelectSide(int faceIndex)
        {
            Face face = this.faceArray[faceIndex];
            if (face != null)
            {
                this.faceOrientation = this.GetFaceOrientation(face);
                this.count = 0;
                this.SetEnterFrame(true);
                this.fading = true;
                this.SetMasksVisibility(Visibility.Collapsed);
            }
        }

        private void SetEnterFrame(bool keyPressed)
        {
            this.isKeyPressed = keyPressed;
            if (this.timer == null)
            {
                this.timer = new DispatcherTimer();
                this.SetSpeed();
                this.timer.Tick += new EventHandler(this.Timer_Tick);
            }
            else
            {
                this.timer.Stop();
            }
            this.timer.Start();
        }

        private void SetMasksVisibility(Visibility visibility)
        {
            for (int k = 0; k < 6; k++)
            {
                FrameworkElement mask = base.GetTemplateChild("side" + (k + 1) + "Mask") as FrameworkElement;
                mask.Visibility = visibility;
            }
        }

        internal void SetSpeed()
        {
            if (this.timer != null)
            {
                this.timer.Interval = TimeSpan.FromMilliseconds(20.0 / this.RotateSpeed);
            }
        }

        private static Point3D ShiftPoint(Point3D point)
        {
            return new Point3D(point.XPosition, point.YPosition, point.ZPosition);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (this.isKeyPressed)
            {
                this.FaceOnAnimation();
            }
            else
            {
                this.RotateByMouse();
            }
        }

        public bool IsRotateOnClickEnabled
        {
            get
            {
                return (bool) base.GetValue(IsRotateOnClickEnabledProperty);
            }
            set
            {
                base.SetValue(IsRotateOnClickEnabledProperty, value);
            }
        }

        public bool IsRotateOnStart
        {
            get
            {
                return (bool) base.GetValue(IsRotateOnStartProperty);
            }
            set
            {
                base.SetValue(IsRotateOnStartProperty, value);
            }
        }

        public bool IsRotateXAxisOnly
        {
            get
            {
                return (bool) base.GetValue(IsRotateXAxisOnlyProperty);
            }
            set
            {
                base.SetValue(IsRotateXAxisOnlyProperty, value);
            }
        }

        public bool IsRotateYAxisOnly
        {
            get
            {
                return (bool) base.GetValue(IsRotateYAxisOnlyProperty);
            }
            set
            {
                base.SetValue(IsRotateYAxisOnlyProperty, value);
            }
        }

        public double RotateSpeed
        {
            get
            {
                return (double) base.GetValue(RotateSpeedProperty);
            }
            set
            {
                base.SetValue(RotateSpeedProperty, value);
            }
        }

        public int SelectedIndex
        {
            get
            {
                return (int) base.GetValue(SelectedIndexProperty);
            }
            set
            {
                base.SetValue(SelectedIndexProperty, value);
            }
        }

        public Brush Side1Background
        {
            get
            {
                return (Brush) base.GetValue(Side1BackgroundProperty);
            }
            set
            {
                base.SetValue(Side1BackgroundProperty, value);
            }
        }

        public Brush Side2Background
        {
            get
            {
                return (Brush) base.GetValue(Side2BackgroundProperty);
            }
            set
            {
                base.SetValue(Side2BackgroundProperty, value);
            }
        }

        public Brush Side3Background
        {
            get
            {
                return (Brush) base.GetValue(Side3BackgroundProperty);
            }
            set
            {
                base.SetValue(Side3BackgroundProperty, value);
            }
        }

        public Brush Side4Background
        {
            get
            {
                return (Brush) base.GetValue(Side4BackgroundProperty);
            }
            set
            {
                base.SetValue(Side4BackgroundProperty, value);
            }
        }

        public Brush Side5Background
        {
            get
            {
                return (Brush) base.GetValue(Side5BackgroundProperty);
            }
            set
            {
                base.SetValue(Side5BackgroundProperty, value);
            }
        }

        public Brush Side6Background
        {
            get
            {
                return (Brush) base.GetValue(Side6BackgroundProperty);
            }
            set
            {
                base.SetValue(Side6BackgroundProperty, value);
            }
        }

        public double XWidth
        {
            get
            {
                return (double) base.GetValue(XWidthProperty);
            }
            set
            {
                base.SetValue(XWidthProperty, value);
            }
        }

        public double YWidth
        {
            get
            {
                return (double) base.GetValue(YWidthProperty);
            }
            set
            {
                base.SetValue(YWidthProperty, value);
            }
        }

        public double ZWidth
        {
            get
            {
                return (double) base.GetValue(ZWidthProperty);
            }
            set
            {
                base.SetValue(ZWidthProperty, value);
            }
        }
    }
}

