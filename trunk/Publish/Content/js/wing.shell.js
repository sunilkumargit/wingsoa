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

                var grid = this.grid = new wui.Grid().setRows("26px", "*", "26px").setCols("*");
                this.content(grid);

                // tabs
                var tabs = this.tabs = new wui.Tabs({ margin: { top: 5, right: 4, left: 4, bottom: 4} });
                this.grid.add(0, 1, tabs);

                // main bar
                var mainBar = this._mainBar = new wui.Toolbar().css("height", "").vAlign(VALIGN.STRETCH);
                grid.add(0, 0, mainBar);
                mainBar.createGroup("current_user", true);
                mainBar.add(new wui.Button({ caption: User.name(), secondIconClass: "ui-icon-carat-1-s",
                    onClick: function (e)
                    {
                        WindowManager.showContextMenu({
                            menu: {
                                items: [
                               { text: "Perfil...", iconClass: "ui-icon ui-icon-person" },
                               { text: "Sair", iconClass: "ui-icon ui-icon-close" }
                            ]
                            },
                            relativeTo: this
                        });
                        e.stopPropagation();
                    }
                }), "current_user");


                mainBar.add(new wui.Button({ caption: "Sair", iconClass: "ui-icon-close", onClick: function () { window.location = '/Login/Signout'; } }), "current_user");

                mainBar.createGroup("applications", false);
                mainBar.add(new wui.Button({ caption: "Aplicativos", iconClass: "ui-icon-gear" }), "applications");
                mainBar.add(new wui.Button({ caption: "Ferramentas", iconClass: "ui-icon-wrench" }), "applications");
                mainBar.add(new wui.Button({ caption: "Ajuda", iconClass: "ui-icon-help" }), "applications");

                var statusBar = this._statusBar = new wui.Toolbar({ id: "shellSb" }).css("height", "").vAlign(VALIGN.STRETCH);
                grid.add(0, 2, statusBar);

                // home frame
                var homeFrame = new wui.ContentFrame();

                var homeTab = this.openTab("home_frame", "Home");
                homeTab.add(homeFrame);
            },

            openTab: function (tabId, caption)
            {
                return this.tabs.addTabItem(tabId, caption);
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
        }
    };
});