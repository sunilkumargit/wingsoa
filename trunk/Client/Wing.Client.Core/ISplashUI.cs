using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.IO.IsolatedStorage;

namespace Wing.Client.Core
{
    public interface ISplashUI
    {
        void DisplayMessage(string message);
        void DisplayLoadingBar();
        void DisplayProgressBar(int max);
        void UpdateProgressBar(int absoluteValue, int relativeValue);
        void HideProgressBar();
    }
}
