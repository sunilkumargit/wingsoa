using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Utils;

namespace Wing.EntityStore
{
    public class StoreEntityTypeMetadata
    {
        public Type EntityType { get; private set; }
        public List<StoreEntityPropertyMetadata> Properties { get; private set; }

        public StoreEntityTypeMetadata(Type type)
        {
            Properties = new List<StoreEntityPropertyMetadata>();
            BuildMetadata(type);
        }

        private void BuildMetadata(Type type)
        {
            EntityType = type;

            foreach (var property in ReflectionUtils.GetPropertiesWithAttribute<PersistentMemberAttribute>(type))
            {
                if ((!property.CanWrite || !property.CanRead) && !property.Name.Equals("InstanceId"))
                    throw new Exception(String.Format("A property with {0} attribute must permit read and write access", typeof(PersistentMemberAttribute).Name));

                var attr = ReflectionUtils.GetAttribute<PersistentMemberAttribute>(property);
                var propertyMetadata = new StoreEntityPropertyMetadata();
                propertyMetadata.PropertyName = property.Name;
                propertyMetadata.PropertyType = property.PropertyType;
                if (propertyMetadata.PropertyType == typeof(System.String))
                    propertyMetadata.MaxLength = attr.MaxLength == 0 ? 50 : attr.MaxLength;
                Properties.Add(propertyMetadata);
            }
        }
    }
}
