using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlServerCe;
using Wing.Server.Sdk;
using Wing.EntityStore;
using NHibernate.Cfg;
using Wing.ServiceLocation;
using Wing.Logging;
using NHibernate;
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
        private const string _hbPropertyTemplate = "<property name=\"{0}\" type=\"{1}\"><column name=\"{0}\" {2}/></property>";

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

                var conn = new SqlCeConnection(ConnectionString);

                _sessionFactory = _cfg.BuildSessionFactory();

                var dialect = NHibernate.Dialect.Dialect.GetDialect(_cfg.Properties);
                if (dialect != null)
                {
                    var _session = _sessionFactory.OpenSession();
                    try
                    {
                        var commands = _cfg.GenerateSchemaUpdateScript(dialect, new NHibernate.Tool.hbm2ddl.DatabaseMetadata(
                            (System.Data.Common.DbConnection)_session.Connection, dialect));

                        foreach (var cmd in commands)
                        {
                            var cmdQuery = _session.CreateSQLQuery(cmd);
                            cmdQuery.ExecuteUpdate();
                        }
                    }
                    finally { _session.Close(); }
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
                _sessionFactory.Dispose();
            _cfg = null;
        }

        private void ExecuteSession(Action<ISession> action)
        {
            Sync(() =>
            {
                if (_sessionFactory == null)
                    BuildSectionFactory();

                var session = _sessionFactory.OpenSession();
                try
                {
                    action(session);
                }
                finally
                {
                    session.Flush();
                    session.Close();
                }
            });
        }

        private string GetHBMappingXml(StoreEntityTypeMetadata entityMeta)
        {
            var properties = new StringBuilder();
            foreach (var propertyMeta in entityMeta.Properties)
            {
                var xmlStr = String.Format(_hbPropertyTemplate, propertyMeta.PropertyName, propertyMeta.PropertyType.FullName,
                    propertyMeta.PropertyType == typeof(String) ? "length=" + propertyMeta.MaxLength.ToString().Quoted() : "");
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

        public object Get(Type entityType, Guid entityId)
        {
            throw new NotImplementedException();
        }

        public TEntityType Get<TEntityType>(Guid entityId) where TEntityType : IStoreEntity
        {
            throw new NotImplementedException();
        }

        public IEntityStoreCriteria CreateCriteria(Type entityType)
        {
            throw new NotImplementedException();
        }

        public IEntityStoreCriteria CreateCriteria<TEntityType>() where TEntityType : IStoreEntity
        {
            throw new NotImplementedException();
        }

        public List<object> Find(object entityType, IEntityStoreCriteria criteria, int maxResults)
        {
            throw new NotImplementedException();
        }

        public List<object> Find(object entityType, IEntityStoreCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public List<TEntityType> Find<TEntityType>(IEntityStoreCriteria criteria, int maxResults) where TEntityType : IStoreEntity
        {
            throw new NotImplementedException();
        }

        public List<TEntityType> Find<TEntityType>(IEntityStoreCriteria criteria) where TEntityType : IStoreEntity
        {
            throw new NotImplementedException();
        }

        public object FindFirst(object entityType, IEntityStoreCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public TEntityType FindFirst<TEntityType>(IEntityStoreCriteria criteria) where TEntityType : IStoreEntity
        {
            throw new NotImplementedException();
        }

        public bool Save(params IStoreEntity[] entity)
        {
            ExecuteSession((session) =>
            {
                session.Save(entity);
            });
            return true;
        }

        public bool Remove(params IStoreEntity[] entity)
        {
            throw new NotImplementedException();
        }
    }
}
