using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.EntityStore
{
    public interface IEntityStore
    {
        void RegisterEntity(Type type);
        void RegisterEntity<TEntityType>() where TEntityType : IStoreEntity;
        Object Get(Type entityType, Guid entityId);
        TEntityType Get<TEntityType>(Guid entityId) where TEntityType : IStoreEntity;
        IEntityStoreCriteria CreateCriteria(Type entityType);
        IEntityStoreCriteria CreateCriteria<TEntityType>() where TEntityType : IStoreEntity;
        List<Object> Find(Object entityType, IEntityStoreCriteria criteria, int maxResults);
        List<Object> Find(Object entityType, IEntityStoreCriteria criteria);
        List<TEntityType> Find<TEntityType>(IEntityStoreCriteria criteria, int maxResults) where TEntityType : IStoreEntity;
        List<TEntityType> Find<TEntityType>(IEntityStoreCriteria criteria) where TEntityType : IStoreEntity;
        Object FindFirst(Object entityType, IEntityStoreCriteria criteria);
        TEntityType FindFirst<TEntityType>(IEntityStoreCriteria criteria) where TEntityType : IStoreEntity;
        bool Save(params IStoreEntity[] entity);
        bool Remove(params IStoreEntity[] entity);
    }
}
