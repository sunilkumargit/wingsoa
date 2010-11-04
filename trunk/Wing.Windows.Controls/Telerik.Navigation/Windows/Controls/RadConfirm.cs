namespace Telerik.Windows.Controls
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;

    [TemplatePart(Name="Cancel", Type=typeof(Button))]
    public class RadConfirm : RadAlert
    {
        private Button cancelButton;
        private DialogParameters dialogParams;

        public event EventHandler Cancel;

        public RadConfirm()
        {
            base.DefaultStyleKey = typeof(RadConfirm);
        }

        public override void Configure(RadWindow window, DialogParameters pars)
        {
            this.dialogParams = pars;
            if ((window.Header == null) || string.IsNullOrEmpty(window.Header.ToString()))
            {
                window.Header = "Confirm";
            }
            base.Configure(window, pars);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.cancelButton = base.GetTemplateChild("Cancel") as Button;
            if (this.cancelButton != null)
            {
                this.cancelButton.Click += new RoutedEventHandler(this.OnCancelButtonClicked);
                this.cancelButton.SetValue(RadWindow.ResponseButtonProperty, ResponseButton.Cancel);
                if (this.dialogParams.CancelButtonContent != null)
                {
                    this.cancelButton.Content = this.dialogParams.CancelButtonContent;
                }
            }
        }

        protected virtual void OnCancel(EventArgs e)
        {
            if (base.ParentWindow != null)
            {
                base.ParentWindow.DialogResult = false;
                base.ParentWindow.Close();
            }
            if (this.Cancel != null)
            {
                this.Cancel(this, e);
            }
        }

        private void OnCancelButtonClicked(object sender, RoutedEventArgs e)
        {
            this.OnCancel(new EventArgs());
        }
    }
}

