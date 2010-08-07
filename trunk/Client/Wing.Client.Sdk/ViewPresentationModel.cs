using System;

namespace Wing.Client.Sdk
{
    public class ViewPresentationModel : PresentationModel, IViewPresentationModel
    {
        public ViewPresentationModel() { }
        public ViewPresentationModel(String caption, String title)
        {
            Caption = caption;
            Title = title;
        }

        private string _caption;
        private string _title;

        public virtual String Caption
        {
            get { return _caption; }
            set { _caption = value; NotifyPropertyChanged("Caption"); }
        }

        public virtual String Title
        {
            get { return _title; }
            set { _title = value; NotifyPropertyChanged("Title"); }
        }
    }
}
