using System;
using System.Collections.Generic;
using System.Windows;
using Wing.Client.Core;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Events;
using Wing.Composite.Events;
using Wing.Composite.Regions;
using Wing.Events;
using Wing.Client.Sdk.Services;
using Wing.ServiceLocation;
using Wing.Client.Sdk.Controls;
using System.Windows.Controls;

namespace Wing.Client.Modules.Shell
{
    public class ModalDialogWindowHandler : IPopupWindowHandler
    {
        private ModalDialog _dialog;

        public ModalDialogWindowHandler(ModalDialog dialog)
        {
            _dialog = dialog;
            dialog.BeforeClosed += new EventHandler<DialogResultArgs>(dialog_BeforeClosed);
            dialog.DialogClosed += new EventHandler<DialogResultArgs>(dialog_DialogClosed);
        }

        void dialog_DialogClosed(object sender, DialogResultArgs e)
        {
            if (Closing != null)
                Closing.Invoke(this, e);
        }

        void dialog_BeforeClosed(object sender, DialogResultArgs e)
        {
            if (Closed != null)
                Closed.Invoke(this, e);
        }

        public void SetTitle(string title)
        {
            _dialog.Caption = title;
        }

        public void SetDialogStyle(ModalDialogStyles style)
        {
            _dialog.DialogStyle = style;
        }

        public void Close()
        {
            _dialog.Close();
        }

        public event EventHandler<DialogResultArgs> Closing;

        public event EventHandler<DialogResultArgs> Closed;
    }
}
