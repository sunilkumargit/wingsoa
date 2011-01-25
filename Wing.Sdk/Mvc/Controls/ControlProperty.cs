using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;


namespace Wing.Mvc.Controls
{
    /// <summary>
    /// Represents a property of a HtmlControl the can be customized.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Name} ({PropertyTypeName}), owner: {OwnerTypeName}")]
    public sealed class ControlProperty
    {
        private static List<ControlProperty> _properties = new List<ControlProperty>();
        private static Dictionary<Type, List<ControlProperty>> _typePropertiesCache = new Dictionary<Type, List<ControlProperty>>();
        private static Dictionary<Type, List<ControlProperty>> _typePropertiesWithAppliersCache = new Dictionary<Type, List<ControlProperty>>();
        private static Dictionary<Type, List<ControlProperty>> _templatePropertiesCache = new Dictionary<Type, List<ControlProperty>>();
        private static Object _lockObject = new Object();

        private ControlProperty()
        {

        }

        public Type OwnerType { get; private set; }
        public Type PropertyType { get; private set; }
        public String Name { get; private set; }
        public ControlPropertyChangedCallback PropertyChangedCallback { get; private set; }
        public Object DefaultValue { get; private set; }
        public bool TrackingPropertyChangedEvent { get; private set; }
        public ControlPropertyMetadata Metadata { get; set; }
        public IControlPropertyApplier Applier { get; set; }
        public String PropertyTypeName { get { return PropertyType.Name; } }
        public String OwnerTypeName { get { return OwnerType.Name; } }
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Register a property for a Control.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyType"></param>
        /// <param name="ownerType"></param>
        /// <param name="propertyMetadata"></param>
        /// <returns></returns>
        public static ControlProperty Register(String propertyName, Type propertyType, Type ownerType, ControlPropertyMetadata propertyMetadata = null)
        {
            Assert.NullArgument(propertyType, "propertyType");
            Assert.NullArgument(ownerType, "ownerType");

            lock (_lockObject)
            {
                if (!typeof(HtmlObject).IsAssignableFrom(ownerType))
                    throw new Exception(String.Format("The type {0} must inherit from HtmlObject in order to own a ControlProperty", ownerType.FullName));

                var result = new ControlProperty()
                {
                    Name = propertyName,
                    OwnerType = ownerType,
                    PropertyType = propertyType,
                    Metadata = propertyMetadata
                };

                if (result.Metadata != null)
                {
                    result.PropertyChangedCallback = result.Metadata.PropertyChangedCallback;
                    result.Applier = result.Metadata.Applier;
                    result.IsReadOnly = result.Metadata.ReadOnly;
                    if (result.Metadata.DefaultValue != null)
                        result.DefaultValue = result.Metadata.DefaultValue;
                    else
                        result.DefaultValue = ConversionHelper.Default(propertyType);
                }

                if (propertyType.GetInterfaces().Contains(typeof(INotifyPropertyChanged)))
                    result.TrackingPropertyChangedEvent = true;

                _properties.Add(result);
                _properties = _properties.OrderBy(p => p.Name).ToList();
                _typePropertiesCache.Clear();
                _templatePropertiesCache.Clear();
                _typePropertiesWithAppliersCache.Clear();
                return result;
            }
        }

        public static ControlProperty Register(String propertyName, Type propertyType, Type ownerType, IControlPropertyApplier applier,
            Object defaultValue = null, ControlPropertyChangedCallback propertyChangedCallback = null)
        {
            return Register(propertyName, propertyType, ownerType, new ControlPropertyMetadata(propertyChangedCallback, defaultValue, applier));
        }

        public static ControlProperty Register(String propertyName, Type propertyType, Type ownerType, Object defaultValue, ControlPropertyChangedCallback propertyChangedCallback = null)
        {
            return Register(propertyName, propertyType, ownerType, null, defaultValue, propertyChangedCallback);
        }

        internal static List<ControlProperty> GetTemplatedProperties(Type controlType)
        {
            List<ControlProperty> templateProperties = null;
            if (!_templatePropertiesCache.TryGetValue(controlType, out templateProperties))
            {
                templateProperties = new List<ControlProperty>();
                var fields = controlType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                foreach (var field in fields)
                {
                    if (ReflectionHelper.MemberHasAttribute<TemplatePartAttribute>(field))
                        templateProperties.Add((ControlProperty)field.GetValue(null));
                }
                _templatePropertiesCache[controlType] = templateProperties;
            }
            return templateProperties;
        }

        internal void NotifyPropertyChanged(HtmlObject htmlObject, object oldValue, object newValue)
        {
            if (this.PropertyChangedCallback != null)
            {
                var args = new ControlPropertyChangedEventArgs(htmlObject, this, oldValue, newValue);
                this.PropertyChangedCallback.Invoke(args);
            }
        }

        internal static IEnumerable<ControlProperty> GetPropertiesForType(Type requestedType)
        {
            List<ControlProperty> result = null;
            if (!_typePropertiesCache.TryGetValue(requestedType, out result))
            {
                // forçar o registro das propriedades
                requestedType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                result = new List<ControlProperty>();
                var type = requestedType;
                while (true)
                {

                    result.AddRange(_properties.Where(p => p.OwnerType == type));
                    if (type == typeof(HtmlObject))
                        break;
                    else
                        type = type.BaseType;
                }
                _typePropertiesCache[requestedType] = result;
            }
            return result;
        }

        internal static IEnumerable<ControlProperty> GetPropertiesWithAppliersForType(Type requestedType)
        {
            List<ControlProperty> result = null;
            if (!_typePropertiesWithAppliersCache.TryGetValue(requestedType, out result))
            {
                result = GetPropertiesForType(requestedType).Where(p => p.Applier != null).ToList();
                _typePropertiesWithAppliersCache[requestedType] = result;
            }
            return result;
        }
    }
}
