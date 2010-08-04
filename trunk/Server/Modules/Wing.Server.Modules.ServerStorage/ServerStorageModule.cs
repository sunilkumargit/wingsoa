using System;
using System.IO;
using Wing.Modularity;
using Wing.Server.Core;
using Wing.Server.Sdk;
using Wing.ServiceLocation;

namespace Wing.Server.Modules.ServerStorage
{
    [Module("ServerStorage")]
    [ModuleCategory(ModuleCategory.Core)]
    [ModuleDescription("Controlador do armazenamento de dados do servidor")]
    [ModulePriority(ModulePriority.High)]
    public class ServerStorageModule : ModuleBase
    {
        public override void Initialize()
        {
            var bootSettings = ServiceLocator.Current.GetInstance<BootstrapSettings>();
            var dbPath = Path.Combine(bootSettings.ServerDataBasePath, "ServerData", "server.sdf");

            var serverStorage = new SqlCeServerStorageService(dbPath);
            serverStorage.RegisterEntity<ServerStoreTraceEntity>();

            ServiceLocator.Current.Register<IServerEntityStoreService>(serverStorage);

            // salvar uma entidade aqui para forçar a primeira conexão com o banco de dados
            serverStorage.Save(new ServerStoreTraceEntity() { Date = DateTime.Now, DBPath = dbPath });
        }
    }
}