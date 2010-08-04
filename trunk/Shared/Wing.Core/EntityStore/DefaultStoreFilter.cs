using System;

namespace Wing.EntityStore
{
    public class DefaultStoreFilter : IEntityStoreFilter
    {
        public DefaultStoreFilter(String propertyName, ComparisonType comparison, Object value)
        {
            this.PropertyName = propertyName;
            this.Comparison = comparison;
            this.Value = value;
        }

        public string PropertyName { get; set; }
        public ComparisonType Comparison { get; set; }
        public object Value { get; set; }
    }
}
