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
            _grid.Children.RemoveAt(0);
            _grid.Children.Add(element);
        }

        public void AddResourceDictionary(String assemblyName, params String[] assetsNames)
        {
            foreach (var assetName in assetsNames)
            {
                var resUri = new Uri(String.Format("/{0};Component/{1}.xaml", assemblyName, assetName), UriKind.Relative);
                var resDic = new ResourceDictionary();
                resDic.Source = resUri;
                Application.Current.Resources.MergedDictionaries.Add(resDic);
            }
        }
        #endregion
    }
}
