using System;


namespace Wing.Mvc.Controls
{
    /// <summary>
    /// Base class for user interface controls. 
    /// </summary>
    /// <remarks>
    /// The <see cref="HtmlControl"/> class enables to create user interface controls wich renders HTML as output.
    /// The <see cref="HtmlControl"/> class has a set of tools that allows creation of customizable and extensible controls, 
    /// with a binding system, and properties that reflect HTML attributes and CSS properties, and another features
    /// like property renderization redirection (search for Appliers on Help).
    /// To learn more about how to develop using the HtmlControl class, see Help.
    /// </remarks>
    public abstract class HtmlControl : HtmlObject
    {
        /// <summary>
        /// Padding. same as margin css property
        /// </summary>
        public static readonly ControlProperty PaddingProperty = ControlProperty.Register("Padding",
            typeof(Thickness),
            typeof(HtmlControl),
            new ThicknessCssPropertyApplier(CssProperty.Padding),
            null,
            PaddingPropertyChanged);

        public static readonly ControlProperty PaddingUnitProperty = ControlProperty.Register("PaddingUnit",
            typeof(CssUnit),
            typeof(HtmlControl),
            CssUnit.Px,
            PaddingUnitaryPropertyChanged);

        public static readonly ControlProperty PaddingLeftProperty = ControlProperty.Register("PaddingLeft",
            typeof(int?),
            typeof(HtmlControl),
            null,
            PaddingUnitaryPropertyChanged);

        public static readonly ControlProperty PaddingTopProperty = ControlProperty.Register("PaddingTop",
            typeof(int?),
            typeof(HtmlControl),
            null,
            PaddingUnitaryPropertyChanged);

        public static readonly ControlProperty PaddingRightProperty = ControlProperty.Register("PaddingRight",
            typeof(int?),
            typeof(HtmlControl),
            null,
            PaddingUnitaryPropertyChanged);

        public static readonly ControlProperty PaddingBottomProperty = ControlProperty.Register("PaddingBottom",
            typeof(int?),
            typeof(HtmlControl),
            null,
            PaddingUnitaryPropertyChanged);


        public static readonly ControlProperty MarginProperty = ControlProperty.Register("Margin",
            typeof(Thickness),
            typeof(HtmlControl),
            new ThicknessCssPropertyApplier(CssProperty.Margin),
            null,
            MarginPropertyChanged);

        public static readonly ControlProperty MarginUnitProperty = ControlProperty.Register("MarginUnit",
            typeof(CssUnit),
            typeof(HtmlControl),
            CssUnit.Px,
            MarginUnitaryPropertyChanged);

        public static readonly ControlProperty MarginLeftProperty = ControlProperty.Register("MarginLeft",
            typeof(int?),
            typeof(HtmlControl),
            null,
            MarginUnitaryPropertyChanged);

        public static readonly ControlProperty MarginTopProperty = ControlProperty.Register("MarginTop",
            typeof(int?),
            typeof(HtmlControl),
            null,
            MarginUnitaryPropertyChanged);

        public static readonly ControlProperty MarginRightProperty = ControlProperty.Register("MarginRight",
            typeof(int?),
            typeof(HtmlControl),
            null,
            MarginUnitaryPropertyChanged);

        public static readonly ControlProperty MarginBottomProperty = ControlProperty.Register("MarginBottom",
            typeof(int?),
            typeof(HtmlControl),
            null,
            MarginUnitaryPropertyChanged);

        public static readonly ControlProperty TextAlignProperty = ControlProperty.Register("TextAlign",
            typeof(TextAlignment),
            typeof(HtmlControl),
            new CssEnumPropertyApplier(CssProperty.TextAlign, TextAlignment.Default),
            TextAlignment.Default);

        public static readonly ControlProperty WhiteSpaceBehaviorProperty = ControlProperty.Register("WhiteSpaceBehavior",
            typeof(CssWhiteSpace),
            typeof(HtmlControl),
            new CssEnumPropertyApplier(CssProperty.WhiteSpace, CssWhiteSpace.Normal),
            CssWhiteSpace.Normal);

        public static readonly ControlProperty VisibilityProperty = ControlProperty.Register("Visibility",
            typeof(Visibility),
            typeof(HtmlControl),
            new VisibilityAttributeApplier(),
            Visibility.Visible);

        public static readonly ControlProperty FontFamilyProperty = ControlProperty.Register("FontFamily",
            typeof(String),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.FontFamily));

        public static readonly ControlProperty FontSizeProperty = ControlProperty.Register("FontSize",
            typeof(String),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.FontSize));

        public static readonly ControlProperty FontWeightProperty = ControlProperty.Register("FontWeight",
            typeof(CssFontWeight),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.FontWeight, CssFontWeight.Normal), CssFontWeight.Normal);

        public static readonly ControlProperty FontStyleProperty = ControlProperty.Register("FontStyle",
            typeof(CssFontStyle),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.FontStyle, CssFontStyle.Normal), CssFontStyle.Normal);

        public static readonly ControlProperty PositionProperty = ControlProperty.Register("Position",
            typeof(CssPosition),
            typeof(HtmlControl),
            new CssEnumPropertyApplier(CssProperty.Position, CssPosition.Static), CssPosition.Static);

        public static readonly ControlProperty LeftProperty = ControlProperty.Register("Left",
            typeof(String),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.Left));

        public static readonly ControlProperty RightProperty = ControlProperty.Register("Right",
            typeof(String),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.Right));

        public static readonly ControlProperty TopProperty = ControlProperty.Register("Top",
            typeof(String),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.Top));

        public static readonly ControlProperty BottomProperty = ControlProperty.Register("Bottom",
            typeof(String),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.Bottom));

        public static readonly ControlProperty WidthProperty = ControlProperty.Register("Width",
            typeof(String),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.Width));

        public static readonly ControlProperty HeightProperty = ControlProperty.Register("Height",
            typeof(String),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.Height));

        public static readonly ControlProperty ZIndexProperty = ControlProperty.Register("ZIndex",
            typeof(int),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.ZIndex, 0), 0);

        public static readonly ControlProperty FloatProperty = ControlProperty.Register("Float",
            typeof(CssFloat),
            typeof(HtmlControl),
            new CssEnumPropertyApplier(CssProperty.Float, CssFloat.None), CssFloat.None);

        public static readonly ControlProperty FloatClearProperty = ControlProperty.Register("FloatClear",
            typeof(CssClear),
            typeof(HtmlControl),
            new CssEnumPropertyApplier(CssProperty.Clear, CssClear.None), CssClear.None);

        public static readonly ControlProperty DisplayProperty = ControlProperty.Register("Display",
            typeof(CssDisplay),
            typeof(HtmlControl),
            new CssEnumPropertyApplier(CssProperty.Display, CssDisplay.Default), CssDisplay.Default);

        public static readonly ControlProperty HintProperty = ControlProperty.Register("Hint",
            typeof(String),
            typeof(HtmlControl),
            new HtmlAttributePropertyApplier(HtmlAttr.Title));

        public static readonly ControlProperty BackgroundColorProperty = ControlProperty.Register("BackgroundColor",
            typeof(String),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.BackgroundColor));

        public static readonly ControlProperty BackgroundImageProperty = ControlProperty.Register("BackgroundImage",
            typeof(String),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.BackgroundImage));

        public static readonly ControlProperty ForegroundColorProperty = ControlProperty.Register("ForegroundColor",
            typeof(String),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.Color));

        public static readonly ControlProperty BorderStyleProperty = ControlProperty.Register("BorderStyle",
            typeof(CssBorderStyle),
            typeof(HtmlControl),
            new CssEnumPropertyApplier(CssProperty.BorderStyle, CssBorderStyle.None));

        public static readonly ControlProperty BorderColorProperty = ControlProperty.Register("BorderColor",
            typeof(String),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.BorderColor));

        public static readonly ControlProperty BorderWidthProperty = ControlProperty.Register("BorderWidth",
            typeof(String),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.BorderWidth));

        public static readonly ControlProperty StyleKey = ControlProperty.Register("StyleKey",
            typeof(String),
            typeof(HtmlControl));

        public static readonly ControlProperty MaxHeightProperty = ControlProperty.Register("MaxHeight",
            typeof(String),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.MaxHeight));

        public static readonly ControlProperty MaxWidthProperty = ControlProperty.Register("MaxWidth",
            typeof(String),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.MaxWidth));

        public static readonly ControlProperty MinHeightProperty = ControlProperty.Register("MinHeight",
            typeof(String),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.MinHeight));

        public static readonly ControlProperty MinWidthProperty = ControlProperty.Register("MinWidth",
            typeof(String),
            typeof(HtmlControl),
            new CssPropertyApplier(CssProperty.MinWidth));

        public static readonly ControlProperty TextDecorationProperty = ControlProperty.Register("TextDecoration",
            typeof(CssTextDecoration),
            typeof(HtmlControl),
            new CssEnumPropertyApplier(CssProperty.TextDecoration, CssTextDecoration.None), CssTextDecoration.None);

        public static readonly ControlProperty CursorProperty = ControlProperty.Register("Cursor",
            typeof(CssCursor),
            typeof(HtmlControl),
            new CssEnumPropertyApplier(CssProperty.Cursor, CssCursor.Auto), CssCursor.Auto);

        public HtmlControl(HtmlTag tag) : base(tag) { }


        public Thickness Padding
        {
            get { return GetValue<Thickness>(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public Thickness Margin
        {
            get { return GetValue<Thickness>(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        public TextAlignment TextAlign
        {
            get { return GetValue<TextAlignment>(TextAlignProperty); }
            set { SetValue(TextAlignProperty, value); }
        }

        public CssWhiteSpace WhiteSpaceBehavior
        {
            get { return GetValue<CssWhiteSpace>(WhiteSpaceBehaviorProperty); }
            set { SetValue(WhiteSpaceBehaviorProperty, value); }
        }

        public Visibility Visibility
        {
            get { return GetValue<Visibility>(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        public String FontFamily
        {
            get { return GetValue<String>(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public String FontSize
        {
            get { return GetValue<String>(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public CssFontWeight FontWeight
        {
            get { return GetValue<CssFontWeight>(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        public CssFontStyle FontStyle
        {
            get { return GetValue<CssFontStyle>(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        public CssPosition Position
        {
            get { return GetValue<CssPosition>(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public String Left
        {
            get { return GetValue<String>(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        public String Right
        {
            get { return GetValue<String>(RightProperty); }
            set { SetValue(RightProperty, value); }
        }

        public String Top
        {
            get { return GetValue<String>(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        public String Bottom
        {
            get { return GetValue<String>(BottomProperty); }
            set { SetValue(BottomProperty, value); }
        }

        public String Width
        {
            get { return GetValue<String>(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public String Height
        {
            get { return GetValue<String>(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public int ZIndex
        {
            get { return GetValue<int>(ZIndexProperty); }
            set { SetValue(ZIndexProperty, value); }
        }

        public CssFloat Float
        {
            get { return GetValue<CssFloat>(FloatProperty); }
            set { SetValue(FloatProperty, value); }
        }

        public CssClear FloatClear
        {
            get { return GetValue<CssClear>(FloatClearProperty); }
            set { SetValue(FloatClearProperty, value); }
        }

        public CssDisplay Display
        {
            get { return GetValue<CssDisplay>(DisplayProperty); }
            set { SetValue(DisplayProperty, value); }
        }

        public String Hint
        {
            get { return GetValue<String>(HintProperty); }
            set { SetValue(HintProperty, value); }
        }

        public String ForegroundColor
        {
            get { return GetValue<String>(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

        public String BorderWidth
        {
            get { return GetValue<String>(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
        }

        public String BackgroundColor
        {
            get { return GetValue<String>(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public CssBorderStyle BorderStyle
        {
            get { return GetValue<CssBorderStyle>(BorderStyleProperty); }
            set { SetValue(BorderStyleProperty, value); }
        }

        public String BorderColor
        {
            get { return GetValue<String>(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public String BackgroundImage
        {
            get { return GetValue<String>(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }

        public CssUnit PaddingUnit
        {
            get { return GetValue<CssUnit>(PaddingUnitProperty); }
            set { SetValue(PaddingUnitProperty, value); }
        }

        public int? PaddingLeft
        {
            get { return GetValue<int?>(PaddingLeftProperty); }
            set { SetValue(PaddingLeftProperty, value); }
        }

        public int? PaddingRight
        {
            get { return GetValue<int?>(PaddingRightProperty); }
            set { SetValue(PaddingRightProperty, value); }
        }

        public int? PaddingTop
        {
            get { return GetValue<int?>(PaddingTopProperty); }
            set { SetValue(PaddingTopProperty, value); }
        }

        public int? PaddingBottom
        {
            get { return GetValue<int?>(PaddingBottomProperty); }
            set { SetValue(PaddingBottomProperty, value); }
        }

        public int? MarginLeft
        {
            get { return GetValue<int?>(MarginLeftProperty); }
            set { SetValue(MarginLeftProperty, value); }
        }

        public int? MarginRight
        {
            get { return GetValue<int?>(MarginRightProperty); }
            set { SetValue(MarginRightProperty, value); }
        }

        public int? MarginTop
        {
            get { return GetValue<int?>(MarginTopProperty); }
            set { SetValue(MarginTopProperty, value); }
        }

        public int? MarginBottom
        {
            get { return GetValue<int?>(MarginBottomProperty); }
            set { SetValue(MarginBottomProperty, value); }
        }

        public CssUnit MarginUnit
        {
            get { return GetValue<CssUnit>(MarginUnitProperty); }
            set { SetValue(MarginUnitProperty, value); }
        }

        private bool _settingPaddingProperty;
        private bool _settingUnitaryPaddingProperty;

        private static void PaddingPropertyChanged(ControlPropertyChangedEventArgs args)
        {
            var target = args.Target as HtmlControl;
            if (target != null && !target._settingUnitaryPaddingProperty)
            {
                target._settingPaddingProperty = true;
                var padding = args.NewValue as Thickness;
                target.PaddingLeft = padding.Left;
                target.PaddingRight = padding.Right;
                target.PaddingTop = padding.Top;
                target.PaddingBottom = padding.Bottom;
                target.PaddingUnit = padding.Unit;
                target._settingUnitaryPaddingProperty = false;
            }
        }

        private static void PaddingUnitaryPropertyChanged(ControlPropertyChangedEventArgs args)
        {
            var target = args.Target as HtmlControl;
            if (target != null && !target._settingPaddingProperty)
            {
                target._settingUnitaryPaddingProperty = true;
                var padding = target.Padding;
                if (padding == null)
                {
                    padding = new Thickness();
                    target.Padding = padding;
                }
                padding.Left = target.PaddingLeft;
                padding.Right = target.PaddingRight;
                padding.Top = target.PaddingTop;
                padding.Bottom = target.PaddingBottom;
                padding.Unit = target.PaddingUnit;
                target._settingUnitaryPaddingProperty = false;
            }
        }

        private bool _settingMarginProperty;
        private bool _settingUnitaryMarginProperty;

        private static void MarginPropertyChanged(ControlPropertyChangedEventArgs args)
        {
            var target = args.Target as HtmlControl;
            if (target != null && !target._settingUnitaryMarginProperty)
            {
                target._settingMarginProperty = true;
                var margin = args.NewValue as Thickness;
                target.MarginLeft = margin.Left;
                target.MarginRight = margin.Right;
                target.MarginTop = margin.Top;
                target.MarginBottom = margin.Bottom;
                target.MarginUnit = margin.Unit;
                target._settingUnitaryMarginProperty = false;
            }
        }

        private static void MarginUnitaryPropertyChanged(ControlPropertyChangedEventArgs args)
        {
            var target = args.Target as HtmlControl;
            if (target != null && !target._settingMarginProperty)
            {
                target._settingUnitaryMarginProperty = true;
                var margin = target.Margin;
                if (margin == null)
                {
                    margin = new Thickness();
                    target.Margin = margin;
                }
                margin.Left = target.MarginLeft;
                margin.Right = target.MarginRight;
                margin.Top = target.MarginTop;
                margin.Bottom = target.MarginBottom;
                margin.Unit = target.MarginUnit;
                target._settingUnitaryMarginProperty = false;
            }
        }

        public String MaxHeight
        {
            get { return GetValue<String>(MaxHeightProperty); }
            set { SetValue(MaxHeightProperty, value); }
        }

        public String MaxWidth
        {
            get { return GetValue<String>(MaxWidthProperty); }
            set { SetValue(MaxWidthProperty, value); }
        }

        public String MinHeight
        {
            get { return GetValue<String>(MinHeightProperty); }
            set { SetValue(MinHeightProperty, value); }
        }

        public String MinWidth
        {
            get { return GetValue<String>(MinWidthProperty); }
            set { SetValue(MinWidthProperty, value); }
        }

        public CssTextDecoration TextDecoration
        {
            get { return GetValue<CssTextDecoration>(TextDecorationProperty); }
            set { SetValue(TextDecorationProperty, value); }
        }

        public CssCursor Cursor
        {
            get { return GetValue<CssCursor>(CursorProperty); }
            set { SetValue(CursorProperty, value); }
        }
    }
}
