using System;
using System.ComponentModel;


namespace Wing.Mvc.Controls
{
    [System.Diagnostics.DebuggerDisplay("property: {Property.Name}, type: {Property.PropertyType}, value: {Value}, binding: {Binding != null}")]
    internal class ControlPropertySlot
    {
        private WeakReference _owner;
        private object _value;
        private bool _wasSetted;

        internal ControlPropertySlot(HtmlObject owner, ControlProperty property)
        {
            _owner = new WeakReference(owner);
            this.Property = property;
        }

        public HtmlObject Owner { get { return _owner.IsAlive ? (HtmlObject)_owner.Target : null; } }
        public ControlProperty Property { get; private set; }
        public Object StoredValue
        {
            get { return _value; }
            set
            {
                if (_value == value)
                    return;
                if (Property.IsReadOnly && _wasSetted)
                    throw new InvalidOperationException(String.Format("The property {0}.{1} is readonly and its value is already setted. The value of a readonly property can be setted just once."));
                _wasSetted = true;
                if (_value != null && Property.TrackingPropertyChangedEvent)
                    ((INotifyPropertyChanged)_value).PropertyChanged -= ValuePropertyChangedTrackHandler;
                _value = value;
                if (_value != null)
                {
                    if (!Property.PropertyType.IsAssignableFrom(_value.GetType()))
                    {
                        _value = ConversionHelper.Coerce(value, Property.PropertyType);
                        if (_value == null || !Property.PropertyType.IsAssignableFrom(_value.GetType()))
                            throw new Exception(String.Format("Error seting the value on preperty {0}.{1}", Owner.GetType().Name, Property.Name));

                    }
                    if (Property.TrackingPropertyChangedEvent)
                        ((INotifyPropertyChanged)_value).PropertyChanged += ValuePropertyChangedTrackHandler;
                }
            }
        }

        public void SetReadOnlyValue(Object value)
        {
            _wasSetted = false;
            StoredValue = value;
        }

        public Object CurrentValue
        {
            get { return _wasSetted ? _value : Property.DefaultValue; }
        }


        public Binding Binding;
        public int LastTemplateBindingSessionId;
        public IControlPropertyApplier CustomApplier;
        public IControlPropertyApplier CurrentApplier { get { return CustomApplier ?? Property.Applier; } }

        private void ValuePropertyChangedTrackHandler(Object sender, PropertyChangedEventArgs args)
        {
            Property.NotifyPropertyChanged(this.Owner, _value, _value);
        }
    }
}
