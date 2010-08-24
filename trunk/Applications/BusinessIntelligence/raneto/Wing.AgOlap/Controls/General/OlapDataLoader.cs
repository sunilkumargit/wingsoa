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
using Ranet.AgOlap.Controls.General.ClientServer;
using Ranet.Olap.Core.Providers.ClientServer;
using Ranet.ZipLib;
using System.Collections.Generic;
using System.Linq;
using Ranet.AgOlap.Providers;
using Ranet.AgOlap.Controls.PivotGrid;

namespace Ranet.AgOlap.Controls.General
{
	public class OlapDataLoader : IDataLoader
	{
        internal static SessionHolder IdHolder = new SessionHolder();

		String m_URL = String.Empty;
		public String URL
		{
			get { return m_URL; }
			set { m_URL = value; }
		}

		public OlapDataLoader(String url)
		{
			URL = url;
		}

		#region IDataLoader Members

		public void LoadData(OlapActionBase schema, object state)
		{
			var service = Services.ServiceManager.CreateService
			< OlapWebService.OlapWebServiceSoapClient
			, OlapWebService.OlapWebServiceSoap
            >(URL); //>(URL, new TimeSpan(0, 5, 0));

            MdxQueryArgs mdxQuery = schema as MdxQueryArgs;
            if (mdxQuery != null)
            {
                mdxQuery.SessionId = IdHolder[mdxQuery.Connection];
            }
			service.PerformOlapServiceActionCompleted += new EventHandler<Ranet.AgOlap.OlapWebService.PerformOlapServiceActionCompletedEventArgs>(service_PerformOlapServiceActionCompleted);
			service.PerformOlapServiceActionAsync(schema.ActionType.ToString(), XmlSerializationUtility.Obj2XmlStr(schema, Common.Namespace), state);
		}

		void service_PerformOlapServiceActionCompleted(object sender, Ranet.AgOlap.OlapWebService.PerformOlapServiceActionCompletedEventArgs e)
		{
			InvokeResultDescriptor result = null;
			if (e.Error == null)
			{
				result = InvokeResultDescriptor.Deserialize(e.Result);
			}

            if (result != null)
            {
                String connectionId = String.Empty;
                if (result.Headers.Contains(InvokeResultDescriptor.SESSION_ID) &&
                    result.Headers.Contains(InvokeResultDescriptor.CONNECTION_ID))
                {
                    Header session_header = result.Headers[InvokeResultDescriptor.SESSION_ID];
                    Header connection_header = result.Headers[InvokeResultDescriptor.CONNECTION_ID];
                    if (connection_header != null)
                    {
                        connectionId = connection_header.Value;
                        if (session_header != null)
                        {
                            IdHolder[connection_header.Value] = session_header.Value;
                        }
                    }
                }

                if (result.IsArchive)
                {
                    result.Content = ZipCompressor.DecompressFromBase64String(result.Content);
                    result.IsArchive = false;
                }
            }

			Raise_DataLoaded(new DataLoaderEventArgs(result, e.Error, e.UserState));
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
