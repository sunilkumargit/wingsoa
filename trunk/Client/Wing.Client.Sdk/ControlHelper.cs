using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Wing.Client.Sdk
{
    public static class ControlHelper
    {
        public static ImageSource ResolveImageSource(FrameworkElement element, ImageSource value)
        {
            var image = value as BitmapImage;
            if (image == null) return value;

            image.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>((sender, args) =>
            {

                var uri = image.UriSource.OriginalString;
                //se não existir a ;Component/, então colocar
                var componentStr = ";Component/";
                if (uri.IndexOf(componentStr, StringComparison.OrdinalIgnoreCase) == -1)
                {
                    if (uri.StartsWith("/"))
                        uri = uri.Substring(1);
                    uri = componentStr + uri;
                }

                // falta o assembly
                if (uri.StartsWith(";"))
                {
                    var parents = GetParentsUserControls(element);

                    foreach (var userControl in parents)
                    {
                        var asmName = userControl.GetType().Assembly.FullName;
                        asmName = asmName.Substring(0, asmName.IndexOf(','));
                        var resUri = new Uri(asmName + uri, UriKind.Relative);
                        //tentar ler o resource
                        var _resStream = Application.GetResourceStream(resUri);
                        if (_resStream != null && _resStream.Stream != null)
                        {
                            image.SetSource(_resStream.Stream);
                            return;
                        }
                    }
                }

                //tentar ler o resource
                var resStream = Application.GetResourceStream(new Uri(uri, UriKind.Relative));
                if (resStream != null && resStream.Stream != null)
                    image.SetSource(resStream.Stream);
                else
                    image.UriSource = new Uri(uri, UriKind.Relative);
            });

            return image;
        }

        public static List<UserControl> GetParentsUserControls(DependencyObject element)
        {
            var result = new List<UserControl>();
            var parent = element;
            while (parent != null)
            {
                if (parent is UserControl)
                    result.Add((UserControl)parent);
                parent = VisualTreeHelper.GetParent(parent);
            }
            return result;
        }
    }
}
