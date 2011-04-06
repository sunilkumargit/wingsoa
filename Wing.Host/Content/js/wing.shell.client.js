Loader.run("ShellClient", function ()
{
    var ctx = window;
    if (!ctx.Shell)
        ctx.Shell = Util.getFromParentWindow("Shell");

    Loader.onResourceLoading(function (name)
    {
        Shell.statusMessage("Baixando " + name);
    });
    Loader.onResourceLoaded(function (name)
    {
        Shell.statusMessage("");
    });
});