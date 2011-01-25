using System;
using System.Collections;
using System.Collections.Generic;

namespace Wing.EntityStore
{
    public interface IEntityStoreQuery
    {
        Type ForType { get; }
        IEntityStoreQuery AddFilter(IEntityStoreFilter filter);
        IEntityStoreQuery AddOrder(IEntityStoreOrder order);
        List<IEntityStoreFilter> Filters { get; }
        List<IEntityStoreOrder> Orders { get; }

        IList FindObject(int maxResults);
        IList FindObject();
        Object FindFirstObject();

        IEntityStoreQuery AddFilterEqual(String propertyName, Object value);
        IEntityStoreQuery AddFilterEqualOrGreater(String propertyName, Object value);
        IEntityStoreQuery AddFilterEqualOrLess(String propertyName, Object value);
        IEntityStoreQuery AddFilterGreaterThan(String propertyName, Object value);
        IEntityStoreQuery AddFilterLessThan(String propertyName, Object value);
        IEntityStoreQuery AddFilterNull(String propertyName, Object value);
        IEntityStoreQuery AddFilterNotNull(String propertyName, Object value);
        IEntityStoreQuery AddFilterNotEqual(String propertyName, Object value);
        IEntityStoreQuery AddOrderAsc(String propertyName);
        IEntityStoreQuery AddOrderDesc(String propertyName);
    }

    public interface IEntityStoreQuery<TEntityType> : IEntityStoreQuery where TEntityType : StoreEntity
    {
        List<TEntityType> Find();
        List<TEntityType> Find(int maxResults);
        TEntityType FindFirst();
    }
}
