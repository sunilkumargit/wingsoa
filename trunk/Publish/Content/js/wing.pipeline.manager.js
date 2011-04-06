Loader.run("PipeLoader", function ()
{
    wui.net.Router.type.extend({
        fields: {
            active: false,
            interval: 2000,
            $timerId: 0,
            postUrl: 'pipeline.sync',
            $requestIdCnt: 0,
            $opcallCnt: 0,
            $opcallbacks: null,
            $sendBuffer: null,
            $lastMsgSeq: 0,
            $inTransaction: false,
            $currentPack: null
        },
        methods: {
            startPipeline: function ()
            {
                if (this.active)
                    return;
                var router = this;
                this.$sendBuffer = [];
                this.active = true;
                this.$timerId = setInterval(function ()
                {
                    if (!router.$inTransaction && router.active)
                        router._syncPipeline();
                }, this.interval);
            },

            stopPipeline: function ()
            {
                this.active = false;
                clearInterval(this.$timerId);
                this.$timerId = 0;
            },

            scheduleOperation: function (operation, data, callback)
            {
                this.$sendBuffer.push({
                    opid: (++this.$opcallCnt).toString(),
                    type: 'opcall',
                    opname: operation,
                    data: JSON.stringify(data),
                    callback: callback
                });
            },

            _receivePack: function (result)
            {
                try
                {
                    if (result && result.id)
                    {
                        while (result.data.length)
                        {
                            var item = result.data.shift();
                            if (item.type === 'opresult')
                            {
                                var callbackItem = this.$opcallbacks[item.opid];
                                if (callbackItem)
                                {
                                    try
                                    {
                                        item.data = eval(item.data);
                                        callbackItem.callback(item.data);
                                    }
                                    catch (err) { }
                                }
                            }
                            else if (item.type === 'msg')
                            {
                                this.$lastMsgSeq = Math.max(parseInt(item.opid), this.$lastMsgSeq);
                                var msg = "@pipe-msg:" + item.opname + ":" + item.data;
                                window.postMessage(msg, "*");
                            }
                        }
                    }
                }
                finally
                {
                    this.$inTransaction = false;
                    this.$currentPack = null; // resetar o pack corrente, assim outro será criado.   
                }
            },

            _syncPipeline: function ()
            {
                if (this.$inTransaction || !this.postUrl)
                    return;
                this.$inTransaction = true;
                var pack = this.$currentPack;
                if (!pack)
                {
                    this.$opcallbacks = {};
                    //criar um pacote de dados 
                    pack = {
                        id: ++this.$requestIdCnt,
                        data: [],
                        lastMsgSeq: this.$lastMsgSeq
                    };
                    while (this.$sendBuffer.length)
                    {
                        var item = this.$sendBuffer.shift();
                        pack.data.push(item);
                        if (item.type === 'opcall')
                            this.$opcallbacks[item.opid] = item;
                    }
                    this.$currentPack = pack;
                }

                var pipeline = this;
                Net.exec(this.postUrl, pack, function (result)
                {
                    pipeline._receivePack(result);
                });
            }
        }
    });

    //adicionar o metodo 'pipeline'
    Net.addMethod("pipeline", function (operation, params, callback)
    {
        Net.scheduleOperation(operation, params, callback);
    });

    // roteamento da chamada ao pipeline
    Net.route("pipeline.sync", "postJSON", "/Shell/SyncPipe");

    Loader.run("PipeStarter", function ()
    {
        // iniciar o pipeline
        Net.startPipeline();
    });
});