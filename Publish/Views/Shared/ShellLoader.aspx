<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ViewHost.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" EnableSessionState="True"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    FlexWeb 1.00 - Empflex Software 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="InitContent" runat="server">
    <script type="text/javascript">
        <%= ViewHelper.GetResourceMapString() %>
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function(){
            $(".wui-loading-box:first").addClass("ui-widget-overlay");
        });

        Loader.mapResource("wing.pipeline.manager", "js", "/content/js/wing.pipeline.manager.js");
        Loader.mapResource("wing.pipeline.listener", "js", "/content/js/wing.pipeline.listener.js");
        Loader.mapResource("wing.shell", "js", "/content/js/wing.shell.js");
        Loader.mapResource("wing.users.api", "js", "/content/js/wing.users.api.js");
        Loader.dependsOn("wing.pipeline.manager",  "wing.pipeline.listener", "wing.users.api", "wing.shell");
        <%= ViewHelper.GetResourceInitString(ResourceLoadMode.GlobalAddin) %>
        <%= ViewHelper.GetResourceInitString(ResourceLoadMode.ShellAddin) %>
        Loader.run("ShellLoader");
    </script>
</asp:Content>
