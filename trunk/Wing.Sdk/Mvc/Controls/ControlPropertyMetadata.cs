using System;

namespace Wing.Mvc.Controls
{
    public sealed class ControlPropertyMetadata
    {
        public ControlPropertyMetadata(ControlPropertyChangedCallback propertyChangedCallback, Object defaultValue = null, IControlPropertyApplier propertyApplier = null, bool readOnly = false)
        {
            this.PropertyChangedCallback = propertyChangedCallback;
            this.DefaultValue = defaultValue;
            this.Applier = propertyApplier;
            this.ReadOnly = readOnly;
        }

        public ControlPropertyMetadata(Object defaultValue, ControlPropertyChangedCallback propertyChangedCallback = null) : this(propertyChangedCallback, defaultValue, null) { }
        public ControlPropertyMetadata(Object defaultValue, IControlPropertyApplier applier = null) : this(null, defaultValue, applier) { }
        public ControlPropertyMetadata(IControlPropertyApplier applier, ControlPropertyChangedCallback propertyChangedCallback = null) : this(propertyChangedCallback, null, applier) { }

        internal ControlPropertyChangedCallback PropertyChangedCallback { get; private set; }
        internal IControlPropertyApplier Applier { get; private set; }
        public object DefaultValue { get; private set; }
        public bool ReadOnly { get; private set; }
    }
}
