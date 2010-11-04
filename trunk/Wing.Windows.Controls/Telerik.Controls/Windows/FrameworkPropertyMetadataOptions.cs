namespace Telerik.Windows
{
    using System;

    [Flags]
    internal enum FrameworkPropertyMetadataOptions
    {
        AffectsArrange = 2,
        AffectsMeasure = 1,
        AffectsParentArrange = 8,
        AffectsParentMeasure = 4,
        AffectsRender = 0x10,
        BindsTwoWayByDefault = 0x100,
        Inherits = 0x20,
        None = 0
    }
}

