namespace Telerik.Windows.Controls.Animation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    internal static class AnimationExtensions
    {
        internal static AnimationContext AdjustSpeed(this AnimationContext target)
        {
            target.Instance.SpeedRatio = AnimationManager.AnimationSpeedRatio;
            return target;
        }

        internal static AnimationContext Animate(this AnimationContext target, params FrameworkElement[] newTargets)
        {
            target.Targets.Clear();
            foreach (FrameworkElement newElement in newTargets)
            {
                target.Targets.Add(newElement);
            }
            return target;
        }

        internal static ColorAnimation Clone(this ColorAnimation target)
        {
            ColorAnimation result = new ColorAnimation {
                By = target.By,
                From = target.From,
                To = target.To
            };
            CloneTimeline(target, result);
            return result;
        }

        internal static DiscreteObjectKeyFrame Clone(this DiscreteObjectKeyFrame target)
        {
            return new DiscreteObjectKeyFrame { KeyTime = target.KeyTime, Value = target.Value };
        }

        internal static DoubleAnimation Clone(this DoubleAnimation target)
        {
            DoubleAnimation result = new DoubleAnimation {
                By = target.By,
                From = target.From,
                To = target.To
            };
            CloneTimeline(target, result);
            return result;
        }

        internal static ObjectAnimationUsingKeyFrames Clone(this ObjectAnimationUsingKeyFrames target)
        {
            ObjectAnimationUsingKeyFrames result = new ObjectAnimationUsingKeyFrames();
            foreach (ObjectKeyFrame keyframe in target.KeyFrames)
            {
                DiscreteObjectKeyFrame discreteKeyframe = keyframe as DiscreteObjectKeyFrame;
                if (discreteKeyframe != null)
                {
                    result.KeyFrames.Add(discreteKeyframe.Clone());
                }
            }
            CloneTimeline(target, result);
            return result;
        }

        internal static PointAnimation Clone(this PointAnimation target)
        {
            PointAnimation result = new PointAnimation {
                By = target.By,
                From = target.From,
                To = target.To
            };
            CloneTimeline(target, result);
            return result;
        }

        internal static Storyboard Clone(this Storyboard target)
        {
            Storyboard result = new Storyboard();
            CloneTimeline(target, result);
            foreach (Timeline child in target.Children)
            {
                Storyboard storyTimeline = child as Storyboard;
                if (storyTimeline != null)
                {
                    result.Children.Add(storyTimeline.Clone());
                }
                DoubleAnimation doubleTimeline = child as DoubleAnimation;
                if (doubleTimeline != null)
                {
                    result.Children.Add(doubleTimeline.Clone());
                }
                ColorAnimation colorTimeline = child as ColorAnimation;
                if (colorTimeline != null)
                {
                    result.Children.Add(colorTimeline.Clone());
                }
                PointAnimation pointAnimation = child as PointAnimation;
                if (pointAnimation != null)
                {
                    result.Children.Add(pointAnimation.Clone());
                }
                ObjectAnimationUsingKeyFrames discreteObjectTimeline = child as ObjectAnimationUsingKeyFrames;
                if (discreteObjectTimeline != null)
                {
                    result.Children.Add(discreteObjectTimeline.Clone());
                }
            }
            return result;
        }

        internal static void CloneTimeline(Timeline source, Timeline target)
        {
            target.AutoReverse = source.AutoReverse;
            target.BeginTime = source.BeginTime;
            target.Duration = source.Duration;
            target.FillBehavior = source.FillBehavior;
            target.RepeatBehavior = source.RepeatBehavior;
            target.SpeedRatio = source.SpeedRatio;
            Storyboard.SetTargetName(target, Storyboard.GetTargetName(source));
            Storyboard.SetTargetProperty(target, Storyboard.GetTargetProperty(source));
        }

        internal static AnimationContext Create()
        {
            return new AnimationContext();
        }

        internal static AnimationContext Discrete(this AnimationContext target, DependencyProperty propertyPath, params object[] args)
        {
            List<object> values = args.ToList<object>();
            if ((args.Length % 2) != 0)
            {
                throw new InvalidOperationException("Params should come in a time-value pair");
            }
            target.StartIndex = target.EndIndex;
            target.EndIndex += target.Targets.Count;
            int elementCount = 0;
            foreach (FrameworkElement element in target.Targets)
            {
                if (target.IsUpdate)
                {
                    ObjectAnimationUsingKeyFrames moveX = target.Instance.Children[target.StartIndex + elementCount] as ObjectAnimationUsingKeyFrames;
                    for (int i = 0; i < values.Count; i += 2)
                    {
                        DiscreteObjectKeyFrame keyFrame = moveX.KeyFrames[i / 2] as DiscreteObjectKeyFrame;
                        keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(Convert.ToDouble(values[i], CultureInfo.InvariantCulture)));
                        keyFrame.Value = values[i + 1];
                    }
                    elementCount++;
                    continue;
                }
                ObjectAnimationUsingKeyFrames _moveX = new ObjectAnimationUsingKeyFrames();
                Storyboard.SetTarget(_moveX, element);
                Storyboard.SetTargetProperty(_moveX, new PropertyPath(propertyPath));
                for (int i = 0; i < values.Count; i += 2)
                {
                    _moveX.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(Convert.ToDouble(values[i], CultureInfo.InvariantCulture))), Value = values[i + 1] });
                }
                target.Instance.Children.Add(_moveX);
            }
            return target;
        }

        internal static AnimationContext EaseAll(this AnimationContext target, IEasingFunction easing)
        {
            for (int num = target.StartIndex; num < target.EndIndex; num++)
            {
                DoubleAnimationUsingKeyFrames animation = target.Instance.Children[num] as DoubleAnimationUsingKeyFrames;
                foreach (EasingDoubleKeyFrame keyFrame in animation.KeyFrames.Cast<EasingDoubleKeyFrame>())
                {
                    keyFrame.EasingFunction = easing;
                }
            }
            return target;
        }

        internal static AnimationContext Easings(this AnimationContext target, IEasingFunction easing)
        {
            return target.Easings(1, easing);
        }

        internal static AnimationContext Easings(this AnimationContext target, int index, IEasingFunction easing)
        {
            for (int num = target.StartIndex; num < target.EndIndex; num++)
            {
                DoubleAnimationUsingKeyFrames animation = target.Instance.Children[num] as DoubleAnimationUsingKeyFrames;
                EasingDoubleKeyFrame keyFrame = animation.KeyFrames[index] as EasingDoubleKeyFrame;
                keyFrame.EasingFunction = easing;
            }
            return target;
        }

        internal static void EnsureDefaultTransforms(this UIElement element)
        {
            TransformGroup group = element.RenderTransform as TransformGroup;
            if ((((group == null) || (group.Children.Count < 4)) || (!(group.Children[0] is ScaleTransform) || !(group.Children[1] is SkewTransform))) || (!(group.Children[2] is RotateTransform) || !(group.Children[3] is TranslateTransform)))
            {
                group = new TransformGroup();
                group.Children.Add(new ScaleTransform());
                group.Children.Add(new SkewTransform());
                group.Children.Add(new RotateTransform());
                group.Children.Add(new TranslateTransform());
                element.RenderTransform = group;
            }
        }

        internal static AnimationContext EnsureDefaultTransforms(this AnimationContext target)
        {
            foreach (FrameworkElement element in target.Targets)
            {
                TransformGroup group = element.RenderTransform as TransformGroup;
                if ((((group == null) || (group.Children.Count < 4)) || (!(group.Children[0] is ScaleTransform) || !(group.Children[1] is SkewTransform))) || (!(group.Children[2] is RotateTransform) || !(group.Children[3] is TranslateTransform)))
                {
                    group = new TransformGroup();
                    group.Children.Add(new ScaleTransform());
                    group.Children.Add(new SkewTransform());
                    group.Children.Add(new RotateTransform());
                    group.Children.Add(new TranslateTransform());
                    element.RenderTransform = group;
                }
            }
            return target;
        }

        internal static AnimationContext EnsureOpacityMask(this AnimationContext target)
        {
            foreach (FrameworkElement element in target.Targets)
            {
                LinearGradientBrush mask = new LinearGradientBrush {
                    EndPoint = new Point(0.0, 1.0),
                    GradientStops = { new GradientStop { Offset = 0.0, Color = Colors.Transparent }, new GradientStop { Offset = 0.0, Color = Colors.Black }, new GradientStop { Offset = 1.0, Color = Colors.Black }, new GradientStop { Offset = 1.0, Color = Colors.Transparent } },
                    RelativeTransform = new TranslateTransform(),
                    Transform = new TranslateTransform()
                };
                element.OpacityMask = mask;
            }
            return target;
        }

        internal static AnimationContext EnsurePlaneProjection(this AnimationContext target)
        {
            foreach (FrameworkElement element in target.Targets)
            {
                element.Projection = new PlaneProjection();
            }
            return target;
        }

        internal static RotateTransform GetRotateTransform(this UIElement element)
        {
            element.EnsureDefaultTransforms();
            return ((element.RenderTransform as TransformGroup).Children[2] as RotateTransform);
        }

        internal static ScaleTransform GetScaleTransform(this UIElement element)
        {
            element.EnsureDefaultTransforms();
            return ((element.RenderTransform as TransformGroup).Children[0] as ScaleTransform);
        }

        internal static SkewTransform GetSkewTransform(this UIElement element)
        {
            element.EnsureDefaultTransforms();
            return ((element.RenderTransform as TransformGroup).Children[1] as SkewTransform);
        }

        internal static TranslateTransform GetTranslateTransform(this UIElement element)
        {
            element.EnsureDefaultTransforms();
            return ((element.RenderTransform as TransformGroup).Children[3] as TranslateTransform);
        }

        internal static AnimationContext Height(this AnimationContext target, params double[] args)
        {
            return target.SingleProperty("(FrameworkElement.Height)", args);
        }

        internal static AnimationContext HoldEndFillBehavior(this AnimationContext target)
        {
            target.Instance.FillBehavior = FillBehavior.HoldEnd;
            return target;
        }

        internal static AnimationContext MoveX(this AnimationContext target, params double[] args)
        {
            return target.SingleProperty("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)", args);
        }

        internal static AnimationContext MoveY(this AnimationContext target, params double[] args)
        {
            return target.SingleProperty("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)", args);
        }

        internal static AnimationContext OnComplete(this AnimationContext target, Action callback)
        {
            if (target.Instance != null)
            {
                EventHandler completeHandler = null;
                completeHandler = delegate (object s, EventArgs e) {
                    callback();
                    target.Instance.Completed -= completeHandler;
                };
                target.Instance.Completed += completeHandler;
            }
            return target;
        }

        internal static AnimationContext Opacity(this AnimationContext target, params double[] args)
        {
            return target.SingleProperty("(UIElement.Opacity)", args);
        }

        internal static AnimationContext OpacityMaskMoveX(this AnimationContext target, params double[] args)
        {
            return target.SingleProperty("(UIElement.OpacityMask).(Brush.Transform).(TranslateTransform.X)", args);
        }

        internal static AnimationContext OpacityMaskMoveY(this AnimationContext target, params double[] args)
        {
            return target.SingleProperty("(UIElement.OpacityMask).(Brush.Transform).(TranslateTransform.Y)", args);
        }

        internal static AnimationContext OpacityMaskRelativeMoveX(this AnimationContext target, params double[] args)
        {
            return target.SingleProperty("(UIElement.OpacityMask).(Brush.RelativeTransform).(TranslateTransform.X)", args);
        }

        internal static AnimationContext OpacityMaskRelativeMoveY(this AnimationContext target, params double[] args)
        {
            return target.SingleProperty("(UIElement.OpacityMask).(Brush.RelativeTransform).(TranslateTransform.Y)", args);
        }

        internal static AnimationContext Origin(this AnimationContext target, double x1, double x2)
        {
            foreach (FrameworkElement element in target.Targets)
            {
                element.RenderTransformOrigin = new Point(x1, x2);
            }
            return target;
        }

        internal static AnimationContext PlayIfPossible(this AnimationContext target, Control hostControl)
        {
            if (AnimationManager.IsGlobalAnimationEnabled && AnimationManager.GetIsAnimationEnabled(hostControl))
            {
                target.Instance.Begin();
            }
            return target;
        }

        internal static AnimationContext RotationY(this AnimationContext target, params double[] args)
        {
            return target.SingleProperty("(UIElement.Projection).(PlaneProjection.RotationY)", args);
        }

        internal static AnimationContext Scale(this AnimationContext target, params double[] args)
        {
            List<double> values = args.ToList<double>();
            if ((args.Length % 2) != 0)
            {
                throw new InvalidOperationException("Params should come in a time-value pair");
            }
            foreach (FrameworkElement element in target.Targets)
            {
                if (target.IsUpdate)
                {
                    throw new NotImplementedException();
                }
                DoubleAnimationUsingKeyFrames scaleX = new DoubleAnimationUsingKeyFrames();
                Storyboard.SetTarget(scaleX, element);
                Storyboard.SetTargetProperty(scaleX, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)", new object[0]));
                DoubleAnimationUsingKeyFrames scaleY = new DoubleAnimationUsingKeyFrames();
                Storyboard.SetTarget(scaleY, element);
                Storyboard.SetTargetProperty(scaleY, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)", new object[0]));
                for (int i = 0; i < values.Count; i += 2)
                {
                    scaleX.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(values[i])), Value = values[i + 1] });
                    scaleY.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(values[i])), Value = values[i + 1] });
                }
                target.Instance.Children.Add(scaleX);
                target.Instance.Children.Add(scaleY);
            }
            target.StartIndex = target.EndIndex;
            target.EndIndex += 2 * target.Targets.Count;
            return target;
        }

        internal static AnimationContext ScaleX(this AnimationContext target, params double[] args)
        {
            return target.SingleProperty("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)", args);
        }

        internal static AnimationContext ScaleY(this AnimationContext target, params double[] args)
        {
            return target.SingleProperty("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)", args);
        }

        private static AnimationContext SingleProperty(this AnimationContext target, string propertyPath, params double[] args)
        {
            List<double> values = args.ToList<double>();
            if ((args.Length % 2) != 0)
            {
                throw new InvalidOperationException("Params should come in a time-value pair");
            }
            target.StartIndex = target.EndIndex;
            target.EndIndex += target.Targets.Count;
            int elementCount = 0;
            foreach (FrameworkElement element in target.Targets)
            {
                if (target.IsUpdate)
                {
                    DoubleAnimationUsingKeyFrames moveX = target.Instance.Children[target.StartIndex + elementCount] as DoubleAnimationUsingKeyFrames;
                    for (int i = 0; i < values.Count; i += 2)
                    {
                        EasingDoubleKeyFrame keyFrame = moveX.KeyFrames[i / 2] as EasingDoubleKeyFrame;
                        keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(values[i]));
                        keyFrame.Value = values[i + 1];
                    }
                    elementCount++;
                    continue;
                }
                DoubleAnimationUsingKeyFrames _moveX = new DoubleAnimationUsingKeyFrames();
                Storyboard.SetTarget(_moveX, element);
                Storyboard.SetTargetProperty(_moveX, new PropertyPath(propertyPath, new object[0]));
                for (int i = 0; i < values.Count; i += 2)
                {
                    _moveX.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(values[i])), Value = values[i + 1] });
                }
                target.Instance.Children.Add(_moveX);
            }
            return target;
        }

        internal static AnimationContext SingleProperty(this AnimationContext target, DependencyProperty propertyPath, params double[] args)
        {
            List<double> values = args.ToList<double>();
            if ((args.Length % 2) != 0)
            {
                throw new InvalidOperationException("Params should come in a time-value pair");
            }
            target.StartIndex = target.EndIndex;
            target.EndIndex += target.Targets.Count;
            int elementCount = 0;
            foreach (FrameworkElement element in target.Targets)
            {
                if (target.IsUpdate)
                {
                    DoubleAnimationUsingKeyFrames moveX = target.Instance.Children[target.StartIndex + elementCount] as DoubleAnimationUsingKeyFrames;
                    for (int i = 0; i < values.Count; i += 2)
                    {
                        EasingDoubleKeyFrame keyFrame = moveX.KeyFrames[i / 2] as EasingDoubleKeyFrame;
                        keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(values[i]));
                        keyFrame.Value = values[i + 1];
                    }
                    elementCount++;
                    continue;
                }
                DoubleAnimationUsingKeyFrames _moveX = new DoubleAnimationUsingKeyFrames();
                Storyboard.SetTarget(_moveX, element);
                Storyboard.SetTargetProperty(_moveX, new PropertyPath(propertyPath));
                for (int i = 0; i < values.Count; i += 2)
                {
                    _moveX.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(values[i])), Value = values[i + 1] });
                }
                target.Instance.Children.Add(_moveX);
            }
            return target;
        }

        internal static AnimationContext StopFillBehavior(this AnimationContext target)
        {
            target.Instance.FillBehavior = FillBehavior.Stop;
            return target;
        }

        public static AnimationContext Update(this Storyboard target)
        {
            return new AnimationContext { Instance = target, IsUpdate = true };
        }

        internal static AnimationContext Width(this AnimationContext target, params double[] args)
        {
            return target.SingleProperty("(FrameworkElement.Width)", args);
        }

        internal static AnimationContext With(this AnimationContext target, params FrameworkElement[] newElements)
        {
            foreach (FrameworkElement elements in newElements)
            {
                target.Targets.Add(elements);
            }
            return target;
        }

        internal static AnimationContext Without(this AnimationContext target, params FrameworkElement[] newElements)
        {
            foreach (FrameworkElement elements in newElements)
            {
                target.Targets.Remove(elements);
            }
            return target;
        }

        internal class AnimationContext
        {
            public AnimationContext()
            {
                this.Instance = new Storyboard { FillBehavior = FillBehavior.HoldEnd };
                this.Targets = new List<FrameworkElement>(8);
            }

            public int EndIndex { get; set; }

            internal Storyboard Instance { get; set; }

            internal bool IsUpdate { get; set; }

            public int StartIndex { get; set; }

            internal ICollection<FrameworkElement> Targets { get; private set; }
        }
    }
}

