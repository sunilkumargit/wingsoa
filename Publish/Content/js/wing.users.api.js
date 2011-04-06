Loader.run("UsersApi", function ()
{
    Type.define("wui.User", null, {
        properties: {
            id: {},
            name: {}
        }
    });

    Type.define("wui.UserService", null, {
        properties: {
            currentUser: {}
        }
    });

    if (!window.User)
    {
        window.User = Util.getFromParentWindow("User");
        if (!window.User && __boot && __boot.user)
            window.User = new wui.User().set({ id: __boot.user.userId, name: __boot.user.name });
    }

    if (window.User)
        Services.register("Users", new wui.UserService().set({ currentUser: window.User }));
});
