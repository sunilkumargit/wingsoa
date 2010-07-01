using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Configuration;
using Wing.Server.Core;

namespace Wing.Server.Host
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",                                       // Route name
                "{controller}/{action}",                         // URL with parameters
                new { controller = "Loader", action = "Index" }  // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);

            PrepareServerAssemblyStore();
            CreateServerBootstrapper();
        }

        private void PrepareServerAssemblyStore()
        {
            var assemblyStore = new FileSystemAssemblyStore();
            assemblyStore.SetBasePath(Server.MapPath(ConfigurationManager.AppSettings["ServerAssemblyStorePath"]));
            assemblyStore.ConsolidateStore();
        }

        private void CreateServerBootstrapper()
        {
            var settings = new BootstrapSettings();
            settings.ServerAssemblyStorePath = Server.MapPath(ConfigurationManager.AppSettings["ServerAssemblyStorePath"]);
            settings.ClientAssemblyStorePath = Server.MapPath(ConfigurationManager.AppSettings["ClientAssemblyStorePath"]);

            //criar um AssemblyResolver
            var resolver = new StoreAssemblyResolver(settings.ServerAssemblyStorePath, null);
            var bootstrapAssembly = ConfigurationManager.AppSettings["ServerBootstrapperFile"];
            resolver.LoadAssemblyFrom(bootstrapAssembly);
            AppDomain.CurrentDomain.Load(bootstrapAssembly);
            var bootstrapper = Type.GetType(ConfigurationManager.AppSettings["ServerBootstrapperType"]);
            resolver.Dispose();

            IBootstrapper bootstrapperInstance = (IBootstrapper)Activator.CreateInstance(bootstrapper);

            bootstrapperInstance.Run(settings);
        }
    }
}