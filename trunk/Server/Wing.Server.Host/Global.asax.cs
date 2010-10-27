using System;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using Wing.Server.Core;
using System.Web;

namespace Wing.Server.Host
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication, IBootLogger
    {
        private string BootLogFilePath = "";
        private BootstrapSettings _settings;
        private IBootstrapper _bootstrapperInstance;

        public static String MapUrl(String url)
        {
            return UrlHelper.GenerateContentUrl(url, HttpContext.Current.Request.RequestContext.HttpContext);
        }

        public void Log(String message, Exception ex = null)
        {
            try
            {
                File.AppendAllText(BootLogFilePath, String.Format("{0}{1}: {2}", Environment.NewLine, DateTime.Now.ToString(), message)
                    + (ex == null ? "" : String.Format("{0}     Exception: {1}", Environment.NewLine, ex.ToString())));
            }
            catch { }
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",                                       // Route name
                "{controller}/{action}.actn",                    // URL with parameters
                new { controller = "Loader", action = "Index" }  // Parameter defaults
            );

        }

        private bool SafeExec(String message, Action action)
        {
            var result = true;
            Log(message);
            try
            {
                action();
            }
            catch (Exception ex)
            {
                result = false;
                Log("ERROR: ", ex);
            }
            return result;
        }

        protected void Application_Start()
        {
            BootLogFilePath = Path.Combine(Server.MapPath("~"), ConfigurationManager.AppSettings["BootLogFilePath"]);
            if (!File.Exists(BootLogFilePath))
                File.WriteAllText(BootLogFilePath, String.Format("File Created: {0} \n Path: {1}", DateTime.Now.ToString(), BootLogFilePath));
            File.AppendAllText(BootLogFilePath, Environment.NewLine + Environment.NewLine + "-------------------------------------------------------------------------------------");
            Log("Server starting");
            if (SafeExec("Registering routes", () => RegisterRoutes(RouteTable.Routes)))
            {
                if (SafeExec("Configuring server settings", () => CreateSettings()))
                {
                    if (SafeExec("Preparing server assembly store", () => PrepareServerAssemblyStore()))
                    {
                        if (SafeExec("Creating bootstrapper", () => PrepareBootstrapper()))
                        {
                            SafeExec("Running bootstrapper", () => _bootstrapperInstance.Run(_settings));
                        }
                    }
                }
            }
        }

        private void CreateSettings()
        {
            _settings = new BootstrapSettings();
            _settings.BootLogger = this;
            var basePath = Server.MapPath("~");
            _settings.ServerAssemblyStorePath = Path.GetFullPath(Path.Combine(basePath, ConfigurationManager.AppSettings["ServerAssemblyStorePath"]));
            _settings.ClientAssemblyStorePath = Path.GetFullPath(Path.Combine(basePath, ConfigurationManager.AppSettings["ClientAssemblyStorePath"]));
            _settings.ServerDataBasePath = Path.GetFullPath(Path.Combine(basePath, ConfigurationManager.AppSettings["ServerDataBasePath"]));
            _settings.ServerDataStorePath = Path.GetFullPath(Path.Combine(basePath, ConfigurationManager.AppSettings["ServerFileStorePath"]));
            _settings.UserDataStorePath = Path.GetFullPath(Path.Combine(basePath, ConfigurationManager.AppSettings["UserDataPath"]));
        }

        private void PrepareServerAssemblyStore()
        {
            var assemblyStore = new FileSystemAssemblyStore();
            assemblyStore.SetBasePath(_settings.ServerAssemblyStorePath);
            Log("Consolidating store");
            assemblyStore.ConsolidateStore();
            Log("Server store consolidated succesfully");
        }

        private void PrepareBootstrapper()
        {
            var bootstrapAssembly = ConfigurationManager.AppSettings["ServerBootstrapperFile"];
            //criar um AssemblyResolver
            Log("Loading bootstrapper assembly: " + bootstrapAssembly);
            var resolver = new StoreAssemblyResolver(_settings.ServerAssemblyStorePath, null);
            resolver.LoadAssemblyFrom(bootstrapAssembly);
            AppDomain.CurrentDomain.Load(bootstrapAssembly);
            var bootstrapperType = ConfigurationManager.AppSettings["ServerBootstrapperType"];
            Log("Loading bootstrapper type: " + bootstrapperType);
            var bootstrapper = Type.GetType(bootstrapperType);
            resolver.Dispose();
            Log("Creating bootstrapper instance from type " + bootstrapper.AssemblyQualifiedName);
            _bootstrapperInstance = (IBootstrapper)Activator.CreateInstance(bootstrapper);
            Log("Bootstrapper instance created successfully");
        }

    }
}