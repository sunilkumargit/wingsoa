
using System;
namespace Wing.Client.Sdk
{
    public interface IShellView
    {
        IShellPresentationModel Model { get; set; }
        event EventHandler HomeButtonClicked;
        event EventHandler BackButtonClicked;
    }
}