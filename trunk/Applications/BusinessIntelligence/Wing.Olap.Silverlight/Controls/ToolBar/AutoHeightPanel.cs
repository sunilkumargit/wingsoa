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

namespace Wing.AgOlap.Controls.ToolBar
{
    public class AutoHeightPanel : Panel
    {
        public AutoHeightPanel()
        {
            //this.Background = new SolidColorBrush(Colors.T.FromArgb(0xCC, 0xC0, 0xC0, 0xC0));
        }

        private TimeSpan _AnimationLength = TimeSpan.FromMilliseconds(200);

        protected override Size MeasureOverride(Size availableSize)
        {
            Size infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            double curX = 0, curY = 0, curLineHeight = 0;
            foreach (UIElement child in Children)
            {
                child.Measure(infiniteSize);

                if (curX + child.DesiredSize.Width > availableSize.Width)
                { //Wrap to next line
                    curY += curLineHeight;
                    curX = 0;
                    curLineHeight = 0;
                }

                curX += child.DesiredSize.Width;
                if (child.DesiredSize.Height > curLineHeight)
                    curLineHeight = child.DesiredSize.Height + 2 /*чтобы между рядами было расстояние*/;
            }

            if (curLineHeight > 0) 
            {
                curY += curLineHeight - 2; /*чтобы между рядами было расстояние*/
            }

            Size resultSize = new Size();
            resultSize.Width = double.IsPositiveInfinity(availableSize.Width) ? curX : availableSize.Width;
            resultSize.Height = double.IsPositiveInfinity(availableSize.Height) ? curY : availableSize.Height;

            return resultSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Children == null || this.Children.Count == 0)
                return finalSize;

            TranslateTransform trans = null;
            double curX = 0, curY = 0, curLineHeight = 0;

            foreach (UIElement child in Children)
            {
                trans = child.RenderTransform as TranslateTransform;
                if (trans == null)
                {
                    child.RenderTransformOrigin = new Point(0, 0);
                    trans = new TranslateTransform();
                    child.RenderTransform = trans;
                }

                if (curX + child.DesiredSize.Width > finalSize.Width)
                { //Wrap to next line
                    curY += curLineHeight;
                    curX = 0;
                    curLineHeight = 0;
                }

                child.Arrange(new Rect(curX, curY, child.DesiredSize.Width, child.DesiredSize.Height));

                //trans.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(curX, _AnimationLength), HandoffBehavior.Compose);
                //trans.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(curY, _AnimationLength), HandoffBehavior.Compose);

                curX += child.DesiredSize.Width;
                if (child.DesiredSize.Height > curLineHeight)
                    curLineHeight = child.DesiredSize.Height + 2/*чтобы между рядами было расстояние*/;
            }

            return finalSize;
        }
    }
}
