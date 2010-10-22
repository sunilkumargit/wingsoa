using System;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Controls;

namespace Wing.Client.Modules.Shell
{
    public class ModalDialogWindowHandler : IPopupWindowHandler
    {
        private ModalDialog _dialog;
        private bool _fireEvents = true;

        public ModalDialogWindowHandler(ModalDialog dialog)
        {
            _dialog = dialog;
            dialog.BeforeClosed += new EventHandler<DialogResultArgs>(dialog_BeforeClosed);
            dialog.DialogClosed += new EventHandler<DialogResultArgs>(dialog_DialogClosed);
        }

        void dialog_DialogClosed(object sender, DialogResultArgs e)
        {
            if (!_fireEvents) return;
            VisualContext.Sync(() =>
            {
                if (Closed != null)
                    Closed.Invoke(this, e);
            });
        }

        void dialog_BeforeClosed(object sender, DialogResultArgs e)
        {
            if (!_fireEvents) return;
            VisualContext.Sync(() =>
            {
                if (Closing != null)
                    Closing.Invoke(this, e);
            });
        }

        public void SetTitle(string title)
        {
            VisualContext.Async(() => _dialog.Caption = title);
        }

        public void SetDialogStyle(ModalDialogStyles style)
        {
            VisualContext.Async(() => _dialog.DialogStyle = style);
        }

        public void Close(bool fireEvents = true)
        {
            VisualContext.Sync(() =>
            {
                _fireEvents = fireEvents;
                _dialog.Close();
                _fireEvents = true;
            });
        }

        public event EventHandler<DialogResultArgs> Closing;

        public event EventHandler<DialogResultArgs> Closed;
    }
}
