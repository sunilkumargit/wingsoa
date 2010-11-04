namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using Telerik.Windows.Controls.Design;

    [TypeConverter(typeof(MouseActionConverter))]
    public enum MouseAction
    {
        None,
        LeftClick,
        RightClick,
        MiddleClick,
        WheelClick,
        LeftDoubleClick,
        RightDoubleClick,
        MiddleDoubleClick
    }
}

