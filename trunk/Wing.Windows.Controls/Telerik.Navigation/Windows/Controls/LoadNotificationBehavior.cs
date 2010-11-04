namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;

    internal class LoadNotificationBehavior
    {
        public static readonly DependencyProperty InstanceProperty = DependencyProperty.RegisterAttached("Instance", typeof(LoadNotificationBehavior), typeof(LoadNotificationBehavior), null);

        public LoadNotificationBehavior(DependencyObject d)
        {
            this.InitializeImage(d as System.Windows.Controls.Image);
            this.InitializeMediaElement(d as MediaElement);
        }

        private static bool HasMemoryStreamSource(System.Windows.Controls.Image image)
        {
            BitmapImage source = image.Source as BitmapImage;
            return ((source != null) && string.IsNullOrEmpty(source.UriSource.OriginalString));
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            this.ImageStateChanged(delegate (RadCoverFlowItem item) {
                item.IsContentValid = false;
            }, true);
        }

        private void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
            this.ImageStateChanged(delegate (RadCoverFlowItem item) {
                item.IsLoading = false;
            }, true);
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            this.ImageStateChanged(delegate (RadCoverFlowItem item) {
                item.IsLoading = true;
            }, false);
        }

        private void ImageStateChanged(Action<RadCoverFlowItem> updateParentState, bool clearAllEventHandlers)
        {
            if (clearAllEventHandlers)
            {
                this.Image.ImageOpened -= new EventHandler<RoutedEventArgs>(this.Image_ImageOpened);
                this.Image.ImageFailed -= new EventHandler<ExceptionRoutedEventArgs>(this.Image_ImageFailed);
                this.Image.ClearValue(InstanceProperty);
            }
            this.Image.Loaded -= new RoutedEventHandler(this.Image_Loaded);
            RadCoverFlowItem item = this.Image.ParentOfType<RadCoverFlowItem>();
            if (item != null)
            {
                updateParentState(item);
            }
        }

        private void InitializeImage(System.Windows.Controls.Image image)
        {
            if ((image != null) && !HasMemoryStreamSource(image))
            {
                image.Loaded += new RoutedEventHandler(this.Image_Loaded);
                image.ImageOpened += new EventHandler<RoutedEventArgs>(this.Image_ImageOpened);
                image.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>(this.Image_ImageFailed);
                this.Image = image;
                this.IsValid = true;
            }
        }

        private void InitializeMediaElement(MediaElement media)
        {
            if (media != null)
            {
                media.Loaded += new RoutedEventHandler(this.Media_Loaded);
                media.MediaOpened += new RoutedEventHandler(this.MediaElement_MediaOpened);
                media.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>(this.MediaElement_MediaFailed);
                this.Media = media;
                this.IsValid = true;
            }
        }

        private void Media_Loaded(object sender, RoutedEventArgs e)
        {
            this.MediaStateChanged(delegate (RadCoverFlowItem item) {
                item.IsLoading = true;
            }, false);
        }

        private void MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            this.MediaStateChanged(delegate (RadCoverFlowItem item) {
                item.IsContentValid = false;
            }, true);
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            this.MediaStateChanged(delegate (RadCoverFlowItem item) {
                item.IsLoading = false;
            }, true);
        }

        private void MediaStateChanged(Action<RadCoverFlowItem> updateParentState, bool clearAllEventHandlers)
        {
            if (clearAllEventHandlers)
            {
                this.Media.MediaOpened -= new RoutedEventHandler(this.MediaElement_MediaOpened);
                this.Media.MediaFailed -= new EventHandler<ExceptionRoutedEventArgs>(this.MediaElement_MediaFailed);
                this.Media.ClearValue(InstanceProperty);
            }
            this.Media.Loaded -= new RoutedEventHandler(this.Media_Loaded);
            RadCoverFlowItem item = this.Media.ParentOfType<RadCoverFlowItem>();
            if (item != null)
            {
                updateParentState(item);
            }
        }

        private System.Windows.Controls.Image Image { get; set; }

        public bool IsValid { get; set; }

        private MediaElement Media { get; set; }
    }
}

