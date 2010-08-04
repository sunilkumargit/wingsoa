using System;
using System.Collections;
using System.Collections.Generic;
using Wing.Utils;

namespace Wing.EntityStore
{
    public abstract class AbstractEntityStoreQuery : IEntityStoreQuery
    {
        protected AbstractEntityStoreQuery(Type forType)
        {
            Assert.NullArgument(forType, "forType");
            ForType = forType;
            Filters = new List<IEntityStoreFilter>();
            Orders = new List<IEntityStoreOrder>();
        }

        public Type ForType { get; private set; }

        public virtual IEntityStoreQuery Add(IEntityStoreFilter filter)
        {
            Filters.Add(filter);
            return this;
        }

        public virtual IEntityStoreQuery Add(IEntityStoreOrder order)
        {
            Orders.Add(order);
            return this;
        }

        public List<IEntityStoreFilter> Filters { get; private set; }

        public List<IEntityStoreOrder> Orders { get; private set; }

        public abstract IList FindObject(int maxResults);
        public abstract IList FindObject();
        public abstract Object FindFirstObject();

        public virtual IEntityStoreQuery AddFilterEqual(string propertyName, object value)
        {
            return Add(new DefaultStoreFilter(propertyName, ComparisonType.Equals, value));
        }

        public virtual IEntityStoreQuery AddFilterEqualOrGreater(string propertyName, object value)
        {
            return Add(new DefaultStoreFilter(propertyName, ComparisonType.EqualOrGreater, value));
        }

        public virtual IEntityStoreQuery AddFilterEqualOrLess(string propertyName, object value)
        {
            return Add(new DefaultStoreFilter(propertyName, ComparisonType.EqualOrLess, value));
        }

        public virtual IEntityStoreQuery AddFilterGreaterThan(string propertyName, object value)
        {
            return Add(new DefaultStoreFilter(propertyName, ComparisonType.GreaterThan, value));
        }

        public virtual IEntityStoreQuery AddFilterLessThan(string propertyName, object value)
        {
            return Add(new DefaultStoreFilter(propertyName, ComparisonType.LessThan, value));
        }

        public virtual IEntityStoreQuery AddFilterNull(string propertyName, object value)
        {
            return Add(new DefaultStoreFilter(propertyName, ComparisonType.IsNull, value));
        }

        public virtual IEntityStoreQuery AddFilterNotNull(string propertyName, object value)
        {
            return Add(new DefaultStoreFilter(propertyName, ComparisonType.IsNotNull, value));
        }

        public virtual IEntityStoreQuery AddFilterNotEqual(string propertyName, object value)
        {
            return Add(new DefaultStoreFilter(propertyName, ComparisonType.NotEqual, value));
        }

        public virtual IEntityStoreQuery AddOrderAsc(string propertyName)
        {
            return Add(new DefaultStoreOrder(propertyName, false));
        }

        public virtual IEntityStoreQuery AddOrderDesc(string propertyName)
        {
            return Add(new DefaultStoreOrder(propertyName, true));
        }
    }
}
