﻿/*
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
using Wing.ZipCompression;
using System.Collections.Generic;
using System.Linq;
using Wing.AgOlap.Providers;
using Wing.AgOlap.Controls.PivotGrid;

namespace Wing.AgOlap.Controls.General
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
			service.PerformOlapServiceActionCompleted += new EventHandler<Wing.AgOlap.OlapWebService.PerformOlapServiceActionCompletedEventArgs>(service_PerformOlapServiceActionCompleted);
			service.PerformOlapServiceActionAsync(schema.ActionType.ToString(), XmlSerializationUtility.Obj2XmlStr(schema, Constants.XmlNamespace), state);
		}

		void service_PerformOlapServiceActionCompleted(object sender, Wing.AgOlap.OlapWebService.PerformOlapServiceActionCompletedEventArgs e)
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
