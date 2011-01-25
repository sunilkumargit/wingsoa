using System;

namespace Wing
{
    public class PropertyValueProvider : IValueProvider, IValueSetterProvider
    {
        public String PropertyName { get; set; }

        public PropertyValueProvider(String propertyName)
        {
            Assert.EmptyString(propertyName, "propertyName");
            PropertyName = propertyName;
        }


        public object GetValue(object param)
        {
            if (param == null)
                return null;
            else
                return ReflectionHelper.ReadProperty(param, PropertyName);
        }

        public void SetValue(object target, object value)
        {
            if (target == null)
                return;
            ReflectionHelper.WriteProperty(target, PropertyName, value);
        }
    }
}
