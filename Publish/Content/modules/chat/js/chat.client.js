Loader.dependsOn("chat.style");

Loader.run("ChatClientInit", function ()
{
    //registrar as rotas do chat
    Net.route("chat.signin", "json", "/Chat/Signin");
    Net.route("chat.send", "postJSON", "/Chat/Send");
});

Loader.run("ChatClientClasses", function ()
{
    Type.define("wing.chat.Client", null,
    {
        fields: {
            _contacts: function () { return []; },
            _signedIn: false
        },
        methods: {
            signIn: function ()
            {
                var client = this;
                this._senderKey = Util.uid();

                //se inscrever nas mensagens de pipe do chat
                this.pipeUsrOnline = Net.listen("chat_usr_status", function (data)
                {
                    client.triggerUserStatus(data);
                });

                this.pipeMsgArrive = Net.listen("chat_room_msg", function (data)
                {
                    if (data && data.SenderKey !== client._senderKey)
                        client.triggerMessageArrive(data);
                });

                this.triggerSignIn();
                Net.exec("chat.signin", function (data)
                {
                    if (!data.Signed)
                        client.triggerSignInError(data.Message);
                    else
                    {
                        client._contacts = data.Users;
                        client._user = data.User;
                        client._signed = true;
                        client.triggerSignedIn();
                    }
                });
            },
            signOut: function (callback)
            {
                Net.stopListenning(this.pipeUsrOnline);
                Net.stopListenning(this.pipeMsgArrive);
                var client = this;
                Net.exec("chat.signout", function (data)
                {
                    client.triggerSignedOut();
                });
            },
            sendMessage: function (room, msg)
            {
                Net.exec("chat.send", {
                    SenderKey: this._senderKey,
                    From: this._user.UserId,
                    To: room.mainUserId,
                    RoomId: room.roomId,
                    Message: msg
                }, function (ret)
                {
                    room.roomId = ret.RoomId;
                });
            },
            dispose: function ()
            {
                this.signOut();
            }
        },
        properties: {
            contacts: { get: function () { return this._contacts; }, set: function () { throw "This property is read-only"; } }
        },
        events: {
            signedIn: null,
            signIn: null,
            signInError: null,
            userStatus: null,
            messageArrive: null
        }
    });


    Type.define("wing.chat.RoomView", "wui.ViewPart", {
        methods: {
            init: function ()
            {
                this.base();

                var view = this;
                var grid = new wui.Grid({
                    setRows: ["*", "100px"],
                    setCols: "*"
                });
                this.content(grid);

                this.msgView = msgView = grid.newHtmlHolder().stretch().vScroll(true).addClass("chat-messages");

                var bottomGrid = grid.newGrid({
                    setRows: "*",
                    setCols: ["*", "70px"],
                    gridRow: 1
                });
                var textArea = this.msgBox = (new wui.TextArea()).stretch().addTo(bottomGrid).setFocus();
                var sendBtn = new wui.Button({ caption: "Enviar", gridCol: 1 }).addTo(bottomGrid);
                sendBtn.onClick(function ()
                {
                    var msg = textArea.text();
                    if (!Util.isEmpty(msg))
                    {
                        view.chatClient().sendMessage(view._room, msg);
                        view.addMessage(view._room.owner.UserId, view._room.owner.Name, msg);
                    }
                    textArea.text("");
                    textArea.setFocus();
                });
                textArea.el.keypress(function (e)
                {
                    if (e.keyCode == 13)
                    {
                        sendBtn.triggerClick();
                        e.stopPropagation();
                        return false;
                    }
                });
            },
            addMessage: function (from, name, message)
            {
                this.msgView.newTag("div", { class: "chat-message-item" })
                    .newTag("div", { class: "chat-message-item-header" }).text(name + ":")
                    .parent()
                    .newTag("div", { class: "chat-message-item-text" }).text(message);
                this.msgView.scrollToBottom();
            }
        },
        properties: {
            room: { get: function () { return this._room; }, set: function (value) { this._room = value; } },
            chatClient: {}
        }
    });

    Type.define("wing.chat.View", "wui.ViewPart", {
        fields: {
            _rooms: []
        },
        methods: {
            init: function ()
            {
                this.base();
                var view = this;
                view._rooms = [];

                var toolbar = new wui.Toolbar();
                toolbar.addText("Comunicador");

                var tabs = view.tabs = new wui.Tabs().stretch().gridCoord(1, 0);

                var innerGrid = new wui.Grid({
                    id: "inner_grid",
                    setRows: "*",
                    setCols: ["150px", "*"],
                    marginRight: "2px",
                    marginBottom: "2px",
                    gridRow: 1,
                    children: [tabs]
                });

                this.content(new wui.Grid({
                    id: "grid",
                    setRows: ["26", "*"],
                    setCols: "*",
                    children: [toolbar, innerGrid]
                }));

                this.usersTable = innerGrid.newTable({
                    id: "test",
                    gridCol: 0,
                    gridRow: 0,
                    itemsSource: Binding(),
                    headerVisible: false,
                    footerVisible: false,
                    rowSelectable: true,
                    rowKey: Binding("UserId"),
                    rowData: Binding(),
                    /*
                    showHeader: false,
                    applyTheme: true,
                    recycleTemplates: false,
                    executeDataBind: true,
                    */
                    columns: [
                        { content: Binding(function (data)
                        {
                            return $.newTag("span").text(data.Name).css("color", data.Status == 0 ? "gray" : "black");
                        })
                        },
                        { content: Binding("Status", function (status, obj)
                        {
                            return $.newTag("span").text(status == 0 ? "" : "Online").css("color", status == "0" ? '' : 'green')
                        })
                        }
                    ],
                    onRowClicked: function (key, data)
                    {
                        var room = view.createRoomForUser(data);
                        if (room)
                            room.tab.select();
                    }
                });
            },
            createRoomForUser: function (user, roomId)
            {
                if (!user) return;
                var room = Enum(this._rooms).first(function (x) { return x.mainUserId == user.UserId; });
                if (!room)
                {
                    var roomView = new wing.chat.RoomView({ chatClient: this.chatClient() });
                    var localId = "chat-room-" + Util.uid();
                    var roomTab = this.tabs.addTabItem(localId, user.Name).select().add(roomView);
                    room = {
                        owner: this._user,
                        mainUserId: user.UserId,
                        view: roomView,
                        tab: roomTab,
                        localId: localId,
                        roomId: roomId || ''
                    };
                    roomView.room(room);
                    this._rooms.push(room);
                }
                else if (roomId)
                    room.roomId = roomId;
                return room;
            },
            updateUserStatus: function (user)
            {
                if (!this._contacts) return;
                var idx = Enum(this._contacts).indexOf(function (x) { return x.UserId == user.UserId; });
                if (idx > -1)
                    this._contacts[idx] = user;
                else
                    this._contacts.push(user);
                this.updateContactsView();
            },
            updateContactsList: function ()
            {
                this._contacts = this._chatClient.contacts();
                this.updateContactsView();
            },
            updateContactsView: function ()
            {
                this._contacts = Enum(this._contacts)
                    .orderBy(function (x) { return x.Status * -1; })
                    .thenBy(function (x) { return x.Name; })
                    .toArray();
                this.usersTable.dataBind(this._contacts);
            },
            routeMessage: function (data)
            {
                // tentar pelo id
                // user
                var users = data.To.split(';'), user = null, accept = false, senderObj;
                for (var i = 0, usr; (usr = users[i]) && (!user || !accept); i++)
                {
                    if (usr == this._user.UserId)
                        accept = true;
                    else
                        user = usr;
                }

                if (!accept || !user)
                    return;

                if (data.Sender == this._user.UserId)
                    senderObj = this._user;
                else
                {
                    senderObj = Enum(this._contacts).first(function (x) { return x.UserId == data.Sender; });
                    if (!senderObj)
                        return;
                }

                var room = Enum(this._rooms).first(function (x) { return x.roomId == data.RoomId });
                if (!room)
                {
                    var userObj = Enum(this._contacts).first(function (x) { return x.UserId == user; });
                    room = this.createRoomForUser(userObj, data.RoomId);
                }
                if (room)
                {
                    room.view.addMessage(data.Sender, senderObj.Name, data.Message);
                }
            }
        },
        properties: {
            chatClient: {
                get: function () { return this._chatClient; },
                set: function (value)
                {
                    if (value && this._chatClient)
                        throw "This property can be assigned only once";
                    this._chatClient = value;
                    var view = this;
                    if (value)
                    {
                        value.onSignedIn(function ()
                        {
                            view._user = view._chatClient._user;
                            view.updateContactsList();
                        });
                        value.onUserStatus(function (user) { view.updateUserStatus(user); });
                        value.onMessageArrive(function (data) { view.routeMessage(data); });
                    }
                }
            }
        }
    });
});

Loader.run("ChatClientRun", function ()
{
    var chatClient = new wing.chat.Client();
    chatClient.onSignedIn(function ()
    {
    });

    // ui
    var chatUi = new wing.chat.View();
    chatUi.chatClient(chatClient);

    // adicionar a ui ao window manager
    WindowManager.add(chatUi, true);

    chatClient.signIn();
});