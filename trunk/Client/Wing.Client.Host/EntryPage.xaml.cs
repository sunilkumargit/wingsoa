using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Wing.Utils;
using Wing.Client.Core;
using System.Reflection;

namespace Wing.Client
{
    public partial class EntryPage : UserControl
    {
        public EntryPage()
        {
            InitializeComponent();
            Helper.DelayExecution(TimeSpan.FromMilliseconds(500), () =>
            {
                if (App.Current.IsRunningOutOfBrowser)
                {
                    StatusText.Text = "Procurando atualizações...";
                    App.Current.CheckAndDownloadUpdateCompleted += new CheckAndDownloadUpdateCompletedEventHandler(Current_CheckAndDownloadUpdateCompleted);
                    App.Current.CheckAndDownloadUpdateAsync();
                }
                else
                    BeginBootstrap();

                return false;
            });
        }

        void Current_CheckAndDownloadUpdateCompleted(object sender, CheckAndDownloadUpdateCompletedEventArgs e)
        {
            if (e.UpdateAvailable)
            {
                MessageBox.Show("A aplicacação foi atualizada para uma nova versão. Por favor, reinicie o aplicativo.", "Atualização", MessageBoxButton.OK);
                StatusText.Text = "Aplicação atualizada. Reinicie por favor.";
            }
            else if (e.Error != null &&
               e.Error is PlatformNotSupportedException)
            {
                MessageBox.Show("Uma nova versão da aplicação está disponível, mas não é compatível com a versão do Silverlight instalada em seu computador. Por favor, visite o site do Silverlight e baixe uma nova versão do plug-in.", "Atualização", MessageBoxButton.OK);
            }
            else
            {
                BeginBootstrap();
            }
        }

        void BeginBootstrap()
        {

            var client = new WebClient();
            AssemblyInfoCollection assemblyInfo = null;
            client.BaseAddress = App.Current.Host.GetBaseUrl().ToString();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler((sender, e) =>
            {
                if (e.Cancelled)
                    StatusText.Text = "A operação foi cancelada. Reinicie por favor.";
                else if (e.Error != null)
                    StatusText.Text = "Ocorreu um erro: " + e.Error.Message;
                else
                {
                    StatusText.Text = "Baixando atualizações...";
                    assemblyInfo = AssemblyInfoCollection.DeserializeFromXml(e.Result);
                    DownloadAssembly(assemblyInfo.Assemblies, 0, null);
                }
            });
            client.DownloadStringAsync(new Uri("/WingCltAppSupport/GetAssembliesMetadata", UriKind.Relative));
        }

        void DownloadAssembly(List<AssemblyInfo> list, int nextIndex, ClientAssemblyStore store)
        {
            if (list.Count == 0 || list.Count <= nextIndex)
            {
                if (store != null)
                    store.Unload();
                Bootstrap(list);
                return;
            }
            store = store ?? new ClientAssemblyStore();
            var info = list[nextIndex];

            Helper.DelayExecution(TimeSpan.FromMilliseconds(500), () =>
            {
                StatusText.Text = String.Format("Baixando atualização {0} de {1}", nextIndex + 1, list.Count);
                var client = new WebClient();
                client.OpenReadCompleted += new OpenReadCompletedEventHandler((sender, e) =>
                {
                    var data = new byte[e.Result.Length];
                    e.Result.Read(data, 0, data.Length);
                    store.AddAssembly(info.AssemblyName, data);
                    DownloadAssembly(list, nextIndex + 1, store);
                });
                var uri = new Uri(App.Current.Host.GetBaseUrl(), "/WingCltAppSupport/GetAssemblyData?file=" + info.AssemblyName);
                client.OpenReadAsync(uri, null);
                return false;
            });
        }


        void Bootstrap(List<AssemblyInfo> list)
        {
            StatusText.Text = "Iniciando o sistema...";

            var assemblies = LoadAssemblies(list);
            var bootstrapper = CreateBootstrapper(assemblies);

            var settings = new BootstrapSettings();
            settings.ServerBaseAddress = "";
            settings.SoaEndpointAddressServiceUri = "";

            bootstrapper.Run(settings);
        }

        List<Assembly> LoadAssemblies(List<AssemblyInfo> list)
        {
            //criar um resolver para o app domain
            var assemblies = new List<Assembly>();
            var store = new ClientAssemblyStore();
            foreach (var asmInfo in list)
            {
                var stream = new System.IO.MemoryStream(store.GetAssemblyData(asmInfo.AssemblyName));
                var part = new AssemblyPart();
                assemblies.Add(part.Load(stream));
                stream.Close();
                stream.Dispose();
            }
            return assemblies;
        }

        IBootstrapper CreateBootstrapper(List<Assembly> assemblies)
        {
            Type bootstrapperType = null;
            foreach (var assembly in assemblies)
            {
                bootstrapperType = assembly.GetExportedTypes().FirstOrDefault(type =>
                    typeof(IBootstrapper).IsAssignableFrom(type)
                    && !type.IsAbstract
                    && !type.IsInterface
                    && !type.IsGenericType);
                if (bootstrapperType != null)
                    break;
            }
            return (IBootstrapper)Activator.CreateInstance(bootstrapperType);
        }
    }
}



