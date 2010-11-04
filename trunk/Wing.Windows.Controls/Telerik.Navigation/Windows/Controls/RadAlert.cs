namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;

    [TemplatePart(Name="Ok", Type=typeof(Button))]
    public class RadAlert : ContentControl
    {
        private Button buttonOk;
        private DialogParameters dialogParams;

        public event EventHandler Ok;

        public RadAlert()
        {
            base.DefaultStyleKey = typeof(RadAlert);
        }

        public virtual void Configure(RadWindow window, DialogParameters pars)
        {
            if ((window.Header == null) || string.IsNullOrEmpty(window.Header.ToString()))
            {
                window.Header = "Alert";
            }
            this.ParentWindow = window;
            this.dialogParams = pars;
        }

        public override void OnApplyTemplate()
        {
            this.buttonOk = base.GetTemplateChild("OK") as Button;
            if (this.buttonOk != null)
            {
                this.buttonOk.Click += new RoutedEventHandler(this.OnOkButtonClicked);
                this.buttonOk.SetValue(RadWindow.ResponseButtonProperty, ResponseButton.Accept);
                if (this.dialogParams.OkButtonContent != null)
                {
                    this.buttonOk.Content = this.dialogParams.OkButtonContent;
                }
            }
        }

        protected virtual void OnOk(EventArgs e)
        {
            if (this.ParentWindow != null)
            {
                this.ParentWindow.DialogResult = true;
                this.ParentWindow.Close();
            }
            if (this.Ok != null)
            {
                this.Ok(this, e);
            }
        }

        private void OnOkButtonClicked(object sender, RoutedEventArgs e)
        {
            this.OnOk(new EventArgs());
        }

        protected RadWindow ParentWindow { get; private set; }
    }
}

