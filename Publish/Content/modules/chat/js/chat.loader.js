Loader.run("ChatLoader", function ()
{
    var tb = Shell.getStatusBar();
    tb.createGroup("chat", true);
    var btn = new wui.Button({
        caption: "Comunicador",
        iconClass: "ui-icon-person",
        isToggle: true,
        isChecked: false,
        onClick: function (e, btn)
        {
            window.visible(this.isChecked());
        }
    });

    tb.add(btn, "chat");

    var window = new wui.ContentWindow({
        vAlign: VALIGN.BOTTOM,
        hAlign: HALIGN.RIGHT,
        marginRight: 10,
        marginBottom: 30,
        width: 600,
        height: 500,
        dependsOn: ["chat.client"]
    });
    window.hide();

    WindowManager.block();
});
