using System;
using System.Collections.Generic;
using System.Windows;
using Wing.Client.Sdk;
using Wing.Utils;

namespace Wing.Client.Modules.Shell.Views
{
    public class ShellPresentationModel : ViewPresentationModel, IShellPresentationModel
    {
        private string _statusMessage;
        private bool _progressBarIsVisible;
        private bool _progressBarIsIndeterminate;
        private int _progressMaxValue;
        private int _progressValue;
        private List<IViewPresenter> _activeViews;
        private string _activeViewsText;
        private bool _backButtonEnabled;

        public ShellPresentationModel()
            : base("Shell", "Shell")
        {
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ShellPresentationModel_PropertyChanged);
        }

        void ShellPresentationModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ProgressBarIsVisible")
                NotifyPropertyChanged("ProgressBarVisibility");
        }

        public string StatusMessage
        {
            get { return _statusMessage; }
            set { _statusMessage = value; NotifyPropertyChanged("StatusMessage"); }
        }

        public bool ProgressBarIsVisible
        {
            get { return _progressBarIsVisible; }
            set { _progressBarIsVisible = value; NotifyPropertyChanged("ProgressBarIsVisible"); }
        }

        public bool ProgressBarIsIndeterminate
        {
            get { return _progressBarIsIndeterminate; }
            set { _progressBarIsIndeterminate = value; NotifyPropertyChanged("ProgressBarIsIndeterminate"); }
        }

        public int ProgressMaxValue
        {
            get { return _progressMaxValue; }
            set { _progressMaxValue = value; NotifyPropertyChanged("ProgressMaxValue"); }
        }

        public int ProgressValue
        {
            get { return _progressValue; }
            set { _progressValue = value; NotifyPropertyChanged("ProgressValue"); }
        }

        public bool BackButtonEnabled
        {
            get { return _backButtonEnabled; }
            set { _backButtonEnabled = value; NotifyPropertyChanged("BackButtonEnabled"); }
        }

        public Visibility ProgressBarVisibility
        {
            get { return ProgressBarIsVisible ? Visibility.Visible : Visibility.Collapsed; }
        }

        public String ActiveViewsText
        {
            get { return _activeViewsText; }
            set { _activeViewsText = value; NotifyPropertyChanged("ActiveViewsText"); }
        }

        public List<IViewPresenter> ActiveViews
        {
            get { return _activeViews; }
            set
            {
                _activeViews = value; NotifyPropertyChanged("ActiveViews");
                BuildActiveViewText();
            }
        }

        void BuildActiveViewText()
        {
            if (ActiveViews == null)
                ActiveViewsText = "";
            var text = "";
            foreach (var item in ActiveViews)
            {
                if (item.Caption.HasValue())
                    text += (text.Length == 0 ? "" : " > ") + item.Caption;
            }
            ActiveViewsText = text;
        }
    }
}
