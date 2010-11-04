namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;

    internal class Face
    {
        public Face(Point3D topLeft, Point3D topRight, Point3D bl)
        {
            this.TL = topLeft;
            this.TR = topRight;
            this.BL = bl;
        }

        public Point3D BL { get; internal set; }

        public Point3D TL { get; internal set; }

        public Point3D TR { get; internal set; }
    }
}

