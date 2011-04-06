<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ViewHost.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Wing Content Page 1.41
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Loader.dependsOn("wing.pipeline.listener");
        Loader.dependsOn("wing.users.api");
        <%= ViewHelper.GetResourceInitString(ResourceLoadMode.GlobalAddin) %>
        <%= ViewHelper.GetResourceInitString(ResourceLoadMode.ContentAddin) %>
        Loader.run("ContentLoader", function() {});
    </script>
</asp:Content>
