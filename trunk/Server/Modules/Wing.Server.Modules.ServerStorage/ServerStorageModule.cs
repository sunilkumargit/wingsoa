using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Modularity;
using Wing.ServiceLocation;
using Wing.Server.Core;
using System.IO;
using System.Data.SqlServerCe;
using Wing.Server.Sdk;
using Wing.EntityStore;

namespace Wing.Server.Modules.ServerStorage
{
    [Module("ServerStorage")]
    [ModuleCategory(ModuleCategory.Core)]
    [ModuleDescription("Controlador do armazenamento de dados do servidor")]
    [ModulePriority(ModulePriority.High)]
    public class ServerStorageModule : IModule
    {
        public void Initialize()
        {
            var bootSettings = ServiceLocator.Current.GetInstance<BootstrapSettings>();
            var dbPath = Path.Combine(bootSettings.ServerDataBasePath, "ServerData", "server.sdf");

            var serverStorage = new SqlCeServerStorageService(dbPath);
            serverStorage.RegisterEntity<ServerStoreTrace>();

            ServiceLocator.Current.Register<IServerEntityStoreService>(serverStorage);

            serverStorage.Save(new ServerStoreTrace() { Date = DateTime.Now, DBPath = dbPath });
        }

        public void Initialized()
        {
           //
        }
    }

    public class ServerStoreTrace : StoreEntityBase
    {
        [PersistentMember]
        public DateTime Date { get; set; }

        [PersistentMember(maxLength: 512)]
        public String DBPath { get; set; }
    }
}
