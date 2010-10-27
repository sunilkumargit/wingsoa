<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>


<%
//    foreach(var v in Request.ServerVariables.AllKeys)
 //        Response.Write("<p>" + v + " = " + Request.ServerVariables[v] + "</p>");
    var baseUri = new Uri(HttpContext.Current.Request.Url, Request.ApplicationPath);
    Response.Write("<br/>" + (new Uri(baseUri.ToString() + "/srvmgr").ToString()));
    %>
<%




 %>