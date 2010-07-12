using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Net;
using Wing.Utils;

namespace Wing.Client.Core
{
    public class ApplicationStarter
    {
        private ISplashUI _splash;
        private List<Action> _startActions = new List<Action>();
        private AssemblyInfoCollection _assemblyInfo;
        private ClientAssemblyStore _store;
        private BootstrapSettings _bootstrapSettings;
        private IRootVisualManager _rootVisualManager;

        public ApplicationStarter(ISplashUI splashUI, IRootVisualManager rootVisualManager)
        {
            if (splashUI == null)
                throw new ArgumentNullException("splashUI");
            if (rootVisualManager == null)
                throw new ArgumentNullException("rootVisualManager");
            _splash = splashUI;
            _startActions = new List<Action>();
            _rootVisualManager = rootVisualManager;
        }

        private Application CurrentApp { get { return Application.Current; } }

        public void Run()
        {
            _store = new ClientAssemblyStore();

            _bootstrapSettings = new BootstrapSettings()
            {
                ServerBaseAddress = CurrentApp.Host.GetBaseUrl(),
                RootVisualManager = _rootVisualManager
            };

#if !DEBUG
            _startActions.Add(CheckForUpdates);
#endif
            _startActions.Add(GetAssembliesMetadata);
            _startActions.Add(CheckQuotaSize);
            _startActions.Add(DownloadAssemblies);
            _startActions.Add(LoadAssemblies);
            _startActions.Add(CreateBootstrapperAndRun);

            // startup actions
            PerformNextAction();
        }

        void PerformNextAction()
        {
            if (_startActions.Count == 0)
                return;
            var action = _startActions[0];
            _startActions.Remove(action);
            action();
        }

        void CheckForUpdates()
        {
            // if application is running out-of-brownser, we need to check for updates on server.
            if (CurrentApp.IsRunningOutOfBrowser)
            {
                _splash.DisplayMessage("Procurando por atualizações...");
                CheckAndDownloadUpdateCompletedEventHandler downloadUpdateCompleted = null;
                downloadUpdateCompleted = new CheckAndDownloadUpdateCompletedEventHandler((sender, args) =>
                {
                    //unregister this event from app
                    CurrentApp.CheckAndDownloadUpdateCompleted -= downloadUpdateCompleted;

                    // if a update is available, notify the user don't perform any subsequent action.
                    if (args.UpdateAvailable)
                    {
                        _splash.HideProgressBar();
                        MessageBox.Show("A aplicacação foi atualizada para uma nova versão. Por favor, reinicie o aplicativo.", "Atualização", MessageBoxButton.OK);
                        _splash.DisplayMessage("Aplicação foi atualizada. Reinicie por favor.");
                    }

                    // check if the version of silverlight is compatible with update, if not, notify user and stop start process.
                    else if (args.Error != null && args.Error is PlatformNotSupportedException)
                    {
                        _splash.HideProgressBar();
                        MessageBox.Show("Uma nova versão da aplicação está disponível, mas não é compatível com a versão do Silverlight instalada em seu computador. Por favor, visite o site do Silverlight e baixe uma nova versão do plug-in.", "Atualização", MessageBoxButton.OK);
                    }
                    // if not update is available and version is compatible, continue to perform start actions.
                    else
                        PerformNextAction();

                });

                // bind to update completed event and verify por updates.
                CurrentApp.CheckAndDownloadUpdateCompleted += downloadUpdateCompleted;
                CurrentApp.CheckAndDownloadUpdateAsync();
            }

            // else, app is running on brownser and no update-check is required.
            else
                PerformNextAction();
        }

        void GetAssembliesMetadata()
        {
            _splash.DisplayMessage("Conectando...");
            var client = new WebClient();

            // uri for assemblies metadata
            var fileUri = CurrentApp.Host.GetRelativeUrl("/WingCltAppSupport/GetAssembliesMetadata");

            // bind to downstring completed event
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler((sender, e) =>
            {
                // someting goes wrong, stop start process.
                if (e.Cancelled)
                {
                    _splash.DisplayMessage("A conexão com servidor foi cancelada. Reinice o aplicativo.");
                    _splash.HideProgressBar();
                }
                // an error ocurred, display error message in ui;
                else if (e.Error != null)
                {
                    _splash.DisplayMessage(String.Format("A tentativa de comunicação com servidor falhou \n " +
                        "Mensagem:  {0} \n Caminho: {1}", e.Error.Message, fileUri.ToString()));
                    _splash.HideProgressBar();
                }
                // success, perform next action
                else
                {
                    _assemblyInfo = AssemblyInfoCollection.DeserializeFromXml(e.Result);
                    PerformNextAction();

                }
            });

            // call server
            client.DownloadStringAsync(fileUri);
        }

        void DownloadAssemblies()
        {
            //verificar se é a primeira vez que esta aplicação será executada neste computador, se for, exibir
            //a mensagem "Preparando para usar pela primeira vez neste computador"
            var isFirstTime = true;
            var storage = IsolatedStorageFile.GetUserStoreForApplication();
            if (storage.FileExists("firsttime.tmp"))
            {
                _splash.DisplayMessage("Carregando...");
                isFirstTime = false;
            }
            else
                _splash.DisplayMessage("O Wing está preparando a aplicação para ser executada pela primeira vez \n isto pode levar alguns minutos, aguarde por favor...");

            _splash.DisplayProgressBar(_assemblyInfo.Assemblies.Sum(a => a.Size));

            var assemblies = new Stack<AssemblyInfo>(_assemblyInfo.Assemblies);
            var client = new WebClient();

            AssemblyInfo currentInfo = null;

            // download assembly action
            var downloadNextAssemblyAction = new Action(() =>
            {
                // continue load assemblies
                currentInfo = assemblies.Pop();
                var uri = new Uri(CurrentApp.Host.GetBaseUrl(), "WingCltAppSupport/GetAssemblyData?file=" + currentInfo.AssemblyName);
                _splash.UpdateProgressBar(0, currentInfo.Size);
                client.OpenReadAsync(uri);
            });

            var checkAndGoToNextAction = new Action(() =>
            {
                // assemblies list is empty = all assemblies download, perform next action
                if (assemblies.Count == 0)
                {
                    if (isFirstTime)
                    {
                        var tmpFile = storage.CreateFile("firsttime.tmp");
                        tmpFile.WriteByte(65);
                        tmpFile.Close();
                    }
                    PerformNextAction();
                }
                else
                    downloadNextAssemblyAction();
            });

            // bind to open read completed to update UI and load next assembly
            client.OpenReadCompleted += new OpenReadCompletedEventHandler((sender, e) =>
            {
                if (e.Error != null)
                {
                    _splash.DisplayMessage("Erro ao baixar um arquivo necessário à aplicação: " + currentInfo.AssemblyName);
                    _splash.HideProgressBar();
                }
                var data = new byte[e.Result.Length];
                e.Result.Read(data, 0, data.Length);
                _store.AddAssembly(currentInfo.AssemblyName, data);
                checkAndGoToNextAction();
            });

            checkAndGoToNextAction();
        }

        void LoadAssemblies()
        {
            var assemblies = new List<Assembly>();
            foreach (var asmInfo in _assemblyInfo.Assemblies)
            {
                var stream = new System.IO.MemoryStream(_store.GetAssemblyData(asmInfo.AssemblyName));
                var part = new AssemblyPart();
                var assembly = part.Load(stream);
                assemblies.Add(assembly);
                stream.Close();
                stream.Dispose();
            }

            //set loaded assemblies in bootstrapp settings
            _bootstrapSettings.Assemblies = assemblies;

            //perform next action
            PerformNextAction();
        }

        void CreateBootstrapperAndRun()
        {
            _splash.DisplayMessage("Inicializando...");
            _splash.DisplayLoadingBar();
            //search for bootstrapper class in assemblies
            Type bootstrapperType = null;
            foreach (var assembly in _bootstrapSettings.Assemblies)
            {
                bootstrapperType = assembly.GetExportedTypes().FirstOrDefault(type =>
                    typeof(IBootstrapper).IsAssignableFrom(type)
                    && !type.IsAbstract
                    && !type.IsInterface
                    && !type.IsGenericType);
                if (bootstrapperType != null)
                    break;
            }

            if (bootstrapperType == null)
            {
                _splash.DisplayMessage("Ocorreu um erro ao inicializar a aplicação: 001 - bootstrapper not found");
                _splash.HideProgressBar();
                return;
            }

            // instantiate the bootstrapper class
            var bootstrapperInstance = (IBootstrapper)Activator.CreateInstance(bootstrapperType);

            // and run
            bootstrapperInstance.Run(_bootstrapSettings);
        }

        void CheckQuotaSize()
        {
            var storage = IsolatedStorageFile.GetUserStoreForApplication();
            var size = 1024 * 1024 * 150;
            if (storage.Quota < size)
            {
                _splash.DisplayMessage("Não é possível continuar: espaço insuficiente no disco.");
                return;
            }
            else
                PerformNextAction();
        }
    }
}
