/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;
using Microsoft.AnalysisServices.AdomdClient;
using Wing.Web.Olap;

namespace UILibrary.Olap.UITestApplication.Web
{
    /// <summary>
    /// Summary description for OlapWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class OlapWebService : OlapWebServiceBase
    {
        string SetConnectionString(string argument)
        {
            try
            {
                var err = "Argument Schema must be ConnectionId=ConnectionString";
                if (string.IsNullOrEmpty(argument))
                    throw new ArgumentException(err);

                var ind = argument.IndexOf('=');
                if (ind < 1)
                    throw new ArgumentException(err);

                var connectionName = argument.Substring(0, ind);
                var connectionString = argument.Substring(ind + 1);

                if (string.IsNullOrEmpty(connectionName))
                    throw new ArgumentException(err);

                if (string.IsNullOrEmpty(connectionString))
                    connectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;

                using (var connection = new AdomdConnection(connectionString))
                {
                    string s = "";
                    connection.Open();
                    foreach (var c in connection.Cubes)
                    {
                        s += c.Name;
                    }
                }
                this.Application[connectionName] = connectionString;
                return connectionName+"="+connectionString;
            }
            catch (Exception E)
            {
                return E.ToString();
            }
        }
        [WebMethod]
        public override string PerformOlapServiceAction(String schemaType, string schema)
        {
            if (string.IsNullOrEmpty(schemaType))
                return "schemaType argument can be CheckExist,GetConnectionString,SetConnectionString and others but not empty.";
            switch (schemaType.Trim().ToUpper())
            {
                case "CHECKEXIST":
                    return "OK";
                case "GETCONNECTIONSTRING":
                    return this.Application[schema] as string;
                case "SETCONNECTIONSTRING":
                    return SetConnectionString(schema);
                default:
                    var result = base.PerformOlapServiceAction(schemaType, schema);
                    return result;
            }
        }
    }
}
