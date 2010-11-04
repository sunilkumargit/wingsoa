namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;

    public static class MathUtilities
    {
        static MathUtilities()
        {
            PointDifferenceThreshold = 5.0;
        }

        internal static bool AreAllPositive(params int[] values)
        {
            foreach (int number in values)
            {
                if (number < 0)
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool AreClose(Point point1, Point point2)
        {
            Vector difference = new Vector(point1.X - point2.X, point1.Y - point2.Y);
            return (Math.Sqrt((difference.X * difference.X) + (difference.Y * difference.Y)) < PointDifferenceThreshold);
        }

        public static double Clamp(double value, double min, double max)
        {
            double clampedValue = 0.0;
            if (value > max)
            {
                clampedValue = max;
            }
            else
            {
                clampedValue = value;
            }
            if (clampedValue < min)
            {
                clampedValue = min;
            }
            return clampedValue;
        }

        public static int Clamp(int value, int min, int max)
        {
            int clampedValue = 0;
            if (value > max)
            {
                clampedValue = max;
            }
            else
            {
                clampedValue = value;
            }
            if (clampedValue < min)
            {
                clampedValue = min;
            }
            return clampedValue;
        }

        public static bool IsInRange(int value, int min, int max)
        {
            return ((value >= min) && (value <= max));
        }

        internal static double PointDifferenceThreshold{get;set;}
    }
}

