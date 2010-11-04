namespace Telerik.Windows.Controls
{
    using System;

    public interface ITransition
    {
        void Begin(IFrame page);
        void StopTransition();

        bool IsRunning { get; set; }
    }
}

