namespace Telerik.Windows
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;

    internal class FrameworkPropertyMetadata : Telerik.Windows.PropertyMetadata
    {
        public FrameworkPropertyMetadata(object defaultValue) : base(defaultValue)
        {
        }

        public FrameworkPropertyMetadata(PropertyChangedCallback propertyChangedCallback) : base(propertyChangedCallback)
        {
        }

        public FrameworkPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback) : base(defaultValue, propertyChangedCallback)
        {
        }

        public FrameworkPropertyMetadata(object defaultValue, FrameworkPropertyMetadataOptions flags) : base(defaultValue)
        {
            this.TranslateFlags(flags);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public FrameworkPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback) : base(defaultValue, propertyChangedCallback, coerceValueCallback)
        {
        }

        public FrameworkPropertyMetadata(object defaultValue, FrameworkPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback) : base(defaultValue, propertyChangedCallback)
        {
            this.TranslateFlags(flags);
        }

        public FrameworkPropertyMetadata(object defaultValue, FrameworkPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback) : base(defaultValue, propertyChangedCallback, coerceValueCallback)
        {
            this.TranslateFlags(flags);
        }

        private static bool IsFlagSet(FrameworkPropertyMetadataOptions flag, FrameworkPropertyMetadataOptions flags)
        {
            return ((flags & flag) != FrameworkPropertyMetadataOptions.None);
        }

        private void TranslateFlags(FrameworkPropertyMetadataOptions flags)
        {
            if (IsFlagSet(FrameworkPropertyMetadataOptions.AffectsMeasure, flags))
            {
                this.AffectsMeasure = true;
            }
            if (IsFlagSet(FrameworkPropertyMetadataOptions.AffectsArrange, flags))
            {
                this.AffectsArrange = true;
            }
            if (IsFlagSet(FrameworkPropertyMetadataOptions.AffectsParentMeasure, flags))
            {
                this.AffectsParentMeasure = true;
            }
            if (IsFlagSet(FrameworkPropertyMetadataOptions.AffectsParentArrange, flags))
            {
                this.AffectsParentArrange = true;
            }
            if (IsFlagSet(FrameworkPropertyMetadataOptions.AffectsRender, flags))
            {
                this.AffectsRender = true;
            }
        }

        public bool AffectsArrange { get; set; }

        public bool AffectsMeasure { get; set; }

        public bool AffectsParentArrange { get; set; }

        public bool AffectsParentMeasure { get; set; }

        public bool AffectsRender { get; set; }
    }
}

