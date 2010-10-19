using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Wing.Client.Sdk.Controls;
using Wing.Client.Sdk;
using Flex.BusinessIntelligence.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Flex.BusinessIntelligence.WingClient.Views.RegisterCube
{
    public class CubeRegisterView : HeaderedPage
    {
        private CubeRegisterPresentationModel _model;
        private DataForm dataForm;

        public CubeRegisterView()
        {
            dataForm = new DataForm();
            dataForm.AutoGeneratingField += new EventHandler<DataFormAutoGeneratingFieldEventArgs>(dataForm_AutoGeneratingField);
            this.Content = dataForm;
        }

        void dataForm_AutoGeneratingField(object sender, DataFormAutoGeneratingFieldEventArgs e)
        {
            e.Field.MinWidth = 400;
        }

        public void SetModel(CubeRegisterPresentationModel model)
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