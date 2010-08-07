﻿using Wing.Modularity;
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

            //verificar o usuário system
            CheckSystemUser();
        }

        private void CheckSystemUser()
        {
            var store = ServiceLocator.Current.GetInstance<IServerEntityStoreService>();
            var criteria = store.CreateQuery<UserEntity>();
            criteria.AddFilterEqual("UserName", "System");
            var existing = criteria.FindFirst();
            if (existing == null)
            {
                existing = new UserEntity();
                existing.UserName = "System";
                existing.Password = "System";
                existing.Name = "Usuário de sistema";

                store.Save(existing);
            }
        }
    }
}
