using System;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
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
            StartWingServer();
        }

        private void StartWingServer()
        {
            var settings = new BootstrapSettings();
            var basePath = Server.MapPath("~");
            settings.ServerAssemblyStorePath = Path.GetFullPath(Path.Combine(basePath, ConfigurationManager.AppSettings["ServerAssemblyStorePath"]));
            settings.ClientAssemblyStorePath = Path.GetFullPath(Path.Combine(basePath, ConfigurationManager.AppSettings["ClientAssemblyStorePath"]));
            settings.ServerDataBasePath = Path.GetFullPath(Path.Combine(basePath, ConfigurationManager.AppSettings["ServerDataBasePath"]));
            settings.ServerDataStorePath = Path.GetFullPath(Path.Combine(basePath, ConfigurationManager.AppSettings["ServerFileStorePath"]));
            settings.UserDataStorePath = Path.GetFullPath(Path.Combine(basePath, ConfigurationManager.AppSettings["UserDataPath"]));

            PrepareServerAssemblyStore(settings);
            CreateServerBootstrapperAndRun(settings);
        }

        private void PrepareServerAssemblyStore(BootstrapSettings settings)
        {
            var assemblyStore = new FileSystemAssemblyStore();
            assemblyStore.SetBasePath(settings.ServerAssemblyStorePath);
            assemblyStore.ConsolidateStore();
        }

        private void CreateServerBootstrapperAndRun(BootstrapSettings settings)
        {
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