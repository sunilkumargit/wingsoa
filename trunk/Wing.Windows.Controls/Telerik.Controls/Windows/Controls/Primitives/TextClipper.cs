namespace Telerik.Windows.Controls.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using Telerik.Windows.Controls;

    public class TextClipper : ContentControl
    {
        private int clipStart;
        private double currentWidth;
        private double dotsWidth;
        private StringBuilder fakeBuilder;
        private List<double> indexWidths;
        private bool isClippingActive;
        private bool isTextBlockOutdated;
        private bool justUpdated;
        private double realDesiredWidth;
        private string realText;
        private TextBlock textBlock;

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (!this.isClippingActive || this.justUpdated)
            {
                this.justUpdated = false;
                return base.ArrangeOverride(finalSize);
            }
            if (this.textBlock == null)
            {
                base.InvalidateMeasure();
                return base.ArrangeOverride(finalSize);
            }
            double finalWidth = finalSize.Width;
            if (finalWidth < this.currentWidth)
            {
                while ((finalWidth < this.currentWidth) && (this.clipStart >= 1))
                {
                    this.clipStart--;
                    this.currentWidth = this.indexWidths[this.clipStart] + this.dotsWidth;
                }
                if (this.clipStart == 0)
                {
                    this.textBlock.Text = string.Empty;
                    this.justUpdated = true;
                    this.currentWidth = 0.0;
                }
                else
                {
                    this.fakeBuilder = new StringBuilder(this.realText);
                    this.fakeBuilder.Remove(this.clipStart, this.fakeBuilder.Length - this.clipStart);
                    this.fakeBuilder.Append("...");
                    this.textBlock.Text = this.fakeBuilder.ToString();
                    this.justUpdated = true;
                }
            }
            else if ((finalWidth > this.currentWidth) && (this.clipStart < (this.realText.Length - 1)))
            {
                if (finalWidth <= (this.realDesiredWidth - 5.0))
                {
                    while (((this.clipStart + 1) < this.realText.Length) && (finalWidth > (this.indexWidths[this.clipStart + 1] + this.dotsWidth)))
                    {
                        this.clipStart++;
                        this.currentWidth = this.indexWidths[this.clipStart] + this.dotsWidth;
                    }
                    if (this.clipStart == (this.realText.Length - 1))
                    {
                        this.textBlock.Text = this.realText;
                        this.currentWidth = this.realDesiredWidth;
                        this.justUpdated = true;
                    }
                    else if (this.clipStart == 0)
                    {
                        this.textBlock.Text = string.Empty;
                        this.justUpdated = true;
                        this.currentWidth = 0.0;
                    }
                    else
                    {
                        this.fakeBuilder = new StringBuilder(this.realText);
                        this.fakeBuilder.Remove(this.clipStart, this.fakeBuilder.Length - this.clipStart);
                        this.fakeBuilder.Append("...");
                        this.textBlock.Text = this.fakeBuilder.ToString();
                        this.justUpdated = true;
                    }
                }
                else
                {
                    this.textBlock.Text = this.realText;
                    this.justUpdated = true;
                }
            }
            return base.ArrangeOverride(finalSize);
        }

        private void FindTextBlock()
        {
            if ((this.textBlock == null) || this.isTextBlockOutdated)
            {
                this.textBlock = this.GetFirstDescendantOfType<TextBlock>();
                if (this.textBlock != null)
                {
                    this.isTextBlockOutdated = false;
                    StringBuilder builder = new StringBuilder(this.textBlock.Text);
                    this.indexWidths = new List<double>(builder.Length);
                    this.textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    this.realDesiredWidth = this.textBlock.DesiredSize.Width;
                    while (builder.Length > 0)
                    {
                        this.indexWidths.Add(this.textBlock.DesiredSize.Width);
                        builder.Remove(builder.Length - 1, 1);
                        this.textBlock.Text = builder.ToString();
                        this.textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                    this.textBlock.Text = "...";
                    this.textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    this.dotsWidth = this.textBlock.DesiredSize.Width;
                    this.textBlock.Text = this.realText;
                    if (this.indexWidths.Count > 0)
                    {
                        this.currentWidth = this.indexWidths[0];
                        this.clipStart = this.indexWidths.Count - 1;
                    }
                    else
                    {
                        this.currentWidth = 0.0;
                        this.clipStart = 0;
                    }
                    this.indexWidths.Reverse();
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (!this.isClippingActive)
            {
                return base.MeasureOverride(availableSize);
            }
            if ((this.textBlock == null) || this.isTextBlockOutdated)
            {
                this.FindTextBlock();
                return base.MeasureOverride(availableSize);
            }
            Size result = base.MeasureOverride(availableSize);
            return new Size(Math.Min(availableSize.Width, this.realDesiredWidth), result.Height);
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            this.realText = newContent as string;
            if ((this.textBlock != null) && (this.textBlock.Text != this.realText))
            {
                this.textBlock.Text = this.realText;
            }
            this.Reset();
            base.OnContentChanged(oldContent, newContent);
            this.isClippingActive = (this.realText != null) && !this.realText.Contains("\n");
            this.isTextBlockOutdated = true;
            base.InvalidateMeasure();
        }

        private void Reset()
        {
            this.isTextBlockOutdated = false;
            this.isClippingActive = false;
            this.textBlock = null;
            this.indexWidths = null;
            this.clipStart = 0;
            this.currentWidth = 0.0;
            this.justUpdated = false;
            this.realDesiredWidth = 0.0;
            this.dotsWidth = 0.0;
            this.fakeBuilder = null;
        }
    }
}

