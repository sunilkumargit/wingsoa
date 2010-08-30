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
using Ranet.Olap.Core;

namespace Ranet.AgOlap.Controls.General
{
    public class MembersDataLoader : IDataLoader
    {
        String m_URL = String.Empty;
        public String URL
        {
            get { return m_URL; }
            set { m_URL = value; }
        }

        public MembersDataLoader(String url)
        {
            URL = url;
        }

        #region IDataLoader Members

        void ModifyEndPoint(OlapWebService.OlapWebServiceSoapClient service)
        {
            if (service != null)
            {
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

        public void LoadData(object schema, object state)
        {
            OlapWebService.OlapWebServiceSoapClient service = new Ranet.AgOlap.OlapWebService.OlapWebServiceSoapClient();
            ModifyEndPoint(service);
            service.GetMembersDataCompleted += new EventHandler<Ranet.AgOlap.OlapWebService.GetMembersDataCompletedEventArgs>(service_GetMembersDataCompleted);
            service.GetMembersDataAsync(schema.ToString(), state);
        }

        void service_GetMembersDataCompleted(object sender, Ranet.AgOlap.OlapWebService.GetMembersDataCompletedEventArgs e)
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
