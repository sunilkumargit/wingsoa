using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
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
            _bootstrapSettings = new BootstrapSettings()
            {
                ServerBaseAddress = CurrentApp.Host.GetBaseUrl(),
                RootVisualManager = _rootVisualManager,
                Splash = _splash
            };
        }

        private Application CurrentApp { get { return Application.Current; } }

        public void Run()
        {
            _store = new ClientAssemblyStore();
            _startActions.Add(FirstConnection);
#if !DEBUG
            _startActions.Add(CheckForUpdates);
#endif
            _startActions.Add(GetAssembliesMetadata);
            _startActions.Add(CheckQuotaSize);
            _startActions.Add(DownloadAssemblies);
            _startActions.Add(LoadAssemblies);
            _startActions.Add(ReadSoaMetadaProviderServiceUri);
            _startActions.Add(CreateBootstrapperAndRun);

            // startup actions
            PerformNextAction();
        }

        void PerformNextAction()
        {
            if (_startActions.Count == 0)
                return;
            _splash.DisplayStatusMessage("");
            var action = _startActions[0];
            _startActions.Remove(action);
            action();
        }

        void CheckForUpdates()
        {
            // if application is running out-of-brownser, we need to check for updates on server.
            if (CurrentApp.IsRunningOutOfBrowser)
            {
                _splash.DisplayMessage("Procurando por atualiza��es...");
                CheckAndDownloadUpdateCompletedEventHandler downloadUpdateCompleted = null;
                downloadUpdateCompleted = new CheckAndDownloadUpdateCompletedEventHandler((sender, args) =>
                {
                    //unregister this event from app
                    CurrentApp.CheckAndDownloadUpdateCompleted -= downloadUpdateCompleted;

                    // if a update is available, notify the user and don't perform any subsequent action.
                    if (args.UpdateAvailable)
                    {
                        _splash.HideProgressBar();
                        MessageBox.Show("A aplicaca��o foi atualizada para uma nova vers�o. Por favor, reinicie o aplicativo.", "Atualiza��o", MessageBoxButton.OK);
                        _splash.DisplayMessage("Aplica��o foi atualizada. Reinicie por favor.");
                        ScheduleApplicationTerminate();
                    }

                    // check if the version of silverlight is compatible with update, if not, notify user and stop start process.
                    else if (args.Error != null && args.Error is PlatformNotSupportedException)
                    {
                        _splash.HideProgressBar();
                        MessageBox.Show("Uma nova vers�o da aplica��o est� dispon�vel, mas n�o � compat�vel com a vers�o do Silverlight instalada em seu computador. Por favor, visite o site do Silverlight e baixe uma nova vers�o do plug-in.", "Atualiza��o", MessageBoxButton.OK);
                        ScheduleApplicationTerminate();
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
            var client = new WebClient();

            // uri for assemblies metadata
            var fileUri = CurrentApp.Host.GetRelativeUrl("/WingCltAppSupport/GetAssembliesMetadata");

            // bind to downstring completed event
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler((sender, e) =>
            {
                // someting goes wrong, stop start process.
                if (e.Cancelled)
                {
                    _splash.DisplayMessage("A conex�o com servidor foi cancelada. Reinice o aplicativo.");
                    _splash.HideProgressBar();
                }
                // an error ocurred, display error message in ui;
                else if (e.Error != null)
                {
                    _splash.DisplayMessage(String.Format("A tentativa de comunica��o com servidor falhou \n " +
                        "Mensagem:  {0} \n Caminho: {1}", e.Error.Message, fileUri.ToString()));
                    _splash.HideProgressBar();
                    ScheduleApplicationTerminate();
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
            var assemblies = GetAssembliesToDownload();

            if (assemblies.Count == 0)
                PerformNextAction();

            //verificar se � a primeira vez que esta aplica��o ser� executada neste computador, se for, exibir
            //a mensagem "Preparando para usar pela primeira vez neste computador"
            var isFirstTime = true;
            var storage = IsolatedStorageFile.GetUserStoreForApplication();
            if (storage.FileExists("firsttime.tmp"))
            {
                _splash.DisplayMessage("Baixando atualiza��es...");
                isFirstTime = false;
            }
            else
                _splash.DisplayMessage("O Wing est� preparando a aplica��o para ser executada pela primeira vez, \n isto pode levar alguns minutos, aguarde por favor...");

            var downloadSize = assemblies.Sum(a => a.Size);

            _splash.DisplayProgressBar(downloadSize);


            var client = new WebClient();

            AssemblyInfo currentInfo = null;

            // download assembly action
            var downloadNextAssemblyAction = new Action(() =>
            {
                // continue load assemblies
                currentInfo = assemblies.Pop();

                var uri = new Uri(CurrentApp.Host.GetBaseUrl(), "WingCltAppSupport/GetAssemblyData?file=" + currentInfo.AssemblyName);
                _splash.DisplayStatusMessage("Baixando " + currentInfo.AssemblyName);
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
                    _splash.UpdateProgressBar(downloadSize, 0);
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
                    _splash.DisplayMessage("Erro ao baixar um arquivo necess�rio � aplica��o: " + currentInfo.AssemblyName);
                    _splash.HideProgressBar();
                    ScheduleApplicationTerminate();
                }
                var data = new byte[e.Result.Length];
                e.Result.Read(data, 0, data.Length);
                _store.AddAssembly(currentInfo.AssemblyName, data);
                checkAndGoToNextAction();
            });

            checkAndGoToNextAction();
        }

        private Stack<AssemblyInfo> GetAssembliesToDownload()
        {
            var result = new Stack<AssemblyInfo>();
#if DEBUG
            return new Stack<AssemblyInfo>(_assemblyInfo.Assemblies);
#else
            foreach (var asmInfo in _assemblyInfo.Assemblies)
            {
                //verificar se o arquivo � diferente do servidor
                var asmData = _store.GetAssemblyData(asmInfo.AssemblyName);
                if (asmData != null)
                {
                    var hash = AssemblyInfo.CalculateHashString(asmData);
                    //se o hash for igual, ir para o pr�ximo assembly
                    if (hash.Equals(asmInfo.HashString))
                        continue;
                }
                result.Push(asmInfo);
            }
            return result;
#endif
        }

        void LoadAssemblies()
        {
            _splash.DisplayMessage("Iniciando...");
            _splash.DisplayLoadingBar();
            var assemblies = new List<Assembly>();
            try
            {
                foreach (var asmInfo in _assemblyInfo.Assemblies)
                {
                    _splash.DisplayStatusMessage("Carregando " + asmInfo.AssemblyName);
                    var stream = new System.IO.MemoryStream(_store.GetAssemblyData(asmInfo.AssemblyName));
                    var part = new AssemblyPart();
                    var assembly = part.Load(stream);
                    assemblies.Add(assembly);
                    stream.Close();
                    stream.Dispose();
                }
            }
            catch (Exception ex)
            {
                _splash.DisplayMessage("Ocorreu um erro ao carregar os arquivos: " + ex.Message);
                ScheduleApplicationTerminate();
                return;
            }

            //set loaded assemblies in bootstrapp settings
            _bootstrapSettings.Assemblies = assemblies;

            //perform next action
            PerformNextAction();
        }

        void CreateBootstrapperAndRun()
        {
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
                _splash.DisplayMessage("Ocorreu um erro ao inicializar a aplica��o: 001 - bootstrapper not found");
                _splash.HideProgressBar();
                ScheduleApplicationTerminate();
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
            if (storage.Quota < Constants.ClientQuotaSize)
            {
                _splash.DisplayMessage("N�o � poss�vel continuar: espa�o insuficiente no disco.");
                ScheduleApplicationTerminate();
                return;
            }
            else
                PerformNextAction();
        }

        void FirstConnection()
        {
            _splash.DisplayMessage("Conectando ao servidor...");
            var uri = CurrentApp.Host.GetRelativeUrl("/ServerHello.aspx");
            var tries = 10;
            _splash.DisplayProgressBar(tries);
            _splash.UpdateProgressBar(tries -1, 0);
            var client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler((sender, args) =>
            {
                if (args.Error != null)
                {
                    if (--tries == 0)
                    {
                        _splash.HideProgressBar();
                        _splash.DisplayMessage("Ocorreu um problema ao conectar-se ao servidor.");
                        ScheduleApplicationTerminate();
                    }
                    else
                    {
                        _splash.DisplayStatusMessage("N�o houve resposta do servidor... tentando novamente");
                        _splash.UpdateProgressBar(0, -1);
                        Helper.DelayExecution(TimeSpan.FromSeconds(3), () =>
                        {
                            client.DownloadStringAsync(uri);
                            return false;
                        });
                    }
                }
                else
                {
                    PerformNextAction();
                }
            });
            client.DownloadStringAsync(uri);
        }

        void ReadSoaMetadaProviderServiceUri()
        {
            WebClient client = new WebClient();

            // uri for assemblies metadata
            var fileUri = CurrentApp.Host.GetRelativeUrl("/WingCltAppSupport/GetSoaMetaProviderServiceUri");

            // bind to downstring completed event
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler((sender, e) =>
            {
                // someting goes wrong, stop start process.
                if (e.Cancelled)
                {
                    _splash.DisplayMessage("A conex�o com servidor foi cancelada. Reinice o aplicativo.");
                    _splash.HideProgressBar();
                    ScheduleApplicationTerminate();
                }
                // an error ocurred, display error message in ui;
                else if (e.Error != null)
                {
                    _splash.DisplayMessage(String.Format("A tentativa de comunica��o com servidor falhou \n " +
                        "Mensagem:  {0} \n Caminho: {1}", e.Error.Message, fileUri.ToString()));
                    _splash.HideProgressBar();
                    ScheduleApplicationTerminate();
                }
                // success, perform next action
                else
                {
                    _bootstrapSettings.SoaMetadataProviderUri = new Uri(e.Result);
                    PerformNextAction();
                }
            });

            // call server
            client.DownloadStringAsync(fileUri);
        }

        private void ScheduleApplicationTerminate()
        {
            var cnt = 9;
            Helper.DelayExecution(TimeSpan.FromSeconds(1), new Func<bool>(() =>
            {
                if (cnt == 0)
                {
                    Application.Current.MainWindow.Close();
                    return false;
                }
                else
                {
                    _splash.DisplayStatusMessage("A aplica��o fechar� automaticamente em " + cnt.ToString() + " segundos.");
                    cnt--;
                    return true;
                }
            }));
        }
    }
}
