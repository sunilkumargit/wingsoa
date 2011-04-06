<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" EnableSessionState="True" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Wing App Server 2.00 - logon</title>
    <link type="text/css" rel="Stylesheet" href="/content/css/jquery.ui.css" />
    <script type="text/javascript" src="/content/js/jquery.js"></script>
    <script type="text/javascript" src="/content/js/jquery.ui.js"></script>
    <style type="text/css">
        body { background-color: #003665; font-family: Tahoma, Sans-Serif; font-size: 14px; color: White; }
        h1 { font-weight: normal; }
        .login-box { background-color: White; width: 380px; min-height: 230px; position: absolute; top: 50%; left: 50%; margin-left: -180px; margin-top: -115px; }
        .login-box-content { }
        .login-box-content input { padding: 3px; font-size: 11px; width: 200px; display: block; float: left; }
        .login-box-content .row { display: block; clear: left; padding-bottom: 20px; }
        .login-box-content label { display: block; float: left; width: 100px; text-align: right; font-weight: bold; line-height: 26px; padding-right: 3px; }
        #sendBtn { font-weight: bold; }
        #forgot { display: block; text-align: left; padding-left: 104px; color: Blue; }
        .ui-state-highlight { margin: 2px; padding: 8px; }
    </style>
    <script type="text/javascript">
        $(function ()
        {
            $("#sendBtn").button();
            $("input:first").focus();
        });
    </script>
</head>
<body>
    <h1>
        Logon</h1>
    <form method="post" action="/Login">
    <div class="login-box ui-widget ui-widget-content ui-corner-all">
        <div class="ui-widget-header">
            Logon
        </div>
        <div class="login-box-content">
            <% var msg = ViewData["msg"].AsString();
               if (msg.HasValue())
               {
                %>
                <div class="ui-state-highlight ui-corner-all">
                    <%= msg %>
                </div>
                <%
               }
            %>
            <span class="row" style="padding-top: 28px; ">
                <label for="usr">
                    Login
                </label>
                <input type="text" name="usr" />
                <span class="ui-helper-clearfix">
                </span>
            </span>
            <span class="row" style="padding-bottom: 4px;">
                <label for="pwd">
                    Senha
                </label>
                <input type="password" name="pwd" />
                <span class="ui-helper-clearfix">
                </span>
            </span>
            <span class="row">
                <a id="forgot" href="/Login/PasswordRecovery">Esqueceu sua senha?</a>
            </span>
            <span class="row" style="text-align: left; padding-bottom: 15px; padding-left: 245px;">
                <button id="sendBtn" type="submit">
                    &nbsp;&nbsp;Login&nbsp;&nbsp;</button>
            </span>
        </div>
    </div>
    </form>
</body>
</html>
