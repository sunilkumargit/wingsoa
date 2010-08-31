/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

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
using Wing.Olap.Core;
using Wing.AgOlap.Controls.General.ClientServer;
using Wing.Olap.Core.Providers.ClientServer;

namespace Wing.AgOlap.Controls.General
{
    public interface IDataLoader
    {
        void LoadData(OlapActionBase schema, object state);
        event EventHandler<DataLoaderEventArgs> DataLoaded;
    }

    public interface IStorageManager
    {
        void Invoke(object schema, object state);
        event EventHandler<DataLoaderEventArgs> InvokeCompleted;
    }

    public class DataLoaderEventArgs : EventArgs
    {
        public readonly Exception Error = null;
        InvokeResultDescriptor m_Result = null;
        public InvokeResultDescriptor Result
        {
            get {
                if (m_Result == null)
                {
                    m_Result = new InvokeResultDescriptor();
                } 
                return m_Result;
            }
        }
        public readonly object UserState = null;

        public DataLoaderEventArgs(InvokeResultDescriptor result, Exception error, object state)
        {
            Error = error;
            m_Result = result;
            UserState = state;
        }
    }
}
