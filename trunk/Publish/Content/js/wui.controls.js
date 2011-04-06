/*
*
*
*  Controls system
*
*/
(function WuiUI(ctx)
{
    var wui = ctx.wui;
    var util = wui.Util;

    // extender o jquery
    $.fn.wuiControl = function (data)
    {
        if (!this.length)
            return undefined;
        else if (this.length == 1)
            return $(this).data("_wui_control_", data);

        var result = [];
        for (var i = 0, el; (el = this[i]); i++)
            result[i] = $(el).data("_wui_control_");
        return result;
    };

    $.fn.newTag = function (name, attr, css)
    {
        return $.newTag(name, attr, css).appendTo(this[0]);
    };

    $.fn.addTag = function (name, attr, css)
    {
        $.newTag(name, attr, css).appendTo(this[0]);
        return this;
    };

    $.fn.clearFix = function ()
    {
        $.newTag("div", { "class": "ui-helper-clearfix" }).appendTo(this[0]);
        return this;
    };

    $.newTag = function (tagname, attr, css)
    {
        var result = $("<" + tagname + ">" + "</" + tagname + ">");
        if (attr)
            result.attr(attr);
        if (css)
            result.css(css);
        return result;
    };

    // constants de alinhamento
    ctx.HALIGN = {
        STRETCH: "stretch_h",
        LEFT: "left",
        RIGHT: "right",
        CENTER: "center_h",
        NONE: "none_h"
    };

    ctx.VALIGN = {
        STRETCH: "stretch_v",
        TOP: "top",
        BOTTOM: "bottom",
        CENTER: "center_h",
        NONE: "none_v"
    };

    ctx.DATA_WUI_TYPE_FIRST = "[wui-data-type]:first";
    ctx.DATA_WUI_TYPE = "[wui-data-type]";

    Type.define("wui.Control", null, {
        fields: {
            tagName: "div",
            isUiControl: true
        },
        methods: {
            initialize: function (options)
            {
                this.base();
                var attr = { "data-wui-type": this.type.fullname, "data-wui-iid": this.iid };
                if (!this.el)
                    this.el = $.newTag(this.tagName, attr);
                else
                    this.el.attr(attr);
                this.el.wuiControl(this);
                this.el.addClass("ui-widget wui-control");
                this.init();
                if (options)
                    this.set(options);
                this.update();
            },
            init: function () { },
            css: function (cssProperty, value)
            {
                this.el.css(cssProperty, value);
                return this;
            },
            "$": function (selector)
            {
                return this.el.find(selector);
            },
            getInstanceSelector: function ()
            {
                return "[data-wui-iid=" + this.iid + "]";
            },
            addClass: function (className)
            {
                this.el.addClass(className);
                return this;
            },
            removeClass: function (className)
            {
                this.el.removeClass(className);
                return this;
            },
            update: function ()
            {
                this.dispatch("update", this.updateUi);
            },
            updateUi: function () { },
            dispatch: function (key, callback)
            {
                var dispatchs = (this._p.dispatchs = this._p.dispatchs || {}), instance = this;
                if (dispatchs[key]) return;
                dispatchs[key] = true;
                setTimeout(function ()
                {
                    try
                    {
                        dispatchs[key] = false;
                        callback.apply(instance);
                    }
                    catch (err)
                    {
                        throw "Error on process dispatch of key " + key + " of " + instance.getInstanceInfo() + ": " + (err || "").toString();
                    }
                }, 1);
                return this;
            },
            dispose: function ()
            {
                this.detach();
                this.el.remove();
                this.p = null;
            },
            detach: function (detaching)
            {
                if (!detaching)
                {
                    var parentContainer = this.el.parents(".wui-container:first").wuiControl();
                    if (parentContainer)
                        parentContainer.removeItem(this);
                }
                else
                    this.el.detach();
                return this;
            },
            hide: function ()
            {
                return this.visible(false);
            },
            show: function ()
            {
                return this.visible(true);
            },
            stretch: function ()
            {
                return this.vAlign(VALIGN.STRETCH).hAlign(HALIGN.STRETCH);
            },
            clearAlign: function ()
            {
                return this.clearVertAlign().clearHorzAlign();
            },
            clearVertAlign: function ()
            {
                this.el.removeClass("op-align-stretch-v op-align-center-v op-align-top op-align-bottom ");
                return this;
            },
            clearHorzAlign: function ()
            {
                this.el.removeClass("op-align-stretch-h op-align-center-h op-align-left op-align-right");
                return this;
            },
            notifyChildren: function (msgId, msgParam)
            {
                var items = util.getArray(this.$("[wcmsg-" + msgId + "]").wuiControl());
                if (!items || !items.length)
                    return;
                for (var i = 0, item; (item = items[i]); i++)
                    if (item.processMessage(msgId, msgParam) == false)
                        return;
                return this;
            },
            notifyParents: function (msgId, msgParam)
            {
                var items = util.getArray(this.el.parents("[wcmsg-" + msgId + "]").wuiControl());
                if (!items || !items.length)
                    return;
                for (var i = 0, item; (item = items[i]); i++)
                    if (item.processMessage(msgId, msgParam) == false)
                        return;
                return this;
            },
            acceptMessage: function (msgId, callback)
            {
                this._msgCallbacks = this._msgCallbacks || {};
                this._msgCallbacks[msgId] = callback;
                this.el.attr("wcmsg-" + msgId, "1");
                return this;
            },
            rejectMessage: function (msgId)
            {
                if (!this._msgCallbacks) return;
                delete this._msgCallbacks[msgId];
                return this;
            },
            processMessage: function (msgId, msgParam)
            {
                if (!this._msgCallbacks)
                    return;
                var callback = this._msgCallbacks[msgId];
                if (callback)
                    return callback.call(this, msgParam, msgId) !== false;
                return true;
            },
            addTo: function (control)
            {
                if (!control) return;
                control.add(this);
                return this;
            },
            processTemplate: function (template, data)
            {
                var templ = template;
                if (util.isFunction(templ))
                    templ = templ(data);
                if (templ)
                {
                    result = '';
                    if (templ.isUiControl)
                    {
                        templ.dataBind(data);
                        return templ.el;
                    }
                    else if (typeof templ.jquery == "string")
                        return templ;
                    else
                        return $.newTag("span").text(templ.toString());
                }
            },
            scrollToBottom: function ()
            {
                this.el.scrollTop(1000000);
            }

        },
        properties: {
            id: {
                get: function () { return (this.___id = this.el.attr('id')); },
                set: function (value) { this.el.attr('id', value); this.___id = value; }
            },
            parent: {
                get: function ()
                {
                    var p = this.el.parents(DATA_WUI_TYPE_FIRST).wuiControl();
                    if (p && !p.isUiControl)
                        return undefined;
                    return p;
                },
                set: function () { }
            },
            color: {
                get: function () { return this.el.css("color"); },
                set: function (value) { this.el.css("color", value); }
            },
            bgColor: {
                get: function () { return this.el.css("background-color"); },
                set: function (value) { this.el.css("background-color", value); }
            },
            fontWeight: {
                get: function () { return this.el.css("font-weight"); },
                set: function (value) { this.el.css("font-weight", value); }
            },
            fontSize: {
                get: function () { return this.el.css("font-size"); },
                set: function (value) { this.el.css("font-size", value); }
            },
            fontFamily: {
                get: function () { return this.el.css("font-family"); },
                set: function (value) { this.el.css("font-family", value); }
            },
            width: {
                get: function () { return this.el.css("width"); },
                set: function (value) { this.el.css("width", value); }
            },
            height: {
                get: function () { return this.el.css("height"); },
                set: function (value) { this.el.css("height", value); }
            },
            top: {
                get: function () { return this.el.css("top"); },
                set: function (value) { this.el.css("top", value); }
            },
            left: {
                get: function () { return this.el.css("left"); },
                set: function (value) { this.el.css("left", value); }
            },
            right: {
                get: function () { return this.el.css("right"); },
                set: function (value) { this.el.css("right", value); }
            },
            bottom: {
                get: function () { return this.el.css("bottom"); },
                set: function (value) { this.el.css("bottom", value); }
            },
            marginBottom: {
                get: function () { return this.el.css("margin-bottom"); },
                set: function (value) { this.el.css("margin-bottom", value); }
            },
            marginRight: {
                get: function () { return this.el.css("margin-right"); },
                set: function (value) { this.el.css("margin-right", value); }
            },
            marginTop: {
                get: function () { return this.el.css("margin-top"); },
                set: function (value) { this.el.css("margin-top", value); }
            },
            marginLeft: {
                get: function () { return this.el.css("margin-left"); },
                set: function (value) { this.el.css("margin-left", value); }
            },
            margin: {
                get: function () { return this.el.css("margin"); },
                set: function (value)
                {
                    if (typeof value !== "string")
                    {
                        if (util.isArray(value))
                        {
                            if (value[0] !== undefined) this.marginTop(value[0]);
                            if (value[1] !== undefined) this.marginRight(value[1]);
                            if (value[2] !== undefined) this.marginBottom(value[2]);
                            if (value[3] !== undefined) this.marginRight(value[3]);
                        }
                        else
                        {
                            if (value.top) this.marginTop(value.top);
                            if (value.right) this.marginRight(value.right);
                            if (value.bottom) this.marginBottom(value.bottom);
                            if (value.left) this.marginLeft(value.left);
                        }
                    }
                }
            },
            visible: {
                get: function ()
                {
                    return this.el.is(":visible");
                },
                set: function (value)
                {
                    var parentContainer = this.el.parents(".wui-container:first").wuiControl();
                    if (!value)
                    {
                        if (parentContainer)
                            parentContainer.hideItem(this);
                        else
                            this.el.addClass("op-hidden");
                    }
                    else
                    {
                        if (parentContainer)
                            parentContainer.showItem(this);
                        else
                        {
                            if (this.el.css("display") === "none")
                                this.el.css("display", "");
                            this.el.removeClass("op-hidden");
                        }
                    }
                }
            },
            hAlign: {
                get: function ()
                {
                    if (this.el.hasClass("op-align-stretch-h"))
                        return HALIGN.STRETCH;
                    if (this.el.hasClass("op-align-left"))
                        return HALIGN.LEFT;
                    if (this.el.hasClass("op-align-right"))
                        return HALIGN.RIGHT;
                    if (this.el.hasClass("op-align-center-h"))
                        return HALIGN.CENTER;
                    return HALIGN.NONE;
                },
                set: function (value)
                {
                    this.clearHorzAlign();
                    switch (value)
                    {
                        case HALIGN.STRETCH: this.el.addClass("op-align-stretch-h"); break;
                        case HALIGN.LEFT: this.el.addClass("op-align-left"); break;
                        case HALIGN.RIGHT: this.el.addClass("op-align-right"); break;
                        case HALIGN.CENTER: this.el.addClass("op-align-center-h"); break;
                    }
                }
            },
            vAlign: {
                get: function ()
                {
                    if (this.el.hasClass("op-align-stretch-v"))
                        return VALIGN.STRETCH;
                    if (this.el.hasClass("op-align-top"))
                        return VALIGN.TOP;
                    if (this.el.hasClass("op-align-bottom"))
                        return VALIGN.BOTTOM;
                    if (this.el.hasClass("op-align-center-v"))
                        return VALIGN.CENTER;
                    return VALIGN.NONE;
                },
                set: function (value)
                {
                    this.clearVertAlign();
                    switch (value)
                    {
                        case VALIGN.STRETCH: this.el.addClass("op-align-stretch-v"); break;
                        case VALIGN.TOP: this.el.addClass("op-align-top"); break;
                        case VALIGN.BOTTOM: this.el.addClass("op-align-bottom"); break;
                        case VALIGN.CENTER: this.el.addClass("op-align-center-v"); break;
                    }
                }
            },
            vScroll: {
                get: function () { return this.el.css("overflow-y") == "scroll"; },
                set: function (value) { this.el.css("overflow-y", value === true ? "scroll" : ""); }
            }
        }
    });

    var CONTROL_UPDATE_UI_MSG = "control-update-ui";

    Type.define("wui.Container", "wui.Control", {
        fields: {
        },
        methods: {
            init: function ()
            {
                this.base();
                this.el.addClass("wui-container");
            },
            set: function (options)
            {
                if (!options) return;
                var children = null;
                if (options.children)
                {
                    children = util.getArray(options.children).slice(0);
                    delete options["children"];
                }
                this.base(options);
                if (children)
                {
                    while (children.length)
                        this.add(children.shift());
                }
            },
            add: function () // args - controls
            {
                for (var i = 0; i < arguments.length; i++)
                {
                    var arg = arguments[i];
                    if (arg)
                        this.addItem(arg);
                }
                return this;
            },
            remove: function (args)
            {
                for (var i = 0; i < arguments.length; i++)
                {
                    var arg = arguments[i];
                    if (arg)
                    {
                        arg = this.getChild(arg);
                        if (arg)
                            removeItem(arg);
                    }
                }
                return this;
            },
            addItem: function (item)
            {
                this.el.append(item.el);
            },
            removeItem: function (item)
            {
                item.detach(true);
            },
            hideItem: function (item)
            {
                item.el.addClass("op-hidden");
            },
            showItem: function (item)
            {
                if (item.el.css("display") === "none")
                    item.el.css("display", "");
                item.el.removeClass("op-hidden");
            },
            getChildren: function ()
            {
                return util.getArray(this.el.children(DATA_WUI_TYPE).wuiControl());
            },
            getChild: function (id)
            {
                if (!id) return undefined;
                else if (id.isUIControl) return id;
                else if (typeof id === "string") return this.el.children("#" + id + DATA_WUI_TYPE_FIRST).wuiControl();
                else if (util.isArray(id)) return Enum(this.getChildren()).first(id);
                return this.getChildren()[id];
            }
        }
    });

    Type.define("wui.ViewPart", "wui.Control", {
        methods: {
            init: function ()
            {
                this.base();
                this.stretch();
            }
        },
        properties: {
            content: {
                get: function () { return this.el.find(DATA_WUI_TYPE_FIRST).wuiControl(); },
                set: function (value)
                {
                    var curr = this.content();
                    if (curr)
                        curr.detach();
                    if (value)
                        this.el.append(value.el);
                }
            }
        }
    });

    Type.define("wui.TextBlock", "wui.Control", {
        methods: {
            initialize: function (options)
            {
                this.tagName = "span";
                this.base(options);
                this.addClass("wui-text-block");
            },
            set: function (options)
            {
                if (typeof options === "string")
                    options = { text: options };
                this.base(options);
            }
        },
        properties: {
            text: {
                get: function () { return this.el.text(); },
                set: function (value) { this.el.text(value); }
            }
        }
    });

    wui.Container.extend().method("addText", function (options) { new wui.TextBlock(options).addTo(this); return this; });
    wui.Container.extend().method("newText", function (options) { return new wui.TextBlock(options).addTo(this); });

    Type.define("wui.Grid", "wui.Container", {
        fields: {
            _cell: null,
            _rows: null,
            _cols: null,
            _colCount: 0,
            _rowCount: 0,
            _cellTemplate: "<div class='wui-grid-cell'><div class='wui-grid-cell-content op-overflow-hidden'></div><div class='op-clear-fix'></div></div>"
        },
        methods: {
            init: function ()
            {
                this.base();
                this.el.addClass("wui-grid");
                this.stretch();
                this._cells = {};
                this._rows = [];
                this._cols = [];

                this.acceptMessage(GRID_CHILD_POS_CHANGED_MSG, function (child)
                {
                    this.update();
                    return false; // não continuar propagando a mensagem
                });
                this.acceptMessage(CONTROL_UPDATE_UI_MSG, function ()
                {
                    this.update();
                    return false; // a grid ira propagar a mensagem depois.
                });
            },
            _updateCells: function ()
            {
                var grid = this;
                for (var i = 0; i < grid._colCount; i++)
                {
                    for (var j = 0; j < grid._rowCount; j++)
                    {
                        var key = i + "," + j;
                        var cell = grid._cells[key];
                        if (!cell)
                        {
                            var el = $(grid._cellTemplate).appendTo(grid.el);
                            grid._cells[key] = cell = {
                                key: key,
                                el: el,
                                content: el.find(".wui-grid-cell-content:first")
                            };
                            grid._rows[j].cells.push(cell);
                            grid._cols[i].cells.push(cell);
                        }
                    }
                }
            },
            updateUi: function ()
            {
                var grid = this;
                this._updateCells();

                // update rows and cols sizes
                grid._applyMeasures(grid._rows, "height", grid.el.innerHeight());
                grid._applyMeasures(grid._cols, "width", grid.el.innerWidth());

                //position
                grid._applyPositions(grid._rows, grid._cols, "top", "height");
                grid._applyPositions(grid._cols, grid._rows, "left", "width");

                var children = this.getChildren();
                for (var i = 0, childItem; (childItem = children[i]); i++)
                {
                    var p = child._p;
                    if (_p.currGridRow !== _p.gridRow || _p.currGridCol !== _p.gridCol)
                    {
                        if (p.currGridRow > -1)
                            child.el.detach();
                        key = p.gridCol + "," + p.gridRow;
                        cell = grid._cells[key];
                        if (cell)
                        {
                            cell.content.append(childItem.el);
                            p.currGridCol = p.gridCol;
                            p.currGridRow = p.gridRow;
                        }
                    }
                }
                this.notifyChildren(CONTROL_UPDATE_UI_MSG);
            },
            _setRowsOrCols: function (store, args)
            {
                if (util.isArray(args[0]))
                    args = args[0];
                for (var i = 0, arg; (arg = args[i]); i++)
                {
                    var item = store[i];
                    if (!item)
                        store[i] = item = { index: i, cells: [], size: DOMHelper.getUnit(arg) };
                    else
                        item.size = DOMHelper.getUnit(arg);
                }
            },
            _applyMeasures: function (items, measure, max)
            {
                var left = max;
                // calcular o tamanho de cada item
                for (var pass = 1; pass <= 2; pass++)
                {
                    for (var i = 0, item; (item = items[i]); i++)
                    {
                        if ((pass == 1 && item.size.unit == "*") ||
                            (pass == 2 && item.size.unit !== "*"))
                            continue;
                        //calcular o tamanho
                        var m = item[measure] = DOMHelper.calculatePixels(item.size.value, item.size.unit, max, left);
                        if (pass == 1) left -= m;

                        for (var c = 0, cell; (cell = item.cells[c]); c++)
                            cell.el.css(measure, m + "px");
                    }
                }
            },
            _applyPositions: function (source, mirror, coord, measure)
            {
                var sum = 0;
                for (var i = 0, item; (item = source[i]); i++)
                {
                    for (var c = 0, cell; (cell = item.cells[c]); c++)
                        cell.el.css(coord, sum + "px");
                    sum += item[measure];
                }
            },
            add: function (col, row, item)
            {
                if (col && col.isUiControl)
                {
                    item = col;
                    col = row = undefined;
                }
                if (col == undefined)
                    col = item.gridCol();
                if (row == undefined)
                    row = item.gridRow();
                var cell = this._cells[col + "," + row];
                if (cell)
                {
                    item._p.currGridCol = item._p.gridCol = col;
                    item._p.currGridRow = item._p.gridRow = row;
                    cell.content.append(item.el);
                    this.update();
                }
                else
                    throw "Grid cell not found: " + col + "," + row;
                return this;
            },
            setRows: function ()
            {
                this._setRowsOrCols(this._rows, arguments);
                this._rowCount = this._rows.length;
                this._updateCells();
                return this;
            },
            setCols: function ()
            {
                this._setRowsOrCols(this._cols, arguments);
                this._colCount = this._cols.length;
                this._updateCells();
                return this;
            },
            getChildren: function ()
            {
                return util.getArray(this.el.children(".wui-grid-cell")
                        .children(".wui-grid-cell-content")
                        .children(DATA_WUI_TYPE)
                        .wuiControl());
            }
        },
        properties: {
            rowCount: {
                "get": function () { return this._rowCount; },
                "set": function (value)
                {
                    this._rowCount = value;
                    this._updateCells();
                }
            },
            colCount: {
                "get": function () { return this._colCount; },
                "set": function (value)
                {
                    this._colCount = value;
                    this._updateCells();
                }
            }
        }
    });

    var GRID_CHILD_POS_CHANGED_MSG = "grid-child-pos-changed";

    // extender o wui.Control para acomodar os metodos aux. do grid
    wui.Control.type.extend({
        methods: {
            gridCoord: function (x, y) { this.gridCol(x); this.gridRow(y); return this; }
        },
        properties: {
            gridRow: {
                get: function () { return this._p.gridRow || 0; },
                set: function (value)
                {
                    this._p.gridRow = value;
                    this.notifyParents(GRID_CHILD_POS_CHANGED_MSG, this);
                }
            },
            gridCol: {
                get: function () { return this._p.gridCol || 0; },
                set: function (value)
                {
                    this._p.gridCol = value;
                    this.notifyParents(GRID_CHILD_POS_CHANGED_MSG, this);
                }
            }
        }
    });

    // extender o container para adicionar um atalho para criar um grid
    wui.Container.extend().method("newGrid", function (options) { return (new wui.Grid(options)).addTo(this); });
    wui.Container.extend().method("addGrid", function (options) { return (new wui.Grid(options)).addTo(this); return this; });

    ctx.ORIENTATION = {};
    ctx.ORIENTATION.V = ORIENTATION.VERTICAL = "orientation_vertical";
    ctx.ORIENTATION.H = ORIENTATION.HORIZONTAL = "orientation_horizontal";

    Type.define("wui.StackPanel", "wui.Container", {
        methods: {
            init: function ()
            {
                this.base();
                this._contentHolder = $.newTag("div").addClass("wui-stackpanel-content");
                this.stretch();
                this._contentHolder.appendTo(this.el);
                this._orientation = ORIENTATION.VERTICAL;
                this.el.addClass("wui-stackpanel");
            },
            addItem: function (item)
            {
                this._contentHolder.newTag("div", { class: "wui-stackpanel-item" })
                    .append(item.el)
                    .clearFix();
            },
            removeItem: function (item)
            {
                var holder = item.el.parent();
                item.detach();
                holder.remove();
            },
            getChildren: function ()
            {
                return util.getArray(this._contentHolder.children().children(DATA_WUI_TYPE).wuiControl());
            },
            showItem: function (item)
            {
                item.el.parent().removeClass("op-hidden");
            },
            hideItem: function (item)
            {
                item.el.parent().addClass("op-hidden");
            },
            updateUi: function ()
            {
                if (this._orientation === ORIENTATION.HORIZONTAL)
                {
                    this.el.removeClass("wui-sp-v-layout");
                    this.el.addClass("wui-sp-h-layout");
                }
                else
                {
                    this.el.removeClass("wui-sp-h-layout");
                    this.el.addClass("wui-sp-v-layout");
                }
            }
        },
        properties: {
            orientation: {
                get: function () { return this._orientation; },
                set: function (value)
                {
                    this._orientation = value;
                    this.update();
                }
            }
        }
    });

    wui.Container.extend().method("newStackPanel", function (options) { return new wui.StackPanel(options).addTo(this); });
    wui.Container.extend().method("addStackPanel", function (options) { return new wui.StackPanel(options).addTo(this); return this; });

    Type.define("wui.WindowManager", "wui.Container", {
        fields: {
            _history: null,
            _views: null,
            _blockCount: 0
        },
        methods: {
            init: function ()
            {
                this.base();
                var manager = this;
                this._history = [];
                this._views = {};
                this.stretch();
                this.el.addClass("wui-window-mngr");
                this.el.appendTo("body");
                this._loadingBox = $("body").newTag("div", { class: "wui-overlay-box" }).hide();
                this._loadingBox.addTag("div", { class: "wui-overlay-content ui-corner-all" });
                var container = this;

                $(function ()
                {
                    $("body").click(function ()
                    {
                        manager._checkContextMenuState();
                    });

                    $(window).resize(function ()
                    {
                        container.update();
                    });
                });
            },
            updateUi: function ()
            {
                //forçar o update das posições absolutas.
                this.el.removeClass("wui-window-mngr").addClass("wui-window-mngr");
                this.triggerWindowResize();
                this.notifyChildren(CONTROL_UPDATE_UI_MSG);
            },
            add: function (view, show)
            {
                this.base(view);
                if (show)
                    this.showView(view.id());
            },
            showView: function (id)
            {
                var view = this.getChild(id);
                if (view)
                    view.show();
            },
            showItem: function (view)
            {
                var id = view.id();
                if (id !== this.current)
                {
                    if (this.current)
                    {
                        var currView = this.getChild(this.current);
                        var mgr = this;
                        currView.el.fadeOut("normal", function ()
                        {
                            this.current = undefined;
                            mgr.showItem(view);
                        });
                    }
                    else
                    {
                        Enum(this.getChildren()).each(function (item)
                        {
                            if (item.id() === id)
                                item.el.parent().show();
                            else
                                item.el.parent().hide();
                        });
                        this.current = id;
                    }
                }
            },
            addItem: function (item)
            {
                this.el.newTag("div", { "class": "wui-window-mngr-view op-fill-parent op-overflow-hidden" }).append(item.el);
            },
            removeItem: function (item)
            {
                var holder = item.parent();
                item.detach(true);
                holder.remove();
            },
            windowHeight: function ()
            {
                return this.el.innerHeight();
            },
            windowWidth: function ()
            {
                return this.el.innerWidth();
            },
            block: function ()
            {
                if (this._blockCount == 0)
                    this._loadingBox.fadeIn();
                this._blockCount++;

                return this;
            },
            unblock: function ()
            {
                if (this._blockCount == 0)
                    return;
                this._blockCount--;
                if (this._blockCount == 0)
                    this._loadingBox.fadeOut();

                return this;
            },
            showContextMenu: function (options)
            {
                if (!options.ctxWnd)
                {
                    options.ctxWnd = this;
                    var relativeTo = options.relativeTo;
                    relativeTo = relativeTo || (relativeTo.IsUiControl ? relativeTo.el : $(relativeTo));
                    if (relativeTo)
                        options.relativeTo = DOMHelper.getMetrics(relativeTo);
                }
                var parent = util.getFromParentWindow("WindowManager");
                if (parent)
                    parent.showContextMenu(options);
                else
                {
                    if (this._contextWindowVisible)
                        this._closeContextWindow();
                    var elements = $.newTag("div", { class: "ui-corner-all wui-context-menu op-box-shadow" })
                        .addTag("div", { class: "wui-context-menu-bg" });
                    var items = elements.newTag("ul", { class: "ui-widget-content" });
                    var menu = options.menu;
                    for (var i = 0, mItem; (mItem = menu.items[i]); i++)
                    {
                        var item = items.newTag("li", { class: "ui-state-default wui-context-menu-item" });
                        item.newTag("span").addClass("wui-context-menu-icon").addClass(mItem.iconClass);
                        item.newTag("span", { class: "wui-context-menu-text" }).text(mItem.text);
                    }
                    this._showContextWindow({ content: elements, relativeTo: options.relativeTo });
                }
            },
            _showContextWindow: function (options)
            {
                // reset element
                var w = this._contextWindow
                    || (this._contextWindow = $("body").newTag("div", { class: "ui-widget wui-context-window" }));

                this._contextWindowVisible = true;
                w.css("visibility", "hidden")
                 .css("display", "block")
                 .css("top", "")
                 .css("left", "")
                 .css("right", "")
                 .css("bottom", "")
                 .css("min-width", "")
                 .css("width", options.width || "")
                 .css("height", options.height || "");
                if (options.content)
                    w.append(options.content);
                this._currentCtxWndOptions = options;

                this.dispatch("updCtxWnd", this._posCtxWindow);
            },
            _posCtxWindow: function ()
            {
                var options = this._currentCtxWndOptions;
                if (!options) return;
                var el = this._contextWindow;
                if (options.relativeTo)
                {
                    var winMetrics = DOMHelper.getMetrics(this.el);
                    var elMetrics = DOMHelper.getMetrics(el);
                    var relMetrics = DOMHelper.getMetrics(options.relativeTo);

                    //bottom
                    var relMaxY = relMetrics.offsetTop + relMetrics.height;
                    if (relMaxY + elMetrics.height < winMetrics.height)
                        el.css("top", relMaxY + 1);
                    else
                        el.css("bottom", relMetrics.offsetTop - 1);

                    var left = relMetrics.offsetLeft + elMetrics.width;
                    if (left > winMetrics.width)
                        left = winMetrics.width - 3;
                    el.css("left", left - elMetrics.width);
                    el.css("min-width", relMetrics.width);
                }
                else
                    el.css("min-width", "");
                el.css("visibility", "visible");
            },
            _closeContextWindow: function ()
            {
                this._contextWindowVisible = false;
                var w = this._contextWindow;
                if (!w) return;
                w.css("display", "none").children().remove();
                var options = this._currentCtxWndOptions;
                if (!options) return;
            },
            _checkContextMenuState: function ()
            {
                if (this._contextWindowVisible)
                    this._closeContextWindow();
                else
                {
                    var parent = util.getFromParentWindow("WindowManager");
                    if (parent)
                        parent._checkContextMenuState();
                }
            }
        },
        events: {
            windowResize: null
        }
    });

    // jquery para o menu de contexto
    $(".wui-context-menu .wui-context-menu-item *").live("mouseenter", function ()
    {
        $(this).parents(".wui-context-menu-item:first").removeClass("ui-state-default").addClass("ui-state-hover");
    }).
    live("mouseout", function ()
    {
        $(this).parents(".wui-context-menu-item:first").removeClass("ui-state-hover").addClass("ui-state-default");
    });

    Type.define("wui.TabItem", "wui.Container", {
        methods: {
            init: function (options)
            {
                this.base();
                this.el.addClass("wui-tab-item");
                this.id("w-tab-item-" + Util.uid());
            },
            select: function ()
            {
                this.dispatch("select", this._select);
                return this;
            },
            _select: function ()
            {
                var tabs = this.el.parents(".wui-tabs:first");
                if (tabs.length)
                    tabs.tabs("select", "#" + this.id());
            }
        },
        properties: {
            index: {
                get: function ()
                {
                    return this.el.index();
                },
                set: function (value)
                {
                    var header = this.el.parent().find('.wui-tabs-header:first');
                    var curr = header.children().eq(value);
                    if (curr.length)
                    {
                        var self = header.find("[href=#" + this.id() + "]").parent();
                        self.detach(true);
                        self.insertBefore(curr);
                    }
                }
            }
        }
    });

    $(".wui-tabs-header span.ui-icon-close").live("click", function ()
    {
        var header = $(this).parents(".wui-tabs-header:first");
        var index = $("li", header).index($(this).parent());
        header.parent().tabs("remove", index);
    });


    Type.define("wui.Tabs", "wui.Control", {
        fields: {
            _tabsHolder: null
        },
        methods: {
            init: function ()
            {
                this.base();
                this.stretch();
                this.el.addClass("wui-tabs");
                this._header = $.newTag("ul", { class: "wui-tabs-header" });
                this.el.append(this._header);
                var tab = this;
                this.el.tabs({
                    tabTemplate: "<li><a href='#{href}'>#{label}</a> <span class='ui-icon ui-icon-close' title='Fechar'>Fechar</span></li>",
                    select: function (event, ui)
                    {
                        $(ui.panel).wuiControl().notifyChildren(CONTROL_UPDATE_UI_MSG);
                    }
                });
            },
            addTabItem: function (name, label, position)
            {
                //criar o elemento
                var tab = new wui.TabItem();
                this.el.append(tab.el);
                this.el.tabs('add', '#' + tab.id(), label, position);
                return tab;
            }
        },
        properties: {
            selectedIndex: {
                get: function () { return this._header.find(".ui-tabs-selected:first").index(); },
                set: function (value) { this.el.tabs('select', value); }
            }
        }
    });

    Type.define("wui.Toolbar", "wui.Container", {
        fields: {
        },
        methods: {
            init: function (options)
            {
                this.base();
                this.el.addClass("ui-widget-header wui-toolbar");
                this._container = this.el.newTag("div", { class: "wui-toolbar-items" }).clearFix();
                this.el.clearFix();
                this.hAlign(HALIGN.STRETCH);
                this.vAlign(VALIGN.TOP);
            },
            createGroup: function (group, alignRight)
            {
                var groupHolder = $.newTag("div", {
                    "data-tb-group": group,
                    "class": "wui-toolbar-group"
                }).clearFix().insertBefore(this._container.children(":last"));
                if (alignRight)
                    groupHolder.addClass("wui-toolbar-group-align-right");
                return groupHolder;
            },
            add: function (control, group, position)
            {
                if (!group)
                    group = "Default";
                var groupHolder = this._container.find("[data-tb-group=" + group + "]");
                if (!groupHolder.length)
                    groupHolder = this.createGroup(group);

                $.newTag("div", { "class": "wui-toolbar-group-item" })
                        .append(control.el)
                        .clearFix()
                        .insertBefore(groupHolder.children(":last"));
            },
            removeItem: function (item)
            {
                var parent = item.el.parent();
                item.detach(true);
                parent.remove();
            },
            showItem: function (item)
            {
                item.el.parent().removeClass("op-hidden");
            },
            hideItem: function (item)
            {
                item.el.parent().addClass("op-hidden");
            }
        }
    });

    wui.Container.extend().method("newToolbar", function (options) { return new wui.Toolbar(options).addTo(this); });
    wui.Container.extend().method("addToolbar", function (options) { return new wui.Toolbar(options).addTo(this); return this; });

    Type.define("wui.Button", "wui.Control", {
        fields: {
            _isChecked: false,
            _isToggle: false
        },
        methods: {
            init: function ()
            {
                this.base();
                this._icons = { primary: null, secondary: null };
                this.el.addClass("wui-button");
                this.el.button({ icons: this._icons });
                var button = this;
                this.el.click(function (e)
                {
                    if (button._isToggle)
                        button._isChecked = !button._isChecked;
                    button.triggerClick(e);
                    button.updateUi();
                });
                this.el.mouseout(function () { button.updateUi(); });
            },
            updateUi: function ()
            {
                if (!this._isToggle || !this._isChecked)
                    this.el.removeClass("ui-state-active");
                else if (this._isToggle && this._isChecked)
                    this.el.addClass("ui-state-active");
            }
        },
        properties: {
            showText: {
                get: function () { return this.el.button('option', 'text'); },
                set: function (value) { this.el.button('option', 'text', value); this.el.button('refresh'); }
            },
            caption: {
                get: function () { return this.el.button('option', 'label'); },
                set: function (value) { this.el.button('option', 'label', value); this.el.button('refresh'); }
            },
            enabled: {
                get: function () { return !this.el.button('option', 'disabled'); },
                set: function (value) { this.el.button('option', 'disabled', value === false); }
            },
            iconClass: {
                get: function () { return this.el.button("option", "icons").primary; },
                set: function (value)
                {
                    this._icons.primary = value;
                    this.el.button("option", "icons", this._icons);
                    this.el.button('refresh');
                }
            },
            secondIconClass: {
                get: function () { return this.el.button("option", "icons").secondary; },
                set: function (value)
                {
                    this._icons.secondary = value;
                    this.el.button("option", "icons", this._icons);
                    this.el.button('refresh');
                }
            },
            isToggle: {
                get: function () { return this._isToggle; },
                set: function (value)
                {
                    this._isToggle = value;
                    if (!value)
                        this.el.removeClass("ui-state-active");
                    this.update();
                }
            },
            isChecked: {
                get: function () { return this._isChecked; },
                set: function (value)
                {
                    this._isChecked = value;
                    this.update();
                }
            }
        },
        events: {
            click: null
        }
    });


    Type.define("wui.PopupMenu", "wui.Control", {
        fields: {},
        methods: {
            init: function ()
            {
                this.base();
                this.addClass("wui-popup-menu");
            }
        },
        properties: {}
    });

    Type.define("wui.ContentFrame", "wui.Control", {
        fields: {},
        methods: {
            init: function ()
            {
                this.base();
                this.stretch();
                this._dispatchs = [];
                this.addClass("wui-content-frame");
                this._frame = $.newTag("iframe", {
                    src: "/Shell/ContentView",
                    "class": "op-align-stretch-h op-align-stretch-v"
                })
                    .appendTo(this.el);
                var frame = this;
                this._frame.load(function ()
                {
                    frame._dispatched = true;
                    frame.processFrameDispatchs();
                });
            },
            processFrameDispatchs: function ()
            {
                if (!this._dispatched || this._dispatchs.length === 0)
                    return;
                while (this._dispatchs.length)
                    this._frame[0].contentWindow["Loader"].dependsOn(this._dispatchs.shift());
                this._frame[0].contentWindow["Loader"].run();
            },
            dispatchToFrame: function (dep)
            {
                this._dispatchs.push(dep);
                this.processFrameDispatchs();
            },
            dependsOn: function ()
            {
                if (!arguments.length) return;
                for (var i = 0, arg; (arg = arguments[i]); i++)
                {
                    if (util.isArray(arg))
                        this.dependsOn.apply(this, arg);
                    else
                        this.dispatchToFrame(arguments[0])
                }
                return this;
            }
        }
    });

    Type.define("wui.ContentWindow", "wui.Control", {
        methods: {
            init: function ()
            {
                this.base();
                this.frame = new wui.ContentFrame();
                this.el.addClass("wui-content-window ui-widget-content ui-corner-all op-box-shadow");
                $("body").append(this.el);
                this.el.append(this.frame.el);
            },
            dependsOn: function ()
            {
                this.frame.dependsOn.apply(this.frame, arguments);
                return this;
            }
        }
    });

    Type.define("wui.TextArea", "wui.Control", {
        methods: {
            init: function ()
            {
                this.base();
                this.el.addClass("wui-input-control wui-text-area");
                this.input = this.el.newTag("textarea", { "class": "op-align-stretch-h op-align-stretch-v" });
            },
            setFocus: function ()
            {
                this.dispatch("setFocus", this._setFocus);
                return this;
            },
            _setFocus: function ()
            {
                this.input.focus();
            }
        },
        properties: {
            text: {
                get: function () { return this.input.val(); },
                set: function (value) { this.input.val(value); }
            }
        }
    });

    Type.define("wui.HtmlHolder", "wui.Control", {
        methods: {
            init: function ()
            {
                this.base();
                this.el.addClass("wui-html-holder");
            },
            newTag: function (tagName, attr, css)
            {
                return this.el.newTag(tagName, attr, css);
            },
            addTag: function (tagName, attr, css)
            {
                return this.el.addTag(tagName, attr, css);
            }
        }
    });

    wui.Container.extend().method("newHtmlHolder", function (options) { return (new wui.HtmlHolder(options)).addTo(this); });
    wui.Container.extend().method("addHtmlHolder", function (options) { return (new wui.HtmlHolder(options)).addTo(this); return this; });

    Type.define("wui.TableColumn", null, {
        fields: {
            _uid: ''
        },
        properties: {
            id: {
                get: function () { return this._id; },
                set: function (value) { this._id = value; }
            },
            header: {
                get: function () { return this._header; },
                set: function (value) { this._header = value; },
                supressBinding: true
            },
            content: {
                get: function () { return this._content; },
                set: function (value) { this._content = value; },
                supressBinding: true
            },
            width: {
                get: function () { return this._width; },
                set: function (value) { this._width = value; }
            },
            footer: {
                get: function () { return this._footer; },
                set: function (value) { this._footer = value; },
                supressBinding: true
            },
            wrap: {
                get: function () { return this._wrap; },
                set: function (value) { this._wrap = value; }
            }
        },
        methods: {
            initialize: function ()
            {
                this._uid = util.uid();
            }
        }
    });

    Type.define("wui.Table", "wui.Control", {
        methods: {
            init: function ()
            {
                this.base();
                this._columns = [];
                this.el.addClass("wui-table");
                var table = this.table = this.el.newTag("table", {
                    class: "wui-table-el",
                    border: "0",
                    width: "100%",
                    cellpadding: "0",
                    cellspacing: "0"
                });
                this.thead = table.newTag("thead");
                this.theadRow = this.thead.newTag("tr");
                this.tbody = table.newTag("tbody");
                this.tfoot = table.newTag("tfoot");
                this.tfootRow = this.tfoot.newTag("tr");
            },
            processRow: function (data, row, colTagName, objFieldName, propName, processRowData)
            {
                var bindings = this._p.bindings, rowKey, rowData;
                if (processRowData)
                {
                    if (bindings)
                    {
                        if (bindings["rowKey"])
                            this.processBinding(bindings["rowKey"], data);
                        if (bindings["rowData"])
                            this.processBinding(bindings["rowData"], data);
                        rowKey = this.rowKey();
                        rowData = this.rowData();
                        if (rowKey)
                            row.attr("data-wui-tbrk", rowKey);
                        if (rowData)
                            row.data("wui-table-r-data", rowData);
                    }
                }
                for (var i = 0, col; (col = this._columns[i]); i++)
                {
                    col.dataBind(data);
                    bindings = col._p.bindings;
                    var colEl = row.newTag(colTagName, { "data-wui-col-id": col._uid });
                    if (bindings && bindings[propName])
                        col.processBinding(bindings[propName], data);
                    if (col[objFieldName])
                        colEl.append(this.processTemplate(col[objFieldName], data));
                    if (col._wrap)
                        colEl.addClass("op-text-wrap");
                    if (col._width)
                        colEl.css("width", col._width);
                }
            },
            dataBind: function (data)
            {
                this.base(data);
                var source = this.itemsSource();
                this.theadRow.children().remove();
                this.processRow(source, this.theadRow, "th", "_header", "header", false);
                this.tbody.children().remove();
                if (source)
                {
                    var en = Enum(source);
                    while (en.next())
                        this.processRow(en.current, this.tbody.newTag("tr"), "td", "_content", "content", true);
                }
                this.tfootRow.children().remove();
                this.processRow(source, this.tfootRow, "th", "_footer", "footer", false);
            },
            updateUi: function ()
            {
            },
            addColumn: function (options)
            {
                var col = new wui.TableColumn().set(options);
                this._columns.push(col);
                this.update();
            },
            addRow: function (data)
            {
                this.processRow(data, this.tbody.newTag("tr"), "td", "_content", "content", true);
            },
            set: function (options)
            {
                if (!options)
                    return;
                if (options.columns)
                {
                    var cols = util.getArray(options.columns).slice(0);
                    delete options["columns"];
                    for (var i = 0, col; (col = cols[i]); i++)
                        this.addColumn(col);
                }
                this.base(options)
            },
            _rowClicked: function (row)
            {
                this.triggerRowClicked(row.attr("data-wui-tbrk"), row.data("wui-table-r-data"));
            }
        },
        properties: {
            headerVisible: {
                get: function () { return !this.theadRow.hasClass("op-hidden"); },
                set: function (value)
                {
                    if (value)
                        this.theadRow.removeClass("op-hidden");
                    else
                        this.theadRow.addClass("op-hidden");
                }
            },
            footerVisible: {
                get: function () { return !this.tfootRow.hasClass("op-hidden"); },
                set: function (value)
                {
                    if (value)
                        this.tfootRow.removeClass("op-hidden");
                    else
                        this.tfootRow.addClass("op-hidden");
                }
            },
            rowSelectable: {
                get: function () { return this.el.hasClass("wui-table-row-select"); },
                set: function (value)
                {
                    if (value)
                        this.el.addClass("wui-table-row-select");
                    else
                        this.el.removeClass("wui-table-row-select");
                }
            },
            rowData: {
                // suprimir o binding automatico nesta propriedade, ele será feito a mão depois...
                supressBinding: true
            },
            rowKey: {
                // suprimir o binding automatico nesta propriedade, ele será feito a mão depois...
                supressBinding: true
            },
            itemsSource: {}
        },
        events: {
            rowClicked: null /* function(rowKey, rowData) */
        }
    });

    //jquery para a tabela
    $(".wui-table.wui-table-row-select tr *").live("mouseenter", function (e)
    {
        $(this).parents("tr:first").addClass("ui-state-hover");
        e.stopPropagation();
    }).live("mouseout", function (e)
    {
        $(this).parents("tr:first").removeClass("ui-state-hover");
        e.stopPropagation();
    }).live("click", function (e)
    {
        $(this).parents(".wui-table:first").wuiControl()._rowClicked($(this).parents("tr:first"));
        e.stopPropagation();
    });

    wui.Container.extend().method("newTable", function (options) { return new wui.Table(options).addTo(this); });
    wui.Container.extend().method("addTable", function (options) { return new wui.Table(options).addTo(this); return this; });

    // carregar o window manager
    Loader.run(function ()
    {
        ctx.WindowManager = new wui.WindowManager();
        ctx.WindowManager.block();
        setTimeout(function ()
        {
            ctx.WindowManager.unblock();
        }, 500);
    });
})(window);