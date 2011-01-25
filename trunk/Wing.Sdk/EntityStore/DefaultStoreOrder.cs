using System;

namespace Wing.EntityStore
{
    public class DefaultStoreOrder : IEntityStoreOrder
    {
        public DefaultStoreOrder(String propertyName, bool desc)
        {
            this.PropertyName = propertyName;
            this.Desc = desc;
        }

        public string PropertyName { get; set; }
        public bool Desc { get; set; }
    }
}
