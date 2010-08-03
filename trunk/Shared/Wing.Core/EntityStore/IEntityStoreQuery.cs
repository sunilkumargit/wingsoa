using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Wing.EntityStore
{
    public interface IEntityStoreQuery
    {
        Type ForType { get; }
        IEntityStoreQuery Add(IEntityStoreFilter filter);
        IEntityStoreQuery Add(IEntityStoreOrder order);
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

    public interface IEntityStoreQuery<TEntityType> : IEntityStoreQuery where TEntityType : IStoreEntity
    {
        IList<TEntityType> Find();
        IList<TEntityType> Find(int maxResults);
        TEntityType FindFirst();
    }
}
