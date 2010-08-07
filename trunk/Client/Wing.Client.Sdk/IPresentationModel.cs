using System;
using System.Collections.Generic;
using System.ComponentModel;

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
