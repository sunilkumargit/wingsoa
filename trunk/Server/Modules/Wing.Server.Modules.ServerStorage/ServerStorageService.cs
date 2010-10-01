using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Tool.hbm2ddl;
using Wing.EntityStore;
using Wing.Logging;
using Wing.Server;
using Wing.ServiceLocation;
using Wing.Utils;

namespace Wing.Server.Modules.ServerStorage
{
    class SqlCeServerStorageService : IServerEntityStoreService
    {
        protected String DbPath;
        protected String ConnectionString;
        protected Configuration _cfg;
        protected ISessionFactory _sessionFactory;
        protected Dictionary<Type, StoreEntityTypeMetadata> _entities = new Dictionary<Type, StoreEntityTypeMetadata>();
        protected Dictionary<Type, String> _hbMappings = new Dictionary<Type, string>();

        private const string _hbXmlTemplate = "<hibernate-mapping xmlns=\"urn:nhibernate-mapping-2.2\" default-lazy=\"false\"><class name=\"{0}\" table=\"{1}Entity\" lazy=\"false\"><id name=\"InstanceId\" access=\"field.lowercase-underscore\" column=\"Id\" type=\"System.Guid\"><generator class=\"assigned\"></generator></id>{2}</class></hibernate-mapping>";
        private const string _hbPropertyTemplate = "<property name=\"{0}\" type=\"{1}\"><column name=\"{0}\" {2} {3}/></property>";

        public SqlCeServerStorageService(String dbPath)
        {
            DbPath = dbPath;
            ConnectionString = String.Format("Data Source={0}", dbPath);
            //habilitar o SqlCe para o ASP.NET
            AppDomain.CurrentDomain.SetData("SQLServerCompactEditionUnderWebHosting", true);
            CreateDbIfNotExists();
        }

        private Object _syncObject = new Object();
        private void Sync(Action action)
        {
            while (true)
            {
                if (System.Threading.Monitor.TryEnter(_syncObject, 100))
                    break;
            }
            try
            {
                action();
            }
            finally
            {
                System.Threading.Monitor.Exit(_syncObject);
            }
        }

        private void CreateDbIfNotExists()
        {
            Sync(() =>
            {
                if (File.Exists(DbPath))
                    return;

                var engine = new SqlCeEngine(ConnectionString);
                Directory.CreateDirectory(Path.GetDirectoryName(DbPath));
                engine.CreateDatabase();
                engine.Dispose();
            });
        }

        private void BuildSectionFactory()
        {
            try
            {
                InvalidateCurrentFactory();

                _cfg = new Configuration();
                _cfg.SetProperty("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
                _cfg.SetProperty("dialect", "NHibernate.Dialect.MsSqlCeDialect");
                _cfg.SetProperty("connection.driver_class", "NHibernate.Driver.SqlServerCeDriver");
                _cfg.SetProperty("query.substitutions", "true 1, false 0");
                _cfg.SetProperty("connection.connection_string", ConnectionString);
                _cfg.SetProperty("proxyfactory.factory_class", "NHibernate.ByteCode.LinFu.ProxyFactoryFactory, NHibernate.ByteCode.LinFu");
                _cfg.SetProperty("show_sql", "true");

                foreach (var entityMeta in _entities.Values)
                    _cfg.AddXmlString(GetHBMappingXml(entityMeta));

                _sessionFactory = _cfg.BuildSessionFactory();

                var dialect = NHibernate.Dialect.Dialect.GetDialect(_cfg.Properties);
                if (dialect != null)
                {
                    var connection = new SqlCeConnection(ConnectionString);
                    try
                    {
                        connection.Open();
                        var commands = _cfg.GenerateSchemaUpdateScript(dialect, new DatabaseMetadata(connection, dialect));

                        foreach (var cmd in commands)
                        {
                            var dbCmd = connection.CreateCommand();
                            dbCmd.CommandText = cmd;
                            dbCmd.ExecuteNonQuery();
                            dbCmd.Dispose();
                        }
                    }
                    finally
                    {
                        connection.Dispose();
                    }
                }
            }
            catch (System.Exception ex)
            {
                ServiceLocator.Current
                    .GetInstance<ILogger>()
                    .LogException("Falha ao iniciar o factory de sessions do hibernate", ex, Priority.High);
                throw;
            }
        }

        private void InvalidateCurrentFactory()
        {
            if (_sessionFactory != null)
            {
                _sessionFactory.Dispose();
                _sessionFactory = null;
            }
            _cfg = null;
        }

        private TResult Execute<TResult>(Func<ISession, TResult> action)
        {
            TResult result = default(TResult);
            Sync(() =>
            {
                if (_sessionFactory == null)
                    BuildSectionFactory();

                var session = _sessionFactory.OpenSession();
                try
                {
                    result = action(session);
                }
                finally
                {
                    session.Flush();
                    session.Close();
                }
            });
            return result;
        }

        private string GetHBMappingXml(StoreEntityTypeMetadata entityMeta)
        {
            var properties = new StringBuilder();
            foreach (var propertyMeta in entityMeta.Properties)
            {
                var isString = propertyMeta.PropertyType == typeof(String);
                var isMaxLength = isString && propertyMeta.MaxLength == Int32.MaxValue;

                var xmlStr = String.Format(_hbPropertyTemplate, propertyMeta.PropertyName, propertyMeta.PropertyType.FullName,
                    isString && !isMaxLength ? "length=" + propertyMeta.MaxLength.ToString().Quoted() : "",
                    isString && isMaxLength ? "sql-type=" + "ntext".Quoted() : "");
                properties.AppendLine(xmlStr);
            }
            return String.Format(_hbXmlTemplate, entityMeta.EntityType.AssemblyQualifiedName, entityMeta.EntityType.Name, properties.ToString());
        }

        public void RegisterEntity(Type type)
        {
            Sync(() =>
            {
                if (_entities.ContainsKey(type))
                    _entities.Remove(type);
                if (_hbMappings.ContainsKey(type))
                    _hbMappings.Remove(type);

                _entities[type] = new StoreEntityTypeMetadata(type);

                InvalidateCurrentFactory();
            });
        }

        public void RegisterEntity<TEntityType>() where TEntityType : IStoreEntity
        {
            RegisterEntity(typeof(TEntityType));
        }

        public bool Save(params IStoreEntity[] entity)
        {
            return Execute<bool>((session) =>
            {
                foreach (var instance in entity)
                    session.Save(instance);
                return true;
            });
        }

        public TEntityType Get<TEntityType>(Guid entityId) where TEntityType : IStoreEntity
        {
            return Execute<TEntityType>((session) =>
            {
                return session.Get<TEntityType>(entityId);
            });
        }

        public object Get(Type entityType, Guid entityId)
        {
            return Execute<Object>((session) =>
            {
                return session.Get(entityType, entityId);
            });
        }

        public IEntityStoreQuery CreateQuery(Type entityType)
        {
            return new NHibernateStoreQuery(entityType, this);
        }

        public IEntityStoreQuery<TEntityType> CreateQuery<TEntityType>() where TEntityType : IStoreEntity
        {
            return new NHibernateStoreQuery<TEntityType>(this);
        }

        public bool Remove(params IStoreEntity[] entity)
        {
            return Execute<bool>((session) =>
            {
                foreach (var instance in entity)
                    session.Delete(instance);
                return true;
            });
        }

        private class NHibernateStoreQuery : AbstractEntityStoreQuery
        {
            protected SqlCeServerStorageService _store;

            public NHibernateStoreQuery(Type forType, SqlCeServerStorageService store)
                : base(forType)
            {
                _store = store;
            }

            protected ICriteria CreateCriteria(ISession session, int maxResults)
            {
                var criteria = session.CreateCriteria(ForType);
                foreach (var filter in Filters)
                {
                    switch (filter.Comparison)
                    {
                        case ComparisonType.EqualOrGreater: criteria.Add(Restrictions.Ge(filter.PropertyName, filter.Value)); break;
                        case ComparisonType.EqualOrLess: criteria.Add(Restrictions.Le(filter.PropertyName, filter.Value)); break;
                        case ComparisonType.Equals: criteria.Add(Restrictions.Eq(filter.PropertyName, filter.Value)); break;
                        case ComparisonType.GreaterThan: criteria.Add(Restrictions.Gt(filter.PropertyName, filter.Value)); break;
                        case ComparisonType.IsNotNull: criteria.Add(Restrictions.IsNotNull(filter.PropertyName)); break;
                        case ComparisonType.IsNull: criteria.Add(Restrictions.IsNull(filter.PropertyName)); break;
                        case ComparisonType.LessThan: criteria.Add(Restrictions.Lt(filter.PropertyName, filter.Value)); break;
                        case ComparisonType.NotEqual: criteria.Add(Restrictions.Not(Restrictions.Eq(filter.PropertyName, filter.Value))); break;
                    }
                }
                foreach (var order in Orders)
                {
                    if (order.Desc)
                        criteria.AddOrder(Order.Desc(order.PropertyName));
                    else
                        criteria.AddOrder(Order.Asc(order.PropertyName));
                }
                if (maxResults > 0)
                    criteria.SetMaxResults(maxResults);
                return criteria;
            }

            public override IList FindObject(int maxResults)
            {
                return _store.Execute<IList>((session) => { return CreateCriteria(session, maxResults).List(); });
            }

            public override IList FindObject()
            {
                return FindObject(0);
            }

            public override object FindFirstObject()
            {
                return _store.Execute<Object>((session) => { return CreateCriteria(session, 1).UniqueResult(); });
            }
        }

        private class NHibernateStoreQuery<TEntityType> : NHibernateStoreQuery, IEntityStoreQuery<TEntityType> where TEntityType : IStoreEntity
        {
            public NHibernateStoreQuery(SqlCeServerStorageService store)
                : base(typeof(TEntityType), store) { }

            public IList<TEntityType> Find()
            {
                return Find(0);
            }

            public IList<TEntityType> Find(int maxResults)
            {
                return _store.Execute<IList<TEntityType>>((session) => { return CreateCriteria(session, maxResults).List<TEntityType>(); });
            }

            public TEntityType FindFirst()
            {
                return _store.Execute<TEntityType>((session) => { return CreateCriteria(session, 1).UniqueResult<TEntityType>(); });
            }
        }
    }
}
