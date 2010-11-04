namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct Vector
    {
        private double _x;
        private double _y;
        internal Vector(double x, double y)
        {
            this._x = x;
            this._y = y;
        }

        internal double X
        {
            get
            {
                return this._x;
            }
            set
            {
                this._x = value;
            }
        }
        internal double Y
        {
            get
            {
                return this._y;
            }
            set
            {
                this._y = value;
            }
        }
    }
}

