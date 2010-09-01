using System;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace UILibrary.Olap.UITestApplication
{
    public partial class Page
    {
        void BindData()
        {
            tbOlapWebServiceUrl.Text = @"http://localhost:16580/Wing/OlapWebService.asmx";

            foreach (var pi in Config.Default.Data.GetType().GetProperties())
            {
                var myBinding = new Binding("Data." + pi.Name);
                myBinding.Source = Config.Default;
                myBinding.Mode = BindingMode.TwoWay;
                myBinding.UpdateSourceTrigger = UpdateSourceTrigger.Default;
                myBinding.BindsDirectlyToSource = true;
                myBinding.NotifyOnValidationError = true;
                var tb = this.FindName("tb" + pi.Name) as TextBox;
                if (tb != null)
                    tb.SetBinding(TextBox.TextProperty, myBinding);
                var cb = this.FindName("cb" + pi.Name) as ComboBoxColors;
                if (cb != null)
                    cb.SetBinding(ComboBoxColors.SelectedIndexProperty, myBinding);
            }
        }
        void SetDefaultValues_Click(object sender, RoutedEventArgs e)
        {
            Config.SetDefault();
        }
        void SaveCurrentValues_Click(object sender, RoutedEventArgs e)
        {
            Config.Save();
        }
        private void LoadLastSavedValues_Click(object sender, RoutedEventArgs e)
        {
            Config.Load();
        }
        private void CheckConnection_Click(object sender, RoutedEventArgs e)
        {
            tbLastError.Text = "";
            CheckedInfo.Text = "Connection checking started...";
            Config.Init
            (ConnectionStringId, tbOLAPConnectionString.Text
            , () =>
            {
                try
                {
                    tbOLAPConnectionString.Text = Config.Default.Data.OLAPConnectionString;
                    CheckedInfo.Text = @"Connection has been succesfully set and checked.";
                    //System.Windows.Browser.HtmlPage.Window.SetProperty("status","Done!");
                    //initCubeChoiceButton_Click(null, null);
                    //initmdxDesignerButton_Click(null, null);
                    //initPivotGridButton_Click(null, null);
                    //initServerExplorerButton_Click(null, null);
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.ToString(), "Error", MessageBoxButton.OK);
                }
            }
            , () =>
            {
                CheckedInfo.Text = "There were errors while connection checking....";
                tbLastError.Text = Config.LastError;
            }
            );
        }
    }
}
