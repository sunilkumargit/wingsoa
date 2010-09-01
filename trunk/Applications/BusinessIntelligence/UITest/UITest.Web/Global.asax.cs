using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace UILibrary.Olap.UITestApplication.Web
{
	public class Global : System.Web.HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
			foreach (ConnectionStringSettings css in ConfigurationManager.ConnectionStrings)
			{
				this.Application[css.Name]=css.ConnectionString;
			}
		}
	}
}