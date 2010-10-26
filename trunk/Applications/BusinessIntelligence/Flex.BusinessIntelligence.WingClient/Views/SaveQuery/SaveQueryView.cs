using System;
using System.Windows.Controls;
using Wing.Client.Sdk.Controls;

namespace Flex.BusinessIntelligence.WingClient.Views.SaveQuery
{
    public class SaveQueryView : HeaderedPage
    {
        private SaveQueryPresentationModel _model;
        private DataForm dataForm;

        public SaveQueryView()
        {
            dataForm = new DataForm();
            dataForm.AutoGeneratingField += new EventHandler<DataFormAutoGeneratingFieldEventArgs>(dataForm_AutoGeneratingField);
            this.Content = dataForm;
        }

        void dataForm_AutoGeneratingField(object sender, DataFormAutoGeneratingFieldEventArgs e)
        {
            e.Field.MinWidth = 400;
        }

        public void SetModel(SaveQueryPresentationModel model)
        {
            _model = model;
            dataForm.AutoGenerateFields = true;
            dataForm.CurrentItem = model;
            dataForm.AutoCommit = true;
            dataForm.BeginEdit();
            dataForm.Focus();
        }

        public bool Validate()
        {
            return dataForm.ValidateItem();
        }

        public void CancelEdit()
        {
            dataForm.CancelEdit();
        }
    }
}