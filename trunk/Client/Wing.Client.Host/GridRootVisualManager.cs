using System;
using System.Windows;
using System.Windows.Controls;
using Wing.Client.Core;

namespace Wing.Client.Host
{
    internal class GridRootVisualManager : IRootVisualManager
    {
        private Grid _grid;

        public GridRootVisualManager(Grid grid)
        {
            _grid = grid;
        }

        #region IRootVisualManager Members

        public void SetRootElement(UIElement element)
        {
            Dispatch(() =>
            {
                while (_grid.Children.Count > 0)
                    _grid.Children.RemoveAt(0);
                _grid.Children.Add(element);
            });
        }

        public void AddResourceDictionary(String assemblyName, params String[] assetsNames)
        {
            Dispatch(() =>
            {
                foreach (var assetName in assetsNames)
                {
                    var resUri = new Uri(String.Format("/{0};Component/{1}.xaml", assemblyName, assetName), UriKind.Relative);
                    var resDic = new ResourceDictionary();
                    resDic.Source = resUri;
                    Application.Current.Resources.MergedDictionaries.Add(resDic);
                }
            });
        }

        private void Dispatch(Action action)
        {
            if (_grid.Dispatcher.CheckAccess())
                action();
            else
                _grid.Dispatcher.BeginInvoke(() => action());
        }
        #endregion
    }
}
