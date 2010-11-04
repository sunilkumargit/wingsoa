namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;

    internal class Point3D
    {
        public Point3D(double positionX, double positionY, double positionZ)
        {
            this.XPosition = positionX;
            this.YPosition = positionY;
            this.ZPosition = positionZ;
        }

        public double XPosition { get; set; }

        public double YPosition { get; set; }

        public double ZPosition { get; set; }
    }
}

