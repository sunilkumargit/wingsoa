using System;

namespace Wing.EntityStore
{
    public interface IEntityStore
    {
        String Name { get; }
        void RegisterEntity(Type type);
        void RegisterEntity<TEntityType>() where TEntityType : StoreEntity;
        Object Get(Type entityType, Int32 entityId);
        TEntityType Get<TEntityType>(Int32 entityId) where TEntityType : StoreEntity;
        IEntityStoreQuery CreateQuery(Type entityType);
        IEntityStoreQuery<TEntityType> CreateQuery<TEntityType>() where TEntityType : StoreEntity;
        bool Save(params StoreEntity[] entity);
        bool Remove(params StoreEntity[] entity);
    }
}
