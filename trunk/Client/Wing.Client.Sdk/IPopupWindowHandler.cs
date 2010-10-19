using System;
using Wing.Composite.Regions;
using Wing.Client.Sdk.Controls;

namespace Wing.Client.Sdk
{
    public interface IPopupWindowHandler
    {
        void SetTitle(String title);
        void SetDialogStyle(ModalDialogStyles style);
        void Close(bool fireEvents = true);
        event EventHandler<DialogResultArgs> Closing;
        event EventHandler<DialogResultArgs> Closed;
    }
}
