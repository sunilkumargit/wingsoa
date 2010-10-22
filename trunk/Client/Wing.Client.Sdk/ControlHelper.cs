using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wing.Utils;

namespace Wing.Client.Sdk
{
    public static class ControlHelper
    {
        public static void SetBorderBrushToDebug(Control element)
        {
#if DEBUG
            var colors = new String[] { "Blue", "Green", "Red" };
            var color = GetPredefinedNamedColor(colors[RandomUtils.Next(0, colors.Length - 1)]);
            //element.BorderThickness = new Thickness(3);
            //element.BorderBrush = color;
#endif
        }


        public static SolidColorBrush GetPredefinedNamedColor(String color)
        {
            string xamlString = "<SolidColorBrush xmlns='http://schemas.microsoft.com/client/2007'"
                + " xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'"
                + " Color='{0}'/>";

            return (SolidColorBrush)XamlReader.Load(String.Format(xamlString, color));
        }

        public static void TryLoadImage(FrameworkElement element, ImageSource source)
        {
            var image = source as BitmapImage;
            if (image == null) return;

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
            {
                var partialUri = image.UriSource.OriginalString
                    .Replace(componentStr, "")
                    .Replace(componentStr.ToLower(), "");
                // não encontrou o assembly provavelmente, procurar nos alias 
                var assemblyAlias = partialUri.Substring(0, partialUri.IndexOf(";"));
                partialUri = String.Format("{0}{1}", componentStr, partialUri.Substring(assemblyAlias.Length + 1));
                var assembliesByAlias = AssembliesAlias.GetAssembliesInAlias(assemblyAlias);
                foreach (var assemblyName in assembliesByAlias)
                {
                    resStream = Application.GetResourceStream(new Uri(assemblyName + partialUri, UriKind.Relative));
                    if (resStream != null)
                    {
                        image.SetSource(resStream.Stream);
                        return;
                    }
                }

                // ultimo recurso
                image.UriSource = new Uri(uri, UriKind.Relative);
            }

        }

        public static ImageSource ResolveImageSource(FrameworkElement element, ImageSource value)
        {
            var image = value as BitmapImage;
            if (image == null) return value;

            image.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>((sender, args) =>
            {
                TryLoadImage(element, value);
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

        public static void StretchContentControl(ContentControl control)
        {
            control.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            control.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            control.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
            control.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
            SetBorderBrushToDebug(control);
        }
    }
}
