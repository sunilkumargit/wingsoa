namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using Wing.Windows.Controls.Telerik.Navigation.Windows.Controls;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class RadPage : UserControl, IFrame
    {
        private string title;

        public RadPage()
        {
            this.NavigationIdentifier = base.GetType().FullName;
            base.Loaded += new RoutedEventHandler(this.Page_Loaded);
        }

        private void GetChildElement(DependencyObject obj)
        {
            int count = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < count; i++)
            {
                if (VisualTreeHelper.GetChild(obj, i) is RadFrameContainer)
                {
                    this.ChildContainer = VisualTreeHelper.GetChild(obj, i) as Panel;
                    return;
                }
                DependencyObject currentElement = VisualTreeHelper.GetChild(obj, i);
                int currentIndex = VisualTreeHelper.GetChildrenCount(VisualTreeHelper.GetChild(obj, i));
                for (int j = 0; j < currentIndex; j++)
                {
                    if (VisualTreeHelper.GetChild(currentElement, j) is RadFrameContainer)
                    {
                        this.ChildContainer = VisualTreeHelper.GetChild(currentElement, j) as Panel;
                        break;
                    }
                }
            }
        }

        private void InitializeChildContainer()
        {
            int count = VisualTreeHelper.GetChildrenCount(this);
            for (int i = 0; i < count; i++)
            {
                DependencyObject currentElement = VisualTreeHelper.GetChild(this, i);
                if (currentElement is RadFrameContainer)
                {
                    this.ChildContainer = currentElement as Panel;
                    return;
                }
                this.GetChildElement(currentElement);
            }
        }

        private void InitializeParentContainer()
        {
            this.ParentContainer = base.Parent as Panel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.ParentContainer = base.Parent as Panel;
            this.InitializeChildContainer();
            this.InitializeParentContainer();
            if ((this.ChildContainer != null) && (this.ChildContainer.Children.Count > 0))
            {
            }
        }

        public virtual Panel ChildContainer { get; set; }

        public string NavigationIdentifier { get; set; }

        public virtual Panel ParentContainer { get; set; }

        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
            }
        }
    }
}

