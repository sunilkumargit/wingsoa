Loader.mapResource("sys_echo", "pipeline");

Loader.run("ShellLoader", function ()
{
    var ctx = window;
    //definir os tipos
    Type.define("wing.ShellView", "wui.ViewPart", {
        methods: {
            init: function ()
            {
                this.base();
                this.id('wx-shell-view');

                // indicador de ajax
                var ajaxLoading = $.newTag("div", { "id": "wx-shell-loading", "class": "ui-corner-all" })
                    .appendTo(this.el).hide();

                var grid = this.grid = new wui.Grid().setRows("28px", "*", "28px").setCols("*");
                this.content(grid);

                // tabs
                var tabs = this.tabs = new wui.Tabs({ margin: { top: 5, right: 4, left: 4, bottom: 4} });
                this.grid.add(0, 1, tabs);

                // main bar
                var mainBar = this._mainBar = new wui.Toolbar().vAlign(VALIGN.STRETCH);
                grid.add(0, 0, mainBar);

                mainBar.createGroup("shell_main_group", false);
                mainBar.add(new wui.Button({ caption: "Aplicativos", iconClass: "ui-icon-gear", dropDownMenuGroup: "applications" }), "shell_main_group");
                mainBar.add(new wui.Button({ caption: "Ferramentas", iconClass: "ui-icon-wrench", dropDownMenuGroup: "tools" }), "shell_main_group");
                mainBar.add(new wui.Button({ caption: "Ajuda", iconClass: "ui-icon-help", dropDownMenuGroup: "help" }), "shell_main_group");

                var statusBar = this._statusBar = new wui.Toolbar({ id: "shellSb" }).css("height", "").vAlign(VALIGN.STRETCH);
                grid.add(0, 2, statusBar);
                this.stMsg = new wui.TextBlock({ id: "statusMessage" });
                statusBar.add(this.stMsg);

                // home frame
                var homeFrame = new wui.ContentFrame();
               // var homeTab = this.openTab("home_frame", "Home");
            },

            openTab: function (tabId, caption)
            {
                return this.tabs.addTabItem(tabId, caption);
            },

            statusMessage: function (message)
            {
                this.stMsg.text(message);
            }
        }
    });
    var shellView = new wing.ShellView().id("shell");
    Services.register("shell", shellView);
    WindowManager.add(shellView);


    ctx.Shell = {
        getStatusBar: function ()
        {
            return shellView._statusBar;
        },
        getMainBar: function ()
        {
            return shellView._mainBar;
        },
        openTab: function (tabId, caption, content)
        {
            return shellView.openTab(tabId, caption).add(content);
        },
        statusMessage: function (message)
        {
            shellView.statusMessage(message);
        }
    };
});