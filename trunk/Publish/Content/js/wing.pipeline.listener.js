Loader.run("PipeListener", function ()
{
    wui.net.Router.type.extend({
        fields: {
            _listeners: null
        },
        methods: {
            startListener: function ()
            {
                // resetar os listeners
                this._listeners = {};

                var listener = this;
                window.addEventListener("message", function (e)
                {
                    var msg = e.data || "";
                    if (!msg.startsWith("@pipe-msg:"))
                        return;

                    var msg = msg.substr(10);
                    var idx = msg.indexOf(':')
                    var msgName = msg.substr(0, idx);
                    var callbacks;
                    if ((callbacks = listener._listeners[msgName]))
                    {
                        var msgData = JSON.parse(msg.substr(idx + 1));
                        for (var c in callbacks)
                        {
                            if (callbacks.hasOwnProperty(c))
                                callbacks[c](msgData);
                        }
                    }

                    //repassar a mensagem para as janelas filhas
                    for (var i = 0; i < window.frames.length; i++)
                    {
                        var cntWnd = (window.frames[i].contentWindow || window.frames[i]);
                        if (cntWnd)
                            cntWnd.postMessage(e.data, "*");
                    }
                }, false);
            },
            listen: function (messageId, callback)
            {
                var msgListeners = this._listeners[messageId];
                if (!msgListeners)
                    this._listeners[messageId] = msgListeners = {};
                var token = Util.uid();
                msgListeners[token] = callback;
            },
            stopListening: function (messageId, token)
            {
                var msgListeners = this._listeners[messageId];
                if (msgListeners)
                    delete msgListeners[token];
            }
        }
    });

    Loader.run("StartPipeListener", function () { Net.startListener(); });
});