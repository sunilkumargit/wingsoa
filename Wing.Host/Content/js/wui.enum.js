(function WuiEnum(ctx)
{
    var wui = ctx.wui;
    var util = ctx.Util;
    //returns an Enumerator for the given object
    var Enum = ctx.Enum = function (obj)
    {
        if (!obj)
            return undefined;
        else if (obj.isW$Enum)
            return obj;
        else if (util.isArray(obj) || (obj.length && obj.callee))
            return new wui.collections.ArrayEnumerator(obj);
        else if (obj.getEnum)
            return obj.getEnum();
        else if (typeof obj.jquery === "string" && typeof obj.length === "number") // jQuery object
            return new wui.collections.JQueryEnumerator(obj);
        return undefined;
    };

    Enum._sortComparers = {};
    Enum.regSortComparer = function (name, fn)
    {
        Enum._sortComparers[name] = fn;
    };

    Enum.getSortComparer = function (name)
    {
        return Enum._sortComparers[name];
    };


    Enum.regSortComparer(util.nativeType(""), function (x, y)
    {
        if (x > y)
            return 1;
        else if (x < y)
            return -1;
        else
            return 0;
    });

    Enum.regSortComparer(util.nativeType(0), function (x, y)
    {
        return x - y;
    });

    Enum.regSortComparer(util.nativeType(true), function (x, y)
    {
        if (x == y)
            return 0;
        else if (x)
            return -1;
        else
            return 1;
    });

    Enum.regSortComparer("cast",
        function (x, y)
        {
            var xtype, ytype, xIsPrim, yIsPrim;
            xtype = util.nativeType(x);
            ytype = util.nativeType(y);
            xIsPrim = xtype === "string" || xtype === "number" || xtype === "boolean";
            yIsPrim = ytype === "string" || ytype === "number" || ytype === "boolean";

            if (xIsPrim && yIsPrim)
            {
                if (xtype === "number")
                    return ytype === "number" ? Enum._comparers[xtype](x, y) : -1;
                else if (xtype === "string")
                {
                    if (ytype === "number") return 1;
                    else return ytype === "string" ? Enum.$comparers[xtype](x, y) : -1;
                } else if (xtype === "boolean")
                {
                    if (ytype === "number" || ytype === "string") return 1;
                    else return ytype === "boolean" ? jMass.$comparers[xtype](x, y) : -1;
                }
            } else if (!xIsPrim && !yIsPrim)
                return 0;
            else if (xIsPrim)
                return -1;
            else
                return 1;
        });


    /*
    *
    *  Enumeration system
    *
    *
    *
    */
    var enumeratorType = Type.define("wui.collections.Enumerator", null, {
        fields: {
            isW$Enum: true,
            current: null,
            _source: null
        },
        properties: {
            source: {
                get: function () { return this._source; },
                set: function (value)
                {
                    this._source = value;
                    this.reset();
                }
            }
        },
        methods: {
            initialize: function (source)
            {
                if (source)
                    this.setSource(source);
            },
            getEnum: function ()
            {
                return this;
            },
            reset: function () { this.current = undefined; },
            next: function () { },
            toArray: function ()
            {
                var en = this;
                en.reset();
                var result = [];
                while (en.next())
                    result[result.length] = en.current;
                return result;
            }
        }
    });


    Type.define("wui.collections.ArrayEnumerator", enumeratorType, {
        fields: {
            _arrayIndex: -1
        },
        methods: {
            reset: function ()
            {
                this.base();
                this._arrayIndex = -1;
            },
            next: function ()
            {
                if (this._arrayIndex < this._source.length - 1)
                {
                    this.current = this._source[++this._arrayIndex];
                    return true;
                }
                this.current = undefined;
                return false;
            }
        }

    });

    Type.define("wui.collections.JQueryEnumerator", enumeratorType, {
        fields: {
            _index: -1
        },
        methods: {
            reset: function ()
            {
                this.base();
                this._index = -1;
            },
            next: function ()
            {
                if (this._index < this._source.length - 1)
                {
                    this.current = $(this._source[++this._index]);
                    return true;
                }
                this.current = undefined;
                return false;
            }
        }
    });



    var chainedEnumType = Type.define("wui.collections.ChainedEnumerator", enumeratorType, {
        methods: {
            setSource: function (source)
            {
                this.base(Enum(source));
            },
            reset: function ()
            {
                this.base();
                this._source.reset();
            }
        }
    });

    Type.define("wui.collections.WhereEnumerator", enumeratorType, {
        fields: {
            _condiction: function () { return true; }
        },
        methods: {
            initialize: function (source, condiction)
            {
                this.base(source);
                if (condiction)
                    this._condiction = util.expr(condiction);

            },
            next: function ()
            {
                while (this._source.next())
                {
                    var item = this._source.current;
                    if (this._condiction.call(item, item))
                    {
                        this.current = item;
                        return true;
                    }
                }
                this.current = undefined;
                return false;
            }
        }
    });
    // add the 'where' method to Enumerator
    enumeratorType.extend().method("where", function (condiction)
    {
        return new wui.collections.WhereEnumerator(this, condiction);
    });

    // define the 'select' enumerator
    Type.define("wui.collections.SelectEnumerator", "wui.collections.ChainedEnumerator", {
        fields: {
            _selectFunction: function (x) { return x; }
        },
        methods: {
            initialize: function (source, selectFn)
            {
                this.base(source);
                if (selectFn)
                    this._selectFunction = util.expr(selectFn);
            },
            next: function ()
            {
                var item;
                while (this._source.next())
                {
                    item = this._source.current;
                    if ((item = this._selectFunction.call(item, item)) !== undefined)
                    {
                        this.current = item;
                        return true;
                    }
                }
                this.current = undefined;
                return false;
            }
        }

    });

    // add the 'select' method to Enumerator
    enumeratorType.extend().method("select", function (selectFn)
    {
        return new wui.collections.SelectEnumerator(this, selectFn);
    });


    // define the 'take' enumerator
    Type.define("wui.collections.TakeEnumerator", "wui.collections.ChainedEnumerator", {
        fields: {
            _count: 0,
            _taked: 0
        },
        methods: {
            initialize: function (source, takeN)
            {
                this.base(source);
                this._count = takeN;
            },
            reset: function ()
            {
                this.base();
                this._taked = 0;
            },
            next: function ()
            {
                if (this._taked < this._count)
                {
                    while (this._source.next())
                    {
                        this.current = this._source.current;
                        ++this._taked;
                        return true;
                    }
                }
                this.current = undefined;
                return false;
            }
        }
    });

    // add the 'take' method to enumerator
    enumeratorType.extend().method("take", function (count)
    {
        return new wui.collections.TakeEnumerator(this, count);
    });

    Type.define("wui.collections.SkipEnumerator", "wui.collections.ChainedEnumerator", {
        fields: {
            _count: 0,
            _skipped: 0
        },
        methods: {
            initialize: function (source, skipN)
            {
                this.base(source);
                this._count = skipN;
            },
            reset: function ()
            {
                this.base();
                this._skipped = 0;
            },
            next: function ()
            {
                while (this._skipped++ < this._count && this._source.next()) { }
                if (this._source.next())
                {
                    this.current = this._source.current;
                    return true;
                }
                this.current = undefined;
                return false;
            }
        }
    });

    enumeratorType.extend().method("skip", function (count)
    {
        return new wui.collections.SkipEnumerator(this, count);
    });

    Type.define("wui.collections.OrderByEnumerator", "wui.collections.ChainedEnumerator", {
        fields: {
            _buffer: null
        },
        methods: {
            initialize: function (source, keySelector, comparer, descending)
            {
                this.base(source);
                this.items = [];
                this.thenBy(keySelector, comparer, descending);
            },
            thenBy: function (keySelector, comparer, descending)
            {
                this.items.push({ keySelector: util.expr(keySelector) || function (x) { return x; },
                    descending: descending === true,
                    comparer: comparer
                });
                return this;
            },
            thenByDescending: function (keySelector, comparer)
            {
                return this.thenBy(keySelector, comparer, true);
            },
            reset: function ()
            {
                this.base();
                this._buffer = null;
            },
            _internalSort: function ()
            {
                var comparer, comparerFn;
                this._buffer = [];
                var items = this.items;
                while (this._source.next())
                {
                    var current = this._source.current;
                    var entry = { item: current, keys: [] };
                    for (var i = 0, item; (item = this.items[i]); i++)
                        entry.keys[i] = item.keySelector(current);
                    this._buffer.push(entry);
                }

                for (var i = 0, item; (item = items[i]); i++)
                {
                    var comparer = item.comparer;
                    if (!comparer)
                    {
                        if (this._buffer.length)
                            comparer = typeof this._buffer[0].keys[i];
                        else comparer = "cast"
                    }
                    if (typeof comparer === "string")
                        comparer = Enum.getSortComparer(comparer);
                    item._comparer = comparer;
                }


                this._buffer.sort(function (a, b)
                {
                    var r;
                    for (var i = 0, item; (item = items[i]); i++)
                    {
                        r = item._comparer(a.keys[i], b.keys[i]);
                        if (r == 0) continue;
                        return item.descending ? r * -1 : r;
                    }
                    return 0;
                });
            },
            next: function ()
            {
                if (!this._buffer)
                    this._internalSort();
                if (this._buffer.length)
                {
                    this.current = this._buffer.shift().item;
                    return true;
                }
                this.current = undefined;
                return false;
            }
        }
    });

    enumeratorType.extend({
        methods: {
            orderBy: function (keySelector, comparer)
            {
                return new wui.collections.OrderByEnumerator(this, keySelector, comparer, false);
            },
            orderByDescending: function (keySelector, comparer)
            {
                return new wui.collections.OrderByEnumerator(this, keySelector, comparer, true);
            }
        }
    });


    /* enum methods */
    enumeratorType.extend({
        methods: {
            contains: function (clause)
            {
                this.reset();
                var condiction = util.expr(clause);
                if (!condiction)
                    return this.next();
                while (this.next())
                {
                    if (condiction.call(this.current, this.current))
                        return true;
                }
                return false;
            },
            each: function (iteratorFn, repeat)
            {
                iteratorFn = util.expr(iteratorFn);
                repeat = repeat || 1;
                while (repeat-- > 0)
                {
                    this.reset();
                    while (this.next())
                        iteratorFn.call(this.current, this.current);
                }
            },
            first: function (clause)
            {
                this.reset();
                clause = util.expr(clause);
                if (!clause)
                {
                    this.next();
                    return this.current;
                }
                while (this.next())
                {
                    if (clause.call(this.current, this.current))
                    {
                        return this.current;
                    }
                }
                return undefined;
            },
            firstOrDefault: function (clause, defaultIfUndef)
            {
                var first;
                return (first = this.first(clause)) === undefined ? defaultIfUndef : first;
            },
            last: function (clause)
            {
                var result;
                this.reset();
                clause = util.expr(clause);
                if (!clause)
                {
                    while (this.next())
                        result = this.current;
                } else
                {
                    while (this.next())
                    {
                        if (clause.call(this.current, this.current))
                            result = this.current;
                    }
                }
                return result;
            },
            lastOrDefault: function (clause, defaultIfUndef)
            {
                var last;
                return (last = this.last(clause)) === undefined ? defaultIfUndef : last;
            },
            sum: function (selectFn)
            {
                var result = 0, itemValue = 0, en = this;
                en.reset();
                clause = util.expr(selectFn);
                if (!clause)
                    clause = function (x) { return x; };
                while (en.next())
                    result += (itemValue = clause(en.current)) ? itemValue : 0;
                return result;
            },
            avg: function (selectFn)
            {
                var sum = 0, count = 0, itemValue = 0, en = this;
                clause = util.expr(selectFn);
                if (!clause)
                    clause = function (x) { return x; };
                en.reset();
                while (en.next())
                {
                    itemValue = clause(en.current);
                    if (itemValue)
                    {
                        sum += itemValue;
                        count++;
                    }
                }
                return count === 0 ? 0 : sum / count;
            },
            max: function (selectFn)
            {
                var result, en = this;
                en.reset();
                if (!en.next())
                    return 0;
                clause = util.expr(selectFn);
                if (!clause)
                    clause = function (x) { return x; };
                result = clause(en.current);
                while (en.next())
                    result = Math.max(result, clause(en.current));
                return result;
            },
            min: function (selectFn)
            {
                var result, en = this;
                en.reset();
                if (!en.next())
                    return 0;
                clause = util.expr(selectFn);
                if (!clause)
                    clause = function (x) { return x; };
                result = clause(en.current);
                while (en.next())
                    result = Math.min(result, clause(en.current));
                return result;
            },
            count: function (clause)
            {
                var result = 0, en = this;
                clause = util.expr(clause);
                en.reset();
                if (!clause)
                {
                    for (; en.next(); result++)
                    {
                    }
                } else
                {
                    while (en.next())
                    {
                        if (clause.call(en.current, en.current))
                            result++;
                    }
                }
                return result;
            },
            any: function (clause)
            {
                clause = util.expr(clause);
                if (!clause)
                    clause = function (x) { return true; };
                this.reset();
                while (this.next())
                {
                    if (clause.call(this.current, this.current))
                        return true;
                }
                return false;
            },
            indexOf: function (clause)
            {
                clause = util.expr(clause);
                if (!clause)
                    return -1;
                this.reset();
                var cnt = 0;
                while (this.next())
                {
                    if (clause.call(this.current, this.current))
                        return cnt;
                    cnt++;
                }
                return -1;
            }
        }
    });
})(window);

