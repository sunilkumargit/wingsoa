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
using Ranet.ZipLib;

namespace Ranet.AgOlap.Controls.General
{
	public class StorageManager : IStorageManager
	{
		String m_URL = String.Empty;
		public String URL
		{
			get { return m_URL; }
			set { m_URL = value; }
		}

		public StorageManager(String url)
		{
			URL = url;
		}

		//void ModifyEndPoint(OlapWebService.OlapWebServiceSoapClient service)
		//{
		//  if (service != null)
		//  {
		//    if (!String.IsNullOrEmpty(URL))
		//    {
		//      service.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(URL));
		//    }
		//    else
		//    {
		//      service.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(Application.Current.Host.Source, "/OlapWebService.asmx"));
		//    }
		//  }
		//}

		#region IStorageManager Members

		void service_PerformStorageActionCompleted(object sender, Ranet.AgOlap.OlapWebService.PerformOlapServiceActionCompletedEventArgs e)
		{
            InvokeResultDescriptor result = null;
            if (e.Error == null)
            {
                result = InvokeResultDescriptor.Deserialize(e.Result);
            }
			if (result != null)
			{
				if (result.IsArchive)
				{
					result.Content = ZipCompressor.DecompressFromBase64String(result.Content);
					result.IsArchive = false;
				}

				Raise_InvokeCompleted(new DataLoaderEventArgs(result, e.Error, e.UserState));
			}
		}

		void Raise_InvokeCompleted(DataLoaderEventArgs args)
		{
			EventHandler<DataLoaderEventArgs> handler = this.InvokeCompleted;
			if (handler != null)
			{
				handler(this, args);
			}
		}

		public void Invoke(object schema, object state)
        {
            if (schema == null)
                throw new ArgumentNullException("schema");

            OlapWebService.OlapWebServiceSoapClient service =
             Services.ServiceManager.CreateService
						 < Ranet.AgOlap.OlapWebService.OlapWebServiceSoapClient
						 , Ranet.AgOlap.OlapWebService.OlapWebServiceSoap
						 >(URL);
            // ModifyEndPoint(service);
            service.PerformOlapServiceActionCompleted += service_PerformStorageActionCompleted;
            service.PerformOlapServiceActionAsync("StorageAction" ,schema.ToString(), state);
        }

		public event EventHandler<DataLoaderEventArgs> InvokeCompleted;

		#endregion
	}
}
