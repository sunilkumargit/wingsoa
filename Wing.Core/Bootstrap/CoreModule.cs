using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Modularity;
using Wing.Logging;
using System.IO;
using Wing.Security;
using Wing.Pipeline;
using Wing.Client;

namespace Wing.Bootstrap
{
    [ModuleCategory(ModuleCategory.Core)]
    [ModulePriority(ModulePriority.Higher)]
    [ModuleDescription("Módulo de inicialização da aplicação composta.")]
    public class CoreModule : ModuleBase
    {
        protected override void RunInternal()
        {
            CheckSystemUser();
            AddHeartBeatPipelineOperation();
            AddBroadcastPipelineOperation();
            AddEchoPipelineOperation();

            ServiceLocator.GetInstance<ILogManager>()
                .GetSystemLogger()
                .Log("Core module inicialized", Category.Info, Priority.None);
        }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            // mapear os recursos de cliente
            var mapping = ServiceLocator.GetInstance<IResourceMapService>();

            // plugins do jquery
            mapping.MapResource("jquery.cookie", ResourceType.Script, "/content/js/jquery.cookie.js", ResourceLoadMode.Plugin);
            mapping.MapResource("jquery.alerts", ResourceType.Script, "/content/js/jquery.alerts.js", ResourceLoadMode.Plugin);

            // extensões do wui
            mapping.MapResource("wui.json", ResourceType.Script, "/content/js/wui.json.js", ResourceLoadMode.Plugin);
            mapping.MapResource("wui.net", ResourceType.Script, "/content/js/wui.net.js", ResourceLoadMode.Plugin);
            mapping.MapResource("wui.enum", ResourceType.Script, "/content/js/wui.enum.js", ResourceLoadMode.Plugin);
            mapping.MapResource("wui.controls.css", ResourceType.Style, "/content/css/wing.controls.css", ResourceLoadMode.Plugin);
            mapping.MapResource("wui.controls", ResourceType.Script, "/content/js/wui.controls.js", ResourceLoadMode.Plugin);


            // client do shell
            mapping.MapResource("wing.shell.client", ResourceType.Script, "/content/js/wing.shell.client.js", ResourceLoadMode.GlobalAddin);

            // shell
            mapping.MapResource("wui.shell.css", ResourceType.Style, "/content/css/wing.shell.css", ResourceLoadMode.ShellAddin);

            // users
            mapping.MapResource("users.module", ResourceType.Script, "/content/modules/users/js/users.module.js", ResourceLoadMode.ShellAddin);
        }

        private void CheckSystemUser()
        {
            var accountService = ServiceLocator.GetInstance<IAccountService>();

            var schema = accountService.GetSchema("system");

            if (schema == null)
                schema = accountService.CreateSchema("system", "Sistema");

            //depois o role
            var role = accountService.GetRole("Administrators");
            if (role == null)
                role = accountService.CreateRole("Administrators", "system");

            if (!role.IsInSchema(schema))
                schema.AddRole(role);

            var sysUser = accountService.GetUser("system");
            if (sysUser == null)
                sysUser = accountService.CreateUser(schema, "system", "Administrador", "marcelo@mdzn.net");

            if (!sysUser.IsInRole(role))
                role.AddUser(sysUser);
        }

        private void AddHeartBeatPipelineOperation()
        {
            var pipelineManager = ServiceLocator.GetInstance<IPipelineManager>();
            pipelineManager.AddOperation<DateTime, DateTime>("sys_hb", (date, ctx) =>
            {
                ctx.Session.SendMessage("sys_heart_beat_ok", true);
                return date;
            });
        }

        private void AddBroadcastPipelineOperation()
        {
            var pipelineManager = ServiceLocator.GetInstance<IPipelineManager>();
            pipelineManager.AddOperation<PipepelineBroadcastParams, String>("sys_broadcast", (param, ctx) =>
            {
                pipelineManager.Broadcast(param.MessageID, param.Data);
                return param.MessageID;
            });
        }

        private void AddEchoPipelineOperation()
        {
            var pipelineManager = ServiceLocator.GetInstance<IPipelineManager>();
            pipelineManager.AddOperation<String, String>("sys_echo", (param, ctx) =>
            {
                ctx.Session.SendMessage("sys_echo_result", param);
                return param;
            });
        }
    }

    public class PipepelineBroadcastParams
    {
        public string MessageID { get; set; }
        public Object Data { get; set; }
    }
}
