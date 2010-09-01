<%@ language="C#" EnableSessionState="false" AutoEventWireup="false" %>
<%@ Import namespace="System.IO" %>
<%@ Import namespace="System.Diagnostics" %>
<%@ Import namespace="System.Configuration" %>
<%@ Import namespace="System.Web" %>
<%@ Import namespace="System.Threading" %>
<%@ Import namespace="System.Xml" %>
<%@ Import namespace="System.Text" %>
<script runat="server">
   static int RecC=0;
//   string msg="Before Load";
//   string fn=@"\\anatol2003\c$\autoexec.bat";

protected override void OnLoad (EventArgs args)
{
 base.OnLoad (args);
 HttpContext.Current.Response.ContentType="text/plain";
/*	  
 msg=@"</p><p>Пробуем прочитать "+fn;
 try
 {
  StreamReader sr=new StreamReader(fn);
  msg+=@"</p><p>"+sr.ReadToEnd();
  sr.Close();
 }
 catch(Exception E)
 {
  msg+=@"</p><p>Error: "+E.ToString();
 }
*/
}
</script>Call number: <%= ++RecC %>
ServiceIdentity=<%= System.Security.Principal.WindowsIdentity.GetCurrent().Name %>
UserIdentity=<%= HttpContext.Current.User.Identity.Name %>
IsAutentificated=<%= HttpContext.Current.User.Identity.IsAuthenticated %>
AutentificationType=<%= HttpContext.Current.User.Identity.AuthenticationType.ToString() %>
Identity.GetType()=<%= HttpContext.Current.User.Identity.GetType().Name %>
Url=<%= HttpContext.Current.Request.Url %>
IsSecureConnection=<%= HttpContext.Current.Request.IsSecureConnection %>
Generated: <%= DateTime.Now %>

