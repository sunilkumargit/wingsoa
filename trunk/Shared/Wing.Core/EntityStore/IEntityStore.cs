using System;

namespace Wing.EntityStore
{
    public interface IEntityStore
    {
        void RegisterEntity(Type type);
        void RegisterEntity<TEntityType>() where TEntityType : IStoreEntity;
        Object Get(Type entityType, Guid entityId);
        TEntityType Get<TEntityType>(Guid entityId) where TEntityType : IStoreEntity;
        IEntityStoreQuery CreateQuery(Type entityType);
        IEntityStoreQuery<TEntityType> CreateQuery<TEntityType>() where TEntityType : IStoreEntity;
        bool Save(params IStoreEntity[] entity);
        bool Remove(params IStoreEntity[] entity);
    }
}
