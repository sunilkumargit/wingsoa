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
using System.ComponentModel;
using System.Collections.Generic;

namespace Wing.Client.Sdk
{
    public interface IPresentationModel : INotifyPropertyChanged, IDataErrorInfo
    {
        String GetError(String propertyName);
        Dictionary<String, String> Errors { get; }
        void SetError(String propertyName, String message);
        void ClearError(String propertyName);
        void ClearErrors();
    }
}
