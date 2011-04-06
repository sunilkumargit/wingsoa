using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using System.IO;
using Wing.Bootstrap;
using Wing.Mvc.Binders;

namespace Wing.Host
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication, IBootLogger, IPathMapper
    {
        private string _bootLogFilePath;
        private Assembly _wingCoreAssembly;
        private IBootstrapper _bootstrapper;
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Redir", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        private Object __logLockObject = new Object();
        public void Log(String message, Exception ex = null)
        {
            try
            {
                lock (__logLockObject)
                {
                    File.AppendAllText(_bootLogFilePath, String.Format("{0}{1}: {2}", Environment.NewLine, DateTime.Now.ToString(), message)
                        + (ex == null ? "" : String.Format("{0}     Exception: {1}", Environment.NewLine, ex.ToString())));
                }
            }
            catch { }
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

#if DEBUG
        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            this.Log("Resolving assembly " + args.Name);
            return null;
        }
#endif

        protected void Application_Start()
        {
#if DEBUG
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
#endif

            _bootLogFilePath = Server.MapPath("~/App_Data/Server/boot.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(_bootLogFilePath));
            if (!File.Exists(_bootLogFilePath))
                File.WriteAllText(_bootLogFilePath, String.Format("File Created: {0} \n Path: {1}", DateTime.Now.ToString(), _bootLogFilePath));
            File.AppendAllText(_bootLogFilePath, "\r\n\r\n\r\n");
            Log("Server starting");
            File.AppendAllText(_bootLogFilePath, Environment.NewLine + Environment.NewLine + "-------------------------------------------------------------------------------------");

            //load wing core
            if (SafeExec("Carregando Wing.Core.dll", () => _wingCoreAssembly = Assembly.LoadFrom(MapPath("~/bin/Wing.Core.dll"))))
            {
                //create Bootstrapper instance
                if (SafeExec("Instanciando o bootstrapper", () => CreateBootstrapperInstance()))
                {
                    Log("Bootstrapper criado, iniciando o servidor...");
                    _bootstrapper.Run(this, this);
                }
            }


            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
            ModelBinders.Binders.DefaultBinder = new DefaultJSONModelBinder();
        }

        private void CreateBootstrapperInstance()
        {
            var type = _wingCoreAssembly.GetType("Wing.Bootstrap.Bootstrapper");
            _bootstrapper = (IBootstrapper)Activator.CreateInstance(type);
        }

        public string MapPath(string relativePath)
        {
            return Server.MapPath(relativePath);
        }
    }
}