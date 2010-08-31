/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
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
using Ranet.Olap.Core.Providers.ClientServer;
using Ranet.Olap.Core;
using System.IO;
using System.Text;

namespace Ranet.AgOlap.Controls.General
{
    public class PivotDataLoader : IPivotDataLoader
    {
        String m_URL = String.Empty;
        public String URL
        {
            get { return m_URL; }
            set { m_URL = value; }
        }

        public PivotDataLoader(String url)
        {
            URL = url;
        }

        #region IDataLoader Members

        void ModifyEndPoint(OlapWebService.OlapWebServiceSoapClient service)
        {
            if (service != null)
            {
                // Таймаут service.InnerChannel.OperationTimeout = new TimeSpan(0, 10, 0);
                if (!String.IsNullOrEmpty(URL))
                {
                    service.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(URL));
                }
                else
                {
                    service.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(Application.Current.Host.Source, "/OlapWebService.asmx"));
                }
            }
        }

        public void ChangeDataSource(object schema, object state)
        { 
        
        }

        public void LoadPivotData(object schema, object state)
        {
            OlapWebService.OlapWebServiceSoapClient service = new Ranet.AgOlap.OlapWebService.OlapWebServiceSoapClient();
            ModifyEndPoint(service);
            service.GetPivotDataCompleted += new EventHandler<Ranet.AgOlap.OlapWebService.GetPivotDataCompletedEventArgs>(service_GetPivotDataCompleted);
            service.GetPivotDataAsync(schema.ToString(), state);
        }

        public void ExecuteQuery(object schema, object state)
        {
            OlapWebService.OlapWebServiceSoapClient service = new Ranet.AgOlap.OlapWebService.OlapWebServiceSoapClient();
            ModifyEndPoint(service);
            service.ExecuteQueryCompleted += new EventHandler<Ranet.AgOlap.OlapWebService.ExecuteQueryCompletedEventArgs>(service_ExecuteQueryCompleted);
            service.ExecuteQueryAsync(schema.ToString(), state);
        }

        public void PerformMemberAction(object schema, object state)
        {
            OlapWebService.OlapWebServiceSoapClient service = new Ranet.AgOlap.OlapWebService.OlapWebServiceSoapClient();
            ModifyEndPoint(service);
            service.PerformMemberActionCompleted += new EventHandler<Ranet.AgOlap.OlapWebService.PerformMemberActionCompletedEventArgs>(service_PerformMemberActionCompleted);
            service.PerformMemberActionAsync(schema.ToString(), state);
        }

        public void PerformServiceCommand(object schema, object state)
        {
            OlapWebService.OlapWebServiceSoapClient service = new Ranet.AgOlap.OlapWebService.OlapWebServiceSoapClient();
            ModifyEndPoint(service);

            //ServiceCommandArgs args = state as ServiceCommandArgs;
            //if (args != null && args.Command == ServiceCommandType.ExportToExcel)
            //{
            //    service.RunExcelAsync(schema.ToString(), state);
            //    return;
            //}

            service.PerformServiceCommandCompleted += new EventHandler<Ranet.AgOlap.OlapWebService.PerformServiceCommandCompletedEventArgs>(service_PerformServiceCommandCompleted);
            service.PerformServiceCommandAsync(schema.ToString(), state);
        }

        public void GetToolBarInfo(object schema, object state)
        {
            OlapWebService.OlapWebServiceSoapClient service = new Ranet.AgOlap.OlapWebService.OlapWebServiceSoapClient();
            ModifyEndPoint(service);
            service.GetToolBarInfoCompleted += new EventHandler<Ranet.AgOlap.OlapWebService.GetToolBarInfoCompletedEventArgs>(service_GetToolBarInfoCompleted);
            service.GetToolBarInfoAsync(schema.ToString(), state);
        }

        public void UpdateCube(object schema, object state)
        {
            OlapWebService.OlapWebServiceSoapClient service = new Ranet.AgOlap.OlapWebService.OlapWebServiceSoapClient();
            ModifyEndPoint(service); 
            service.UpdateCubeCompleted += new EventHandler<Ranet.AgOlap.OlapWebService.UpdateCubeCompletedEventArgs>(service_UpdateCubeCompleted);
            service.UpdateCubeAsync(schema.ToString(), state);
        }

        void service_ExecuteQueryCompleted(object sender, Ranet.AgOlap.OlapWebService.ExecuteQueryCompletedEventArgs e)
        {
            InvokeResultDescriptor result = XmlSerializationUtility.XmlStr2Obj<InvokeResultDescriptor>(e.Result);
            if (result != null)
            {
                //if (result.IsArchive)
                //{
                //    result.Content = ZipCompressor.DecompressFromBase64String(result.Content);
                //    result.IsArchive = false;
                //}

                Raise_DataLoaded(new DataLoaderEventArgs(result, e.Error, e.UserState));
            }
        }

        void service_UpdateCubeCompleted(object sender, Ranet.AgOlap.OlapWebService.UpdateCubeCompletedEventArgs e)
        {
            InvokeResultDescriptor result = XmlSerializationUtility.XmlStr2Obj<InvokeResultDescriptor>(e.Result);
            if (result != null)
            {
                //if (result.IsArchive)
                //{
                //    result.Content = ZipCompressor.DecompressFromBase64String(result.Content);
                //    result.IsArchive = false;
                //}

                Raise_DataLoaded(new DataLoaderEventArgs(result, e.Error, e.UserState));
            }
        }

        void service_GetToolBarInfoCompleted(object sender, Ranet.AgOlap.OlapWebService.GetToolBarInfoCompletedEventArgs e)
        {
            InvokeResultDescriptor result = XmlSerializationUtility.XmlStr2Obj<InvokeResultDescriptor>(e.Result);
            if (result != null)
            {
                //if (result.IsArchive)
                //{
                //    result.Content = ZipCompressor.DecompressFromBase64String(result.Content);
                //    result.IsArchive = false;
                //}

                Raise_DataLoaded(new DataLoaderEventArgs(result, e.Error, e.UserState));
            }
        }

        void service_PerformServiceCommandCompleted(object sender, Ranet.AgOlap.OlapWebService.PerformServiceCommandCompletedEventArgs e)
        {
            InvokeResultDescriptor result = XmlSerializationUtility.XmlStr2Obj<InvokeResultDescriptor>(e.Result);
            if (result != null)
            {
                //if (result.IsArchive)
                //{
                //    result.Content = ZipCompressor.DecompressFromBase64String(result.Content);
                //    result.IsArchive = false;
                //}

                Raise_DataLoaded(new DataLoaderEventArgs(result, e.Error, e.UserState));
            }
        }

        void service_PerformMemberActionCompleted(object sender, Ranet.AgOlap.OlapWebService.PerformMemberActionCompletedEventArgs e)
        {
            InvokeResultDescriptor result = XmlSerializationUtility.XmlStr2Obj<InvokeResultDescriptor>(e.Result);
            if (result != null)
            {
                //if (result.IsArchive)
                //{
                //    result.Content = ZipCompressor.DecompressFromBase64String(result.Content);
                //    result.IsArchive = false;
                //}

                Raise_DataLoaded(new DataLoaderEventArgs(result, e.Error, e.UserState));
            }
        }

        void service_GetPivotDataCompleted(object sender, Ranet.AgOlap.OlapWebService.GetPivotDataCompletedEventArgs e)
        {
            InvokeResultDescriptor result = XmlSerializationUtility.XmlStr2Obj<InvokeResultDescriptor>(e.Result);
            if (result != null)
            {
                //if (result.IsArchive)
                //{
                //    result.Content = ZipCompressor.DecompressFromBase64String(result.Content);
                //    result.IsArchive = false;
                //}
                
                Raise_DataLoaded(new DataLoaderEventArgs(result, e.Error, e.UserState));
            }
        }

        void Raise_DataLoaded(DataLoaderEventArgs args)
        {
            EventHandler<DataLoaderEventArgs> handler = this.DataLoaded;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion

        #region IDataLoader Members

        public event EventHandler<DataLoaderEventArgs> DataLoaded;

        #endregion
    }
}