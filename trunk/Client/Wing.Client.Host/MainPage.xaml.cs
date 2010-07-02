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

namespace Wing.Client
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            if (App.Current.IsRunningOutOfBrowser)
            {
                Helper.DelayExecution(TimeSpan.FromMilliseconds(500), () =>
                {
                    StatusText.Text = "Procurando atualizações...";
                    App.Current.CheckAndDownloadUpdateCompleted += new CheckAndDownloadUpdateCompletedEventHandler(Current_CheckAndDownloadUpdateCompleted);
                    App.Current.CheckAndDownloadUpdateAsync();
                    return false;
                });
            }
        }

        void Current_CheckAndDownloadUpdateCompleted(object sender, CheckAndDownloadUpdateCompletedEventArgs e)
        {
            if (e.UpdateAvailable)
            {
                MessageBox.Show("A aplicacação foi atualizada para uma nova versão. Por favor, reinicie o aplicativo.");
                StatusText.Text = "Aplicação atualizada. Reinicie por favor.";
            }
            else if (e.Error != null &&
               e.Error is PlatformNotSupportedException)
            {
                MessageBox.Show("Uma nova versão da aplicação está disponível, mas não é compatível com a versão do Silverlight instalada em seu computador. Por favor, visite o site do Silverlight e baixe uma nova versão do plug-in.");
            }
            else
            {
                Helper.DelayExecution(TimeSpan.FromMilliseconds(1500), () =>
                {
                    BeginBootstrap();
                    return false;
                });
            }
        }

        void BeginBootstrap()
        {
            StatusText.Text = "Inicializando...";
        }
    }
}
