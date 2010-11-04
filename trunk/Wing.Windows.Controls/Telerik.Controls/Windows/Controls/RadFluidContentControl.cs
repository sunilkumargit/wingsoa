namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Animation;

    [TemplateVisualState(Name="Small", GroupName="SizeStates"), TemplateVisualState(Name="Normal", GroupName="SizeStates"), TemplateVisualState(Name="Large", GroupName="SizeStates"), DefaultProperty("Content")]
    public class RadFluidContentControl : ContentControl
    {
        public static readonly DependencyProperty ContentChangeModeProperty = DependencyProperty.Register("ContentChangeMode", typeof(Telerik.Windows.Controls.ContentChangeMode), typeof(RadFluidContentControl), new System.Windows.PropertyMetadata(Telerik.Windows.Controls.ContentChangeMode.Automatic));
        private static Size defaultLargeToNormalThreshold = new Size(300.0, 300.0);
        private static Size defaultNormalToLargeThreshold = new Size(300.0, 300.0);
        private static Size defaultNormalToSmallThreshold = new Size(150.0, 150.0);
        private static Size defaultSmallToNormalThreshold = new Size(150.0, 150.0);
        public static readonly DependencyProperty LargeContentProperty = DependencyProperty.Register("LargeContent", typeof(object), typeof(RadFluidContentControl), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadFluidContentControl.OnLargeContentPropertyChanged)));
        public static readonly DependencyProperty LargeContentTemplateProperty = DependencyProperty.Register("LargeContentTemplate", typeof(DataTemplate), typeof(RadFluidContentControl), null);
        public static readonly DependencyProperty LargeToNormalThresholdProperty = DependencyProperty.Register("LargeToNormalThreshold", typeof(Size), typeof(RadFluidContentControl), new System.Windows.PropertyMetadata(Size.Empty));
        public static readonly DependencyProperty NormalToLargeThresholdProperty = DependencyProperty.Register("NormalToLargeThreshold", typeof(Size), typeof(RadFluidContentControl), new System.Windows.PropertyMetadata(Size.Empty));
        public static readonly DependencyProperty NormalToSmallThresholdProperty = DependencyProperty.Register("NormalToSmallThreshold", typeof(Size), typeof(RadFluidContentControl), new System.Windows.PropertyMetadata(Size.Empty));
        public static readonly DependencyProperty SmallContentProperty = DependencyProperty.Register("SmallContent", typeof(object), typeof(RadFluidContentControl), null);
        public static readonly DependencyProperty SmallContentTemplateProperty = DependencyProperty.Register("SmallContentTemplate", typeof(DataTemplate), typeof(RadFluidContentControl), null);
        public static readonly DependencyProperty SmallToNormalThresholdProperty = DependencyProperty.Register("SmallToNormalThreshold", typeof(Size), typeof(RadFluidContentControl), new System.Windows.PropertyMetadata(Size.Empty));
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(FluidContentControlState), typeof(RadFluidContentControl), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadFluidContentControl.OnStateChanged)));

        public event EventHandler<FluidContentControlStateChangedEventArgs> StateChanged;

        public RadFluidContentControl()
        {
            base.DefaultStyleKey = typeof(RadFluidContentControl);
        }

        private bool AreThresholdsValid()
        {
            return (((((this.NormalToSmallThreshold.Width - this.SmallToNormalThreshold.Width) <= double.Epsilon) && ((this.NormalToSmallThreshold.Height - this.SmallToNormalThreshold.Height) <= double.Epsilon)) && ((this.LargeToNormalThreshold.Width - this.NormalToLargeThreshold.Width) <= double.Epsilon)) && ((this.LargeToNormalThreshold.Height - this.NormalToLargeThreshold.Height) <= double.Epsilon));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.ContentChangeMode == Telerik.Windows.Controls.ContentChangeMode.Automatic)
            {
                bool stateNeedsChange = this.NumberOfSetContentProperties > 1;
                while (stateNeedsChange)
                {
                    switch (this.State)
                    {
                        case FluidContentControlState.Small:
                            stateNeedsChange = this.UpdateState(finalSize, finalSize, SmallToNormalThresholdProperty, FluidContentControlState.Normal);
                            break;

                        case FluidContentControlState.Normal:
                            stateNeedsChange = this.UpdateState(finalSize, finalSize, NormalToSmallThresholdProperty, FluidContentControlState.Small);
                            if (!stateNeedsChange)
                            {
                                stateNeedsChange = this.UpdateState(finalSize, finalSize, NormalToLargeThresholdProperty, FluidContentControlState.Large);
                            }
                            break;

                        case FluidContentControlState.Large:
                            stateNeedsChange = this.UpdateState(finalSize, finalSize, LargeToNormalThresholdProperty, FluidContentControlState.Normal);
                            break;
                    }
                }
            }
            return base.ArrangeOverride(finalSize);
        }

        internal bool ChangeState(FluidContentControlState newState)
        {
            if (this.IsStateTransitionPossible(newState))
            {
                this.State = newState;
                return true;
            }
            FluidContentControlState? alternativeState = GetAlternativeState(newState, this.IsNewStateLargerThanCurrent(newState));
            return (alternativeState.HasValue && this.ChangeState(alternativeState.Value));
        }

        private void EnsureProperThresholdValues()
        {
            if (!this.AreThresholdsValid() && !RadControl.IsInDesignMode)
            {
                throw new InvalidOperationException(string.Format("\r\nInvalid threshold values! Please ensure that threshold values are set in such a way that there are no gaps between them.\r\nOverlapping and equal thresholds, i.e. SmallToNormalThreshold='100,100' and NormalToSmallThreshold='80,80', are both valid.\r\n{0}Valid example:\r\n     SmallToNormalThreshold='100,100'\r\n     NormalToSmallthreshold='100,100'\r\n     NormalToLargeThreshold='200,200'\r\n     LargeToNormalThreshold='200,200'\r\n\r\nThe current threshold values are:\r\n     SmallToNormalThreshold='{1},{2}'\r\n     NormalToSmallthreshold='{3},{4}'\r\n     NormalToLargeThreshold='{5},{6}'\r\n     LargeToNormalThreshold='{7},{8}'\r\n\t\t\t\t", new object[] { Environment.NewLine, this.SmallToNormalThreshold.Width, this.SmallToNormalThreshold.Height, this.NormalToSmallThreshold.Width, this.NormalToSmallThreshold.Height, this.NormalToLargeThreshold.Width, this.NormalToLargeThreshold.Height, this.LargeToNormalThreshold.Width, this.LargeToNormalThreshold.Height }));
            }
        }

        internal static FluidContentControlState? GetAlternativeState(FluidContentControlState state, bool larger)
        {
            switch (state)
            {
                case FluidContentControlState.Small:
                    if (larger)
                    {
                        return FluidContentControlState.Normal;
                    }
                    return null;

                case FluidContentControlState.Normal:
                    if (larger)
                    {
                        return FluidContentControlState.Large;
                    }
                    return FluidContentControlState.Small;

                case FluidContentControlState.Large:
                    if (larger)
                    {
                        return null;
                    }
                    return FluidContentControlState.Normal;
            }
            return null;
        }

        internal static Size GetDefaultThreshold(DependencyProperty thresholdProperty)
        {
            if (thresholdProperty == SmallToNormalThresholdProperty)
            {
                return defaultSmallToNormalThreshold;
            }
            if (thresholdProperty == NormalToSmallThresholdProperty)
            {
                return defaultNormalToSmallThreshold;
            }
            if (thresholdProperty == NormalToLargeThresholdProperty)
            {
                return defaultNormalToLargeThreshold;
            }
            if (thresholdProperty == LargeToNormalThresholdProperty)
            {
                return defaultLargeToNormalThreshold;
            }
            return Size.Empty;
        }

        internal static Size GetSizeDifference(Size availableSize, Size desiredSize)
        {
            return new Size { Height = Math.Abs((double) (availableSize.Height - desiredSize.Height)), Width = Math.Abs((double) (availableSize.Width - desiredSize.Width)) };
        }

        internal Size GetThreshold(DependencyProperty thresholdProperty)
        {
            Size? thresholdValue = base.ReadLocalValue(thresholdProperty) as Size?;
            if (!thresholdValue.HasValue || (thresholdValue.HasValue && thresholdValue.Value.Equals(Size.Empty)))
            {
                return GetDefaultThreshold(thresholdProperty);
            }
            return thresholdValue.Value;
        }

        private bool GoToState(bool useTransitions, string stateName)
        {
            return VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        internal bool IsNewStateLargerThanCurrent(FluidContentControlState newState)
        {
            switch (this.State)
            {
                case FluidContentControlState.Small:
                    return true;

                case FluidContentControlState.Normal:
                    if (newState == FluidContentControlState.Small)
                    {
                        return false;
                    }
                    return true;

                case FluidContentControlState.Large:
                    return false;
            }
            return false;
        }

        internal static bool IsSpaceEnough(Size availableSize, Size desiredSize)
        {
            bool enoughAvailableHeight = (availableSize.Height - desiredSize.Height) > 0.0;
            bool enoughAvailableWidth = (availableSize.Width - desiredSize.Width) > 0.0;
            return (enoughAvailableHeight && enoughAvailableWidth);
        }

        internal bool IsStateTransitionPossible(FluidContentControlState newState)
        {
            if (this.ContentChangeMode == Telerik.Windows.Controls.ContentChangeMode.Manual)
            {
                return false;
            }
            bool result = true;
            switch (newState)
            {
                case FluidContentControlState.Small:
                    result = this.SmallContent != null;
                    break;

                case FluidContentControlState.Normal:
                    result = base.Content != null;
                    break;

                case FluidContentControlState.Large:
                    result = this.LargeContent != null;
                    break;

                default:
                    result = false;
                    break;
            }
            if (result && this.WillStateTransitionCauseCycle(newState))
            {
                result = false;
            }
            return result;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            bool stateNeedsChange = ((this.NumberOfSetContentProperties > 1) && !double.IsInfinity(availableSize.Height)) && !double.IsInfinity(availableSize.Width);
            while (stateNeedsChange)
            {
                Size desiredSize = base.MeasureOverride(availableSize);
                bool enoughAvailableSize = IsSpaceEnough(availableSize, desiredSize);
                switch (this.State)
                {
                    case FluidContentControlState.Small:
                    {
                        if (!enoughAvailableSize)
                        {
                            break;
                        }
                        stateNeedsChange = this.UpdateState(availableSize, desiredSize, SmallToNormalThresholdProperty, FluidContentControlState.Normal);
                        continue;
                    }
                    case FluidContentControlState.Normal:
                    {
                        if (!enoughAvailableSize)
                        {
                            goto Label_0088;
                        }
                        stateNeedsChange = this.UpdateState(availableSize, desiredSize, NormalToLargeThresholdProperty, FluidContentControlState.Large);
                        continue;
                    }
                    case FluidContentControlState.Large:
                    {
                        if (!enoughAvailableSize)
                        {
                            goto Label_00A0;
                        }
                        stateNeedsChange = false;
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
                stateNeedsChange = false;
                continue;
            Label_0088:
                stateNeedsChange = this.UpdateState(availableSize, desiredSize, NormalToSmallThresholdProperty, FluidContentControlState.Small);
                continue;
            Label_00A0:
                stateNeedsChange = this.UpdateState(availableSize, desiredSize, LargeToNormalThresholdProperty, FluidContentControlState.Normal);
            }
            return base.MeasureOverride(availableSize);
        }

        public override void OnApplyTemplate()
        {
            this.EnsureProperThresholdValues();
            base.OnApplyTemplate();
            this.ChangeState(this.State);
            this.UpdateVisualState(AnimationManager.IsGlobalAnimationEnabled);
        }

        private static void OnLargeContentPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadFluidContentControl sender = (RadFluidContentControl) d;
            if (sender != null)
            {
                if (sender.ContentChangeMode == Telerik.Windows.Controls.ContentChangeMode.Manual)
                {
                    sender.UpdateVisualState(AnimationManager.IsGlobalAnimationEnabled && AnimationManager.GetIsAnimationEnabled(sender));
                }
                if (sender.StateChanged != null)
                {
                    FluidContentControlState oldValue = (e.OldValue is FluidContentControlState) ? ((FluidContentControlState) e.OldValue) : FluidContentControlState.Normal;
                    FluidContentControlState newValue = (e.NewValue is FluidContentControlState) ? ((FluidContentControlState) e.NewValue) : FluidContentControlState.Normal;
                    sender.StateChanged(sender, new FluidContentControlStateChangedEventArgs(oldValue, newValue));
                }
            }
        }

        internal static bool ShouldChangeState(Size desiredSize, Size difference, Size threshold, bool isIncreasingThreshold)
        {
            if (isIncreasingThreshold)
            {
                bool heightThresholdPassed = (threshold.Height > 0.0) && ((desiredSize.Height + difference.Height) > threshold.Height);
                heightThresholdPassed = (threshold.Height > 0.0) ? heightThresholdPassed : true;
                bool widthThresholdPassed = (threshold.Width > 0.0) && ((desiredSize.Width + difference.Width) > threshold.Width);
                widthThresholdPassed = (threshold.Width > 0.0) ? widthThresholdPassed : true;
                return (heightThresholdPassed && widthThresholdPassed);
            }
            bool _heightThresholdPassed = (threshold.Height > 0.0) && ((desiredSize.Height - difference.Height) < threshold.Height);
            _heightThresholdPassed = (threshold.Height > 0.0) ? _heightThresholdPassed : true;
            bool _widthThresholdPassed = (threshold.Width > 0.0) && ((desiredSize.Width - difference.Width) < threshold.Width);
            _widthThresholdPassed = (threshold.Width > 0.0) ? _widthThresholdPassed : true;
            return (_heightThresholdPassed && _widthThresholdPassed);
        }

        internal bool UpdateState(Size availableSize, Size desiredSize, DependencyProperty property, FluidContentControlState newState)
        {
            bool isIncreasingThreshold = (property == SmallToNormalThresholdProperty) || (property == NormalToLargeThresholdProperty);
            Size threshold = this.GetThreshold(property);
            Size sizeDifference = GetSizeDifference(availableSize, desiredSize);
            if (ShouldChangeState(desiredSize, sizeDifference, threshold, isIncreasingThreshold) && this.ChangeState(newState))
            {
                this.UpdateVisualState(AnimationManager.IsGlobalAnimationEnabled && AnimationManager.GetIsAnimationEnabled(this));
                return true;
            }
            return false;
        }

        internal void UpdateVisualState(bool useTransitions)
        {
            switch (this.State)
            {
                case FluidContentControlState.Small:
                    this.GoToState(useTransitions, "Small");
                    return;

                case FluidContentControlState.Normal:
                    this.GoToState(useTransitions, "Normal");
                    return;

                case FluidContentControlState.Large:
                    this.GoToState(useTransitions, "Large");
                    return;
            }
        }

        private bool WillStateTransitionCauseCycle(FluidContentControlState newState)
        {
            bool result = false;
            Size normalToSmallThreshold = this.NormalToSmallThreshold.IsEmpty ? defaultNormalToSmallThreshold : this.NormalToSmallThreshold;
            Size normalToLargeThreshold = this.NormalToLargeThreshold.IsEmpty ? defaultNormalToLargeThreshold : this.NormalToLargeThreshold;
            Size smallToNormalThreshold = this.SmallToNormalThreshold.IsEmpty ? defaultSmallToNormalThreshold : this.SmallToNormalThreshold;
            Size largeToNormalThreshold = this.LargeToNormalThreshold.IsEmpty ? defaultLargeToNormalThreshold : this.LargeToNormalThreshold;
            bool smallToNormalthresholdGapExists = ((normalToSmallThreshold.Width - smallToNormalThreshold.Width) > double.Epsilon) || ((normalToSmallThreshold.Height - smallToNormalThreshold.Height) > double.Epsilon);
            bool normalToLargeThresholdGapExists = ((largeToNormalThreshold.Width - normalToLargeThreshold.Width) > double.Epsilon) || ((largeToNormalThreshold.Height - normalToLargeThreshold.Height) > double.Epsilon);
            if (smallToNormalthresholdGapExists || normalToLargeThresholdGapExists)
            {
                switch (this.State)
                {
                    case FluidContentControlState.Small:
                        if ((newState == FluidContentControlState.Normal) && smallToNormalthresholdGapExists)
                        {
                            result = true;
                        }
                        break;

                    case FluidContentControlState.Normal:
                        if ((newState != FluidContentControlState.Small) || !smallToNormalthresholdGapExists)
                        {
                            if ((newState == FluidContentControlState.Large) && normalToLargeThresholdGapExists)
                            {
                                result = true;
                            }
                            break;
                        }
                        result = true;
                        break;

                    case FluidContentControlState.Large:
                        if ((newState == FluidContentControlState.Normal) && normalToLargeThresholdGapExists)
                        {
                            result = true;
                        }
                        break;
                }
            }
            if ((result || (this.NumberOfSetContentProperties != 2)) || (((normalToLargeThreshold.Width - normalToSmallThreshold.Width) <= double.Epsilon) && ((normalToLargeThreshold.Height - normalToSmallThreshold.Height) <= double.Epsilon)))
            {
                return result;
            }
            return (base.Content == null);
        }

        public Telerik.Windows.Controls.ContentChangeMode ContentChangeMode
        {
            get
            {
                return (Telerik.Windows.Controls.ContentChangeMode) base.GetValue(ContentChangeModeProperty);
            }
            set
            {
                base.SetValue(ContentChangeModeProperty, value);
            }
        }

        public object LargeContent
        {
            get
            {
                return base.GetValue(LargeContentProperty);
            }
            set
            {
                base.SetValue(LargeContentProperty, value);
            }
        }

        public DataTemplate LargeContentTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(LargeContentTemplateProperty);
            }
            set
            {
                base.SetValue(LargeContentTemplateProperty, value);
            }
        }

        public Size LargeToNormalThreshold
        {
            get
            {
                return (Size) base.GetValue(LargeToNormalThresholdProperty);
            }
            set
            {
                base.SetValue(LargeToNormalThresholdProperty, value);
            }
        }

        public Size NormalToLargeThreshold
        {
            get
            {
                return (Size) base.GetValue(NormalToLargeThresholdProperty);
            }
            set
            {
                base.SetValue(NormalToLargeThresholdProperty, value);
            }
        }

        public Size NormalToSmallThreshold
        {
            get
            {
                return (Size) base.GetValue(NormalToSmallThresholdProperty);
            }
            set
            {
                base.SetValue(NormalToSmallThresholdProperty, value);
            }
        }

        internal int NumberOfSetContentProperties
        {
            get
            {
                return ((((this.SmallContent != null) ? 1 : 0) + ((base.Content != null) ? 1 : 0)) + ((this.LargeContent != null) ? 1 : 0));
            }
        }

        public object SmallContent
        {
            get
            {
                return base.GetValue(SmallContentProperty);
            }
            set
            {
                base.SetValue(SmallContentProperty, value);
            }
        }

        public DataTemplate SmallContentTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(SmallContentTemplateProperty);
            }
            set
            {
                base.SetValue(SmallContentTemplateProperty, value);
            }
        }

        public Size SmallToNormalThreshold
        {
            get
            {
                return (Size) base.GetValue(SmallToNormalThresholdProperty);
            }
            set
            {
                base.SetValue(SmallToNormalThresholdProperty, value);
            }
        }

        public FluidContentControlState State
        {
            get
            {
                return (FluidContentControlState) base.GetValue(StateProperty);
            }
            set
            {
                base.SetValue(StateProperty, value);
            }
        }

        public object VisibleContent
        {
            get
            {
                switch (this.State)
                {
                    case FluidContentControlState.Small:
                        return this.SmallContent;

                    case FluidContentControlState.Normal:
                        return base.Content;

                    case FluidContentControlState.Large:
                        return this.LargeContent;
                }
                return base.Content;
            }
        }
    }
}

