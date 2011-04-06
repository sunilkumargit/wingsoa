using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Modularity;
using Wing.Pipeline;
using Wing.Client;
using Wing.Security;

namespace Wing.Modules.ChatServer
{
    [Module("Chat server")]
    [ModulePriority(ModulePriority.Normal)]
    [ModuleCategory(ModuleCategory.Common)]
    [ModuleDescription("Servidor de chat empresarial")]
    public class ChatModule : ModuleBase
    {
        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            ServiceLocator.Register<IChatContactListProvider, ChatContactListProvider>(true);
            ServiceLocator.Register<IChatServer, ChatServerImpl>(true);

            // recursos do cliente
            var resources = ServiceLocator.GetInstance<IResourceMapService>();

            // on demand
            resources.MapResource("chat.client", ResourceType.Script, "/content/modules/chat/js/chat.client.js", ResourceLoadMode.OnDemand);
            resources.MapResource("chat.style", ResourceType.Style, "/content/modules/chat/css/chat.css", ResourceLoadMode.OnDemand);

            // inicializa o chat no shell
            resources.MapResource("chat.loader", ResourceType.Script, "/content/modules/chat/js/chat.loader.js", ResourceLoadMode.ShellAddin);

            // configurar o esquema de segurança do chat 
            var authService = ServiceLocator.GetInstance<IAuthorizationService>();
            authService.CreateKey("/Chat", "Comunicador", 0);
        }
    }
}
