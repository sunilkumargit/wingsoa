namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    [TemplatePart(Name="PromptTextBox", Type=typeof(TextBox))]
    public class RadPrompt : RadConfirm
    {
        private DialogParameters dialogParams;
        private TextBox promptTextBox;

        public RadPrompt()
        {
            base.DefaultStyleKey = typeof(RadPrompt);
        }

        public override void Configure(RadWindow window, DialogParameters pars)
        {
            this.dialogParams = pars;
            if ((window.Header == null) || string.IsNullOrEmpty(window.Header.ToString()))
            {
                window.Header = "Prompt";
            }
            base.Configure(window, pars);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.promptTextBox = base.GetTemplateChild("PromptTextBox") as TextBox;
            if ((this.promptTextBox != null) && !string.IsNullOrEmpty(this.dialogParams.DefaultPromptResultValue))
            {
                this.promptTextBox.Text = this.dialogParams.DefaultPromptResultValue;
            }
        }

        protected override void OnOk(EventArgs e)
        {
            if ((base.ParentWindow != null) && (this.promptTextBox != null))
            {
                base.ParentWindow.PromptResult = this.promptTextBox.Text;
            }
            base.OnOk(e);
        }
    }
}

