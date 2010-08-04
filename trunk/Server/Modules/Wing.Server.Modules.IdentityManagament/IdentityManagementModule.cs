using Wing.Modularity;
using Wing.Server.Sdk;
using Wing.ServiceLocation;
using Wing.Services.IdentityManagerService;

namespace Wing.Server.Modules.IdentityManagament
{
    [Module("IdentityManager")]
    [ModuleDependency("ServerStorage")]
    [ModuleCategory(ModuleCategory.Core)]
    [ModuleDescription("Gerenciador de indetidades de usuários")]
    [ModulePriority(ModulePriority.High)]
    public class IdentityManagamentModule : ModuleBase
    {
        public override void Initialize()
        {
            var serverStore = ServiceLocator.Current.GetInstance<IServerEntityStoreService>();

            // registrar as entidades para persistencia
            serverStore.RegisterEntity<UserEntity>();
            serverStore.RegisterEntity<RoleEntity>();

            // registrar o servico de identidade

            CheckSystemUser();
        }

        private void CheckSystemUser()
        {
            var store = ServiceLocator.Current.GetInstance<IServerEntityStoreService>();
            var criteria = store.CreateQuery<UserEntity>();
            criteria.AddFilterEqual("UserName", "system");
            var existing = criteria.FindFirst();
            if (existing == null)
            {
                existing = new UserEntity();
                existing.UserName = "system";
                existing.Password = "system";
                existing.Name = "Usuário de sistema";

                store.Save(existing);
            }
        }
    }
}
