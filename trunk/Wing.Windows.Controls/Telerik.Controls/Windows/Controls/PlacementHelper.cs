namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;

    internal class PlacementHelper
    {
        private FlowDirection flowDirection;
        private Point offset;
        private Rect placementRect;
        private Size popupSize;
        private Rect viewPortRect;

        public PlacementHelper(Rect placementRect, Size popupSize, double horizontalOffset, double verticalOffset) : this(placementRect, popupSize, horizontalOffset, verticalOffset, ApplicationHelper.ApplicationSize, FlowDirection.LeftToRight)
        {
        }

        public PlacementHelper(Rect placementRect, Size popupSize, double horizontalOffset, double verticalOffset, FlowDirection flowDirection) : this(placementRect, popupSize, horizontalOffset, verticalOffset, ApplicationHelper.ApplicationSize, flowDirection)
        {
        }

        public PlacementHelper(Rect placementRect, Size popupSize, double horizontalOffset, double verticalOffset, Size viewPortSize, FlowDirection flowDirection)
        {
            this.popupSize = popupSize;
            this.placementRect = placementRect;
            this.viewPortRect = new Rect(0.0, 0.0, viewPortSize.Width, viewPortSize.Height);
            this.offset = new Point(horizontalOffset, verticalOffset);
            this.flowDirection = flowDirection;
        }

        private Point AdjustForBottom()
        {
            return new Point(this.AdjustedLeftForVerticalPlacement, Math.Max((double) 0.0, (double) (this.viewPortRect.Height - this.popupSize.Height)));
        }

        private Point AdjustForLeft()
        {
            return new Point(0.0, this.AdjustedTopForHorizontalPlacement);
        }

        private Point AdjustForRight()
        {
            return new Point(Math.Max((double) 0.0, (double) (this.viewPortRect.Width - this.popupSize.Width)), this.AdjustedTopForHorizontalPlacement);
        }

        private Point AdjustForTop()
        {
            return new Point(this.AdjustedLeftForVerticalPlacement, 0.0);
        }

        private bool CanFitBottom()
        {
            return (((this.placementRect.Bottom + this.popupSize.Height) + this.offset.Y) < this.viewPortRect.Height);
        }

        private bool CanFitLeft()
        {
            return ((this.placementRect.Left - this.offset.X) >= this.popupSize.Width);
        }

        private bool CanFitRight()
        {
            return (((this.placementRect.Right + this.popupSize.Width) + this.offset.X) < this.viewPortRect.Width);
        }

        private bool CanFitTop()
        {
            return ((this.placementRect.Top - this.offset.Y) >= this.popupSize.Height);
        }

        private Point FitAbsolute()
        {
            return new Point(this.AdjustedLeftForVerticalPlacement, this.AdjustedTopForHorizontalPlacement);
        }

        private Point FitBottom()
        {
            return new Point(this.AdjustedLeftForVerticalPlacement, this.placementRect.Bottom + this.offset.Y);
        }

        private Point FitCenter()
        {
            Point center = new Point(Math.Max((double) 0.0, (double) ((this.placementRect.X + this.offset.X) + ((this.placementRect.Width - this.popupSize.Width) / 2.0))), Math.Max((double) 0.0, (double) ((this.placementRect.Y + this.offset.Y) + ((this.placementRect.Height - this.popupSize.Height) / 2.0))));
            if ((center.X + this.popupSize.Width) > this.viewPortRect.Width)
            {
                center.X = Math.Max((double) 0.0, (double) (this.viewPortRect.Width - this.popupSize.Width));
            }
            if ((center.Y + this.popupSize.Height) > this.viewPortRect.Height)
            {
                center.Y = Math.Max((double) 0.0, (double) (this.viewPortRect.Height - this.popupSize.Height));
            }
            return center;
        }

        private Point FitLeft()
        {
            return new Point((this.placementRect.Left - this.popupSize.Width) - this.offset.X, this.AdjustedTopForHorizontalPlacement);
        }

        private Point FitRight()
        {
            return new Point(this.placementRect.Right + this.offset.X, this.AdjustedTopForHorizontalPlacement);
        }

        private Point FitTop()
        {
            return new Point(this.AdjustedLeftForVerticalPlacement, (this.placementRect.Top - this.popupSize.Height) - this.offset.Y);
        }

        public Point GetPlacementOrigin(PlacementMode placementMode)
        {
            Point placement = this.GetPlacementOriginWithoutRounding(placementMode);
            return new Point(Math.Round(placement.X, 0), Math.Round(placement.Y, 0));
        }

        private Point GetPlacementOriginWithoutRounding(PlacementMode placementMode)
        {
            bool rightToLeft = this.IsRightToLeft;
            if (rightToLeft)
            {
                if (placementMode == PlacementMode.Left)
                {
                    placementMode = PlacementMode.Right;
                }
                else if (placementMode == PlacementMode.Right)
                {
                    placementMode = PlacementMode.Left;
                }
            }
            switch (placementMode)
            {
                case PlacementMode.Absolute:
                    this.ActualPlacement = PlacementMode.Absolute;
                    return this.FitAbsolute();

                case PlacementMode.Bottom:
                    if (!this.CanFitBottom())
                    {
                        if (this.CanFitTop())
                        {
                            this.ActualPlacement = PlacementMode.Top;
                            return this.FitTop();
                        }
                        this.ActualPlacement = PlacementMode.Bottom;
                        return this.AdjustForBottom();
                    }
                    this.ActualPlacement = PlacementMode.Bottom;
                    return this.FitBottom();

                case PlacementMode.Center:
                    this.ActualPlacement = PlacementMode.Center;
                    return this.FitCenter();

                case PlacementMode.Right:
                    if (!this.CanFitRight())
                    {
                        if (this.CanFitLeft())
                        {
                            this.ActualPlacement = PlacementMode.Left;
                            if (rightToLeft)
                            {
                                return this.FitRight();
                            }
                            return this.FitLeft();
                        }
                        this.ActualPlacement = PlacementMode.Right;
                        if (rightToLeft)
                        {
                            return this.AdjustForLeft();
                        }
                        return this.AdjustForRight();
                    }
                    this.ActualPlacement = PlacementMode.Right;
                    if (!rightToLeft)
                    {
                        return this.FitRight();
                    }
                    return this.FitLeft();

                case PlacementMode.Left:
                    if (!this.CanFitLeft())
                    {
                        if (this.CanFitRight())
                        {
                            this.ActualPlacement = PlacementMode.Right;
                            if (rightToLeft)
                            {
                                return this.FitLeft();
                            }
                            return this.FitRight();
                        }
                        if (rightToLeft)
                        {
                            return this.AdjustForRight();
                        }
                        return this.AdjustForLeft();
                    }
                    this.ActualPlacement = PlacementMode.Left;
                    if (!rightToLeft)
                    {
                        return this.FitLeft();
                    }
                    return this.FitRight();

                case PlacementMode.Top:
                    if (!this.CanFitTop())
                    {
                        if (this.CanFitBottom())
                        {
                            this.ActualPlacement = PlacementMode.Bottom;
                            return this.FitBottom();
                        }
                        this.ActualPlacement = PlacementMode.Top;
                        return this.AdjustForTop();
                    }
                    this.ActualPlacement = PlacementMode.Top;
                    return this.FitTop();
            }
            throw new NotSupportedException("placementMode");
        }

        internal PlacementMode ActualPlacement { get; set; }

        private double AdjustedLeftForVerticalPlacement
        {
            get
            {
                return Math.Max(0.0, Math.Min((double) (this.placementRect.Left + this.offset.X), (double) (this.viewPortRect.Width - this.popupSize.Width)));
            }
        }

        private double AdjustedTopForHorizontalPlacement
        {
            get
            {
                return Math.Max(0.0, Math.Min((double) (this.placementRect.Top + this.offset.Y), (double) (this.viewPortRect.Height - this.popupSize.Height)));
            }
        }

        private bool IsRightToLeft
        {
            get
            {
                return (this.flowDirection == FlowDirection.RightToLeft);
            }
        }
    }
}

