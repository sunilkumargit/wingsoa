using System;
using System.Windows;

namespace Wing.Client.Core
{
    public interface IRootVisualManager
    {
        void SetRootElement(UIElement element);
        void AddResourceDictionary(string assemblyName, params string[] assetsNames);
        void Dispatch(Action action);
    }
}
