Loader.run("ShellClient", function ()
{
    var ctx = window;
    if (!ctx.Shell)
        ctx.Shell = Util.getFromParentWindow("Shell");
});