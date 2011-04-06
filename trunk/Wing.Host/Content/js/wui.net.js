/*
* 
*  Routing system
*
*/
(function Wui(ctx)
{
    var wui = ctx.wui;
    if (!wui)
        return;

    var util = wui.Util;
    Type.define("wui.net.Router", null, {
        fields: {
            _routes: function () { return {}; },
            _methods: function () { return {}; },
            _activeRequests: 0,
        },
        methods: {
            initialize: function ()
            {
                this._methods["postJSON"] = function (url, params, callback)
                {
                    this._ajax({
                        url: url,
                        type: 'POST',
                        dataType: 'json',
                        data: JSON.stringify(params),
                        contentType: 'application/json; charset=utf-8',
                        success: callback
                    });
                };

                this._methods["getText"] = function(url, params, callback){
                    this._ajax({
                        url: url,
                        data: params,
                        success: callback,
                        dataType: "text"
                    });
                };

                this._methods["json"] = function(url, params, callback){
                    this._ajax({
                        url: url,
                        data: params,
                        success: callback,
                        dataType: "json"
                    });
                };

                this._methods["post"] = function(url, params, callback){
                    this._ajax({
                        type: 'POST',
                        url: url,
                        data: params,
                        success: callback
                    });
                };
            },
            addMethod: function(name, fn) {
                this._methods[name] = fn;
            },
            _ajaxRequestCompleted: function (jqXHR, status, params)
            {
                if (!params.terminated){
                    this._activeRequests--;
                    params.terminated = true;
                 }
            },
            _ajax: function (params)
            {
                var rpc = this, successCb = params.success, errorCb = params.error;
                params.error = function(jqXHR, status, error){
                    rpc._ajaxRequestCompleted(jqXHR, status, params);
                    if (errorCb)
                        errorCb(jqXHR, status);
                };
                params.success = function(data, status, jqXHR){
                    rpc._ajaxRequestCompleted(jqXHR, status, params);
                    if (successCb)
                        successCb(data, status, jqXHR);
                };
                this._activeRequests++;
                $.ajax(params);
            },
            route: function (name, method, urlPattern)
            {
                util.assertEmptyArg(name, "name");
                var route = ({ name: name, url: urlPattern || name, method: this._methods[method] });
                this._routes[name] = route;
                return route;
            },
            exec: function (routeNameOrUrl, params, callback, method)
            {
                // (routeNameOrUrl, callback) // call a route with no params
                if (util.isFunction(params)){
                    callback = params;
                    params = undefined;
                }
                // (routeNameOrUrl, params, method)
                if (typeof callback === "string"){
                    method = callback;
                    callback = undefined;
                }
                method = this._methods[method];
                var route = this._routes[routeNameOrUrl];
                var url = route !== undefined ? route.url : routeNameOrUrl;
                url = util.isFunction(url) ? url(params, route) : url;
                if (!method && (!(method = route.method)))
                    throw "Informe a o método que deseja usar para chamar a url: exec('http://...', 'json', [params],  [callback])";
                url = Util.format(url, params);
                method.call(this, url, params, callback);
            }
        }
    });


    ctx.Net = new wui.net.Router();
    wui.net.Router.current = ctx.Net;
})(window);

