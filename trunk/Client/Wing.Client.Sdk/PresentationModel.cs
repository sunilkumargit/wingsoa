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
using Wing.Utils;
using System.Text;
using System.Collections.Generic;

namespace Wing.Client.Sdk
{
    public class PresentationModel : IPresentationModel, INotifyPropertyChanged, IDataErrorInfo
    {
        public PresentationModel()
        {
            Errors = new Dictionary<string, string>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        string IDataErrorInfo.Error
        {
            get
            {
                if (Errors.ContainsKey("_MODEL_"))
                    return Errors["_MODEL_"];
                else if (Errors.Count > 0)
                    return "Verifique os items abaixo";
                return null;
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get { return GetError(columnName); }
        }

        public string GetError(string propertyName)
        {
            String result = null;
            Errors.TryGetValue(String.IsNullOrEmpty(propertyName) ? "_MODEL_" : propertyName, out result);
            return result;
        }

        public void SetError(string propertyName, String message)
        {
            Errors[String.IsNullOrEmpty(propertyName) ? "_MODEL_" : propertyName] = message;
        }

        public void ClearError(string propertyName)
        {
            propertyName = String.IsNullOrEmpty(propertyName) ? "_MODEL_" : propertyName;
            if (Errors.ContainsKey(propertyName))
                Errors.Remove(propertyName);
        }

        public void ClearErrors()
        {
            Errors.Clear();
        }

        public Dictionary<string, string> Errors { get; private set; }
    }
}
