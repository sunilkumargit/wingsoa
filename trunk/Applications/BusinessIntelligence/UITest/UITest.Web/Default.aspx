<%@  language="C#" enablesessionstate="false" autoeventwireup="false" %>
<%@ import namespace="System.IO" %>
<%@ import namespace="System.Diagnostics" %>
<%@ import namespace="System.Configuration" %>
<%@ import namespace="System.Web" %>
<%@ import namespace="System.Threading" %>
<%@ import namespace="System.Xml" %>
<%@ import namespace="System.Text" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title>UILibrary.Olap.UITestApplication</title>
	<style type="text/css">
		html, body
		{
			height: 100%;
			overflow: auto;
		}
		body
		{
			padding: 0;
			margin: 0;
		}
		#silverlightControlHost
		{
			height: 100%;
			text-align: center;
		}
	</style>

	<script type="text/javascript" src="Silverlight.js"></script>

	<script type="text/javascript">
		function onSilverlightError(sender, args) {
			var appSource = "";
			if (sender != null && sender != 0) {
				appSource = sender.getHost().Source;
			}

			var errorType = args.ErrorType;
			var iErrorCode = args.ErrorCode;

			if (errorType == "ImageError" || errorType == "MediaError") {
				return;
			}

			var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

			errMsg += "Code: " + iErrorCode + "    \n";
			errMsg += "Category: " + errorType + "       \n";
			errMsg += "Message: " + args.ErrorMessage + "     \n";

			if (errorType == "ParserError") {
				errMsg += "File: " + args.xamlFile + "     \n";
				errMsg += "Line: " + args.lineNumber + "     \n";
				errMsg += "Position: " + args.charPosition + "     \n";
			}
			else if (errorType == "RuntimeError") {
				if (args.lineNumber != 0) {
					errMsg += "Line: " + args.lineNumber + "     \n";
					errMsg += "Position: " + args.charPosition + "     \n";
				}
				errMsg += "MethodName: " + args.methodName + "     \n";
			}

			throw new Error(errMsg);
		}
	</script>

	<script runat="server">
static int RecC=0;

string ConnectionStrings="";
protected override void OnLoad (EventArgs args)
{
	base.OnLoad (args);
	ConnectionStrings="";
	foreach (ConnectionStringSettings css in ConfigurationManager.ConnectionStrings)
	{
		ConnectionStrings+=css.Name +"=\""+css.ConnectionString+"\"<br/>";
	}

 // HttpContext.Current.Response.ContentType="text/plain";
}
	</script>

</head>
<body>
Lets check that all is OK.
<br/>
<br/>
You are at Url="<%= HttpContext.Current.Request.Url %>"
<br/>
IsSecureConnection=<%= HttpContext.Current.Request.IsSecureConnection %>
<br/>
You are connected to your web server as UserIdentity='<%= HttpContext.Current.User.Identity.Name %>'
<br/>
IsAutentificated=<%= HttpContext.Current.User.Identity.IsAuthenticated %>
<br/>
Identity.GetType()=<%= HttpContext.Current.User.Identity.GetType().Name %>
<br/>
AutentificationType=<%= HttpContext.Current.User.Identity.AuthenticationType.ToString() %>
<br/>
Your web server runs (or is impersonated) under ServiceIdentity="<%= System.Security.Principal.WindowsIdentity.GetCurrent().Name %>"
<br/>
This ServiceIdentity wil be used as OLAP connection identity.
<br/>
<br/>
Connection strings which are specified in Web.config file:
<br/>
<%= ConnectionStrings %>
<br/>
OLAPConnectionString will be used if you will not explicitly set it at Configuration tab.
<br/>
<br/>
<a href="SilverliteApplication.htm">If you find that all above is correct please click here to start the sample silverlight application.</a>
<br/>
<br/>
<a href="OlapWebService.asmx">Alternatively you can check that OlapWebService.asmx is available.</a>
<br/>
Possible parameters for PerformOlapServiceAction:
<br/>
schemaType:  CheckExist	
<br/>
schema: 
<br/>
<br/>
schemaType:  GetConnectionString
<br/>
schema: OLAPConnectionString
<br/>
<br/>
schemaType:  SetConnectionString
<br/>
schema: OLAPConnectionString=Your connection string to OLAP server
<br/>
<br/>
This page is generated at <%= DateTime.Now %>
<br/>
Call number: <%= ++RecC %>
<br/>
</body>
</html>
