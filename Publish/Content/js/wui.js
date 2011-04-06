
/*******
*
* Wing UI Framework
* Version: 0.1a
* Marcelo Dezem (mdezem@hotmail.com)
* Feb/2011
*
*/

/*
*
* Prototype for js objects
*
*
*/
String.prototype.trim = function ()
{
    return this.replace(/^\s+|\s+$/g, '');
};

String.prototype.ltrim = function ()
{
    return this.replace(/^\s+/, '');
};

String.prototype.rtrim = function ()
{
    return this.replace(/\s+$/, '');
};

String.prototype.startsWith = function (str)
{
    return this.match("^" + str) == str;
};

String.prototype.endsWith = function (str)
{
    return (this.match(str + "$") == str);
};

__wuiSysFn = function WuiSys(ctx)
{

    // declare _wui namespace.
    var wui = ctx.wui = {}, toString = Object.prototype.toString;

    /*
    *
    *
    *  Helper methods
    *
    *
    */
    var util = ctx.Util = wui.Util = {};
    util._pvt = {};
    util.each = function (object, callback, args)
    {
        var name, i = 0,
            length = object.length,
            isObj = length === undefined || util.isFunction(object);

        if (args)
        {
            if (isObj)
            {
                for (name in object)
                {
                    if (callback.apply(object[name], args) === false)
                    {
                        break;
                    }
                }
            } else
            {
                for (; i < length; )
                {
                    if (callback.apply(object[i++], args) === false)
                    {
                        break;
                    }
                }
            }

            // A special, fast, case for the most common use of each
        } else
        {
            if (isObj)
            {
                for (name in object)
                {
                    if (callback.call(object[name], name, object[name]) === false)
                    {
                        break;
                    }
                }
            } else
            {
                for (var value = object[0];
                    i < length && callback.call(value, i, value) !== false;
                    value = object[++i]) { }
            }
        }

        return object;
    };

    util.nativeType = function (obj)
    {
        return obj == null ?
            String(obj) :
            this._pvt.class2type[toString.call(obj)] || "object";
    };

    util.isFunction = function (obj) { return util.nativeType(obj) === "function"; };
    util.isArray = function (obj) { return util.nativeType(obj) === "array"; };
    util.isWindow = function (obj) { return obj && typeof obj === "object" && "setInterval" in obj; };
    util.isString = function (obj) { return util.nativeType(obj) === "string"; };
    util.isObject = function (obj) { return obj !== true && obj !== false && util.nativeType(obj) === "object"; };
    util.isEmpty = function (o)
    {
        return !o || (o === "") || (this.isArray(o) && o.length === 0);
    };

    var cls2t = util._pvt.class2type = {};
    util.each("Boolean Number String Function Array Date RegExp Object".split(" "), function (i, name)
    {
        cls2t["[object " + name + "]"] = name.toLowerCase();
    });

    util.getFromParentWindow = function (name, includeSelf)
    {
        if (includeSelf && ctx[name])
            return ctx[name];
        var buffer = ctx, parent = buffer.parent;
        while (parent && parent !== buffer)
        {
            buffer = parent;
            if (buffer[name])
                return buffer[name];
            parent = buffer.parent;
        }
    };


    //
    // UID
    //
    util._pvt.$s3 = function ()
    {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(3);
    };

    util.uid = function ()
    {
        var d = new Date().getTime().toString(16);
        return (this._pvt.$s3() + d.substring(d.length - 8) + this._pvt.$s3());
    };

    //
    // Asserts
    //
    util.assertNullArg = function (value, name)
    {
        if (value === null || value === undefined)
            throw "Null argument exception; name: " + name;
    };

    util.assertEmptyArg = function (value, name)
    {
        if (!value && value !== false || (typeof value === "string" && value.length == 0))
            throw "Empty argument exception. The argumento with name " + name + " cannot be null, undefined or empty.";
    };

    util.assertType = function (obj, typeNameOrDef, name)
    {
        if (!sys._isTypeOf(obj, typeNameOrDef))
            throw "The param name " + name + " must be of type " + typeNameOrDef;
    };

    util._pvt.formatters = {};

    util.registerFormatter = function (name, callback)
    {
        util._pvt.formatters[name] = callback;
    };

    util.format = function (pattern, params, customFormatters)
    {
        var tokens = pattern.split(/([{}])/), token, format, source, formatter, varname;
        params = params || {};
        for (var i = 0, token; i < tokens.length; i++)
        {
            token = tokens[i];
            if (!token || !token.length)
                continue;
            // verificar se é uma variavel e se está bem formatada
            else if (token.substr(0, 1) === "@" && i > 0 && tokens[i - 1] === "{" && tokens[i + 1] === "}")
            {
                tokens[i - 1] = tokens[i + 1] = "";
                // verificar o token
                format = token.substr(1).split(/:/);
                for (var k = 0, item; (item = format[k]); k++)
                {
                    item = item.replace(/(^:|:$)/g, "");
                    if (k === 0)
                    {
                        source = params[item];
                        varname = item;
                    }
                    else
                    {
                        formatter = (customFormatters && customFormatters[item]) || this._pvt.formatters[item];
                        if (!formatter)
                            throw "Formatter not found: " + item;
                        source = formatter(source, varname, params);
                    }
                }
                tokens[i] = source;
            }
        }
        return tokens.join('');
    };

    util.getArray = function (obj)
    {
        if (obj == undefined || obj == null) return [];
        else if (util.isArray(obj)) return obj;
        else return [obj];
    };

    // context id 
    ctx.__ContentId = util.uid();

    // default formatters
    util.registerFormatter("remove", function (value, name, params)
    {
        delete params[name];
        return value;
    });

    util.registerFormatter("upper", function (value) { return (value || "").toString().toLowerCase(); });
    util.registerFormatter("lower", function (value) { return (value || "").toString().toUpperCase(); });
    util.registerFormatter("trim", function (value) { return (value || "").toString().trim(); });

    // 
    // Expression
    //
    //compile a lambda expression and returns his equilavent function()
    util._pvt.$lambdaMatch = /\s*\(?([\w*\,]*)\)?[\s|\S]*\=>(\s*[\s|\S]*([.|\S|\s]*))\s*/;
    util.expr = function (expr)
    {
        if (!expr)
            return undefined;
        if (this.isFunction(expr))
            return expr;
        var splitted = expr.match(this._pvt.$lambdaMatch);

        if (splitted.length < 3) throw "Cannot compile expression " + expr;

        var params = splitted[1].replace(/^\s*|\s(?=\s)|\s*$/g, '');
        var body = splitted[2];

        //check if expression has just 1 statement;
        if (body.indexOf(';') < 2)
        {
            // prepend a return if not already there.
            body = ((!/\s*return\s+/.test(body)) ? "return " : "") + body;
        }

        var result = eval('_wuiExprFn = function(' + params + ') {' + body + '}');
        return result;
    };


    /*
    *
    * WUI Object Model
    * The sys contains the system to create 'types' with some features:
    *    - The type is inheritable -- it's possible to create a new class that inherits the behavior of another class.
    *    - The type is extensible -- it's possible to 'override' a method of a class, even with no need of inherit of the class.
    *    - The type holds properties -- properties is not fields. Properties is accessed by "get/set" methods. i.e.: if a class
    *      has a property with name "Name", then you can access the property by methods getName() and setName(). Properties
    *      is a way to abstract the dom manipulation. Every property has a "applier function", that is responsible to
    *      to "apply" the property value to the DOM or whatelse.
    *    - It's possible to use 'reflection' on this classes. you can use Class.parent to access the prototype of the base class, and so on.
    *      Class.isLeafOf(), Class.getAncestors(), Class.getProperties(), Class.getMethods(), Class.getEvents() is an example of reflection methods.
    */


    var sys = {
        /*
        *
        *
        *
        *   TYPE SYSTEM - Internals
        *
        *
        *
        */

        // definition store
        defCache: {
            types: {},
            typesList: [],
            methods: {},
            methodsList: [],
            properties: {},
            propertiesList: [],
            events: {},
            eventsList: [],
            fields: {},
            fieldsList: [],
            namespaces: {}
        },

        // member cache
        memberCache: {
            fields: {},
            properties: {},
            methods: {},
            events: {},
            ancestors: {},
            isTypeOf: {}
        },

        _setCache: function (typeDef, memberType, memberDef)
        {
            var cache = this.defCache[memberType];
            var list = this.defCache[memberType + "List"];

            cache[memberDef.uniqueName] = memberDef;
            list.push(memberDef);

            this.memberCache[memberType] = {};
        },

        // instance counter
        _instanceId: 0,

        // base function for all types defined in wui
        _base: (function ()
        {
            var base = function () { };
            base.prototype = {
                isW$Object: true,
                base: function ()
                {
                    var fn = this._p.fn;
                    if (fn.prior)
                        return fn.prior.apply(this, arguments);
                    else if (fn.name && fn.parent && fn.parent.proto[fn.name])
                        return fn.parent.proto[fn.name].apply(this, arguments);
                }
            }
            return base;
        })(),

        _registerType: function (typeDef)
        {
            this.defCache.types[typeDef.fullname] = typeDef;
            this.defCache.typesList.push(typeDef);
            this.memberCache.properties = {};
            this.memberCache.fields = {};
            this.memberCache.events = {};
            this.memberCache.methods = {};
            this.memberCache.ancestors = {};
            this.memberCache.isTypeOf = {};
        },

        _resolveType: function (nameOrDef)
        {
            if (!nameOrDef)
                return;
            else if (nameOrDef.isType)
                return nameOrDef;
            else if (nameOrDef.isW$Object)
                return nameOrDef.type;
            return this.defCache.types[nameOrDef];
        },

        _registerMethod: function (typeDef, name, fn)
        {
            var method = this._getMethod(typeDef, name, false);
            if (!method)
            {
                method = {
                    isW$Method: true,
                    owner: typeDef,
                    name: name,
                    fn: fn,
                    chain: [fn],
                    uniqueName: typeDef.fullname + "." + name
                };
                typeDef.methods[method.name] = method;
                this._setCache(typeDef, "methods", method);
            }
            else
            {
                fn._prior = method.fn;
                method.fn = fn;
                method.chain.push(fn);
            }
            return method;
        },

        _registerField: function (typeDef, name, defaultValue)
        {
            var field = {
                isW$Field: true,
                owner: typeDef,
                defaultValue: defaultValue,
                name: name,
                uniqueName: typeDef.fullname + "." + name
            };
            typeDef.fields[name] = field;
            if (util.isFunction(defaultValue))
                typeDef.sys.fldInitLst.push(field);
            this._setCache(typeDef, "fields", field);
            return field;
        },

        _registerProperty: function (typeDef, name, getter, setter, userGetter, userSetter, supressBinding)
        {
            var property = {
                isW$Property: true,
                owner: typeDef,
                name: name,
                getter: getter,
                setter: setter,
                usrGetter: userGetter,
                usrSetter: userSetter,
                uniqueName: typeDef.fullname + "." + name,
                supressBinding: supressBinding === true
            };
            typeDef.properties[property.name] = property;
            this._setCache(typeDef, "properties", property);
            return property;
        },

        _registerEvent: function (typeDef, name)
        {
            var event = {
                isW$Event: true,
                owner: typeDef,
                name: name,
                uniqueName: typeDef.fullname + "." + name
            };
            typeDef.events[name] = event;
            this._setCache(typeDef, "events", event);
            return event;
        },

        _getNamespace: function (namespace, createIfNotExists)
        {
            var result = ctx;
            if (namespace && namespace.length > 0)
            {
                if (this.defCache.namespaces[namespace])
                    return this.defCache.namespaces[namespace];
                var parts = namespace.split('.');
                for (var i = 0, ns; ns = parts[i]; i++)
                {
                    if (result[ns])
                        result = result[ns];
                    else if (createIfNotExists)
                        result = result[ns] = {};
                }
                this.defCache.namespaces[namespace] = result;
            }
            return result;
        },

        _resolveMember: function (typeDef, name, memberType, flatten)
        {
            var cache = this.memberCache[memberType][typeDef.fullname];
            if (cache && cache[name])
                return cache[name];
            else
            {
                var ctype = typeDef;
                while (ctype)
                {
                    var member = ctype[memberType][name];
                    if (member)
                    {
                        if (!cache)
                            cache = this.memberCache[memberType][typeDef.fullname] = {};
                        cache[name] = member;
                        return member;
                    }
                    else if (!ctype.ancestor || !flatten)
                        return undefined;
                    else
                        ctype = ctype.ancestor;
                }
            }
        },

        _getMembers: function (typeDef, memberType, flatten)
        {
            var cacheKey = '@' + typeDef.fullname + "." + memberType + "_flt";
            var cache = this.memberCache[memberType][cacheKey]
            if (!cache)
                cache = this.memberCache[memberType][cacheKey] = { members: {}, list: [] };

            var ctype = typeDef;
            while (ctype)
            {
                var members = ctype[memberType];
                for (var m in members)
                {
                    if (members.hasOwnProperty(m))
                    {
                        if (!cache.members.hasOwnProperty(m))
                        {
                            cache.members[m] = members[m];
                            cache.list.push(members[m]);
                        }
                    }
                }
                if (!ctype.ancestor || !flatten)
                    break;
                else
                    ctype = ctype.ancestor;
            }
            return cache.list;
        },

        _getType: function (fullname)
        {
            return this.defCache.types[fullname];
        },

        _getField: function (typeDef, name, flatten)
        {
            return this._resolveMember(typeDef, name, "fields", flatten !== false);
        },

        _getProperty: function (typeDef, name, flatten)
        {
            return this._resolveMember(typeDef, name, "properties", flatten !== false);
        },

        _getMethod: function (typeDef, name, flatten)
        {
            return this._resolveMember(typeDef, name, "methods", flatten !== false);
        },

        _getEvent: function (typeDef, name, flatten)
        {
            return this._resolveMember(typeDef, name, "events", flatten !== false);
        },

        _getFields: function (typeDef, flatten)
        {
            return this._getMembers(typeDef, "fields", flatten !== false);
        },

        _getProperties: function (typeDef, flatten)
        {
            return this._getMembers(typeDef, "properties", flatten !== false);
        },

        _getMethods: function (typeDef, flatten)
        {
            return this._getMembers(typeDef, "methods", flatten !== false);
        },

        _getEvents: function (typeDef, flatten)
        {
            return this._getMembers(typeDef, "events", flatten !== false);
        },

        _getAncestors: function (typeDef)
        {
            var result = [];
            while (typeDef.ancestor)
            {
                result.push(typeDef.ancestor);
                typeDef = typeDef.ancestor;
            }
            return result.reverse();
        },

        _isTypeOf: function (sourceTypeNameOrDef, targetTypeNameOrDef)
        {
            var source = sys._resolveType(sourceTypeNameOrDef);
            var target = sys._resolveType(targetTypeNameOrDef);
            if (!source || !target)
                return false;
            var key = source.fullname + ":" + target.fullname;
            var result = sys.memberCache.isTypeOf[key];
            if (result === undefined)
            {
                if (source.fullname == target.fullname)
                {
                    sys.memberCache.isTypeOf[key] = true;
                    return true;
                }
                else
                {
                    var ancestors = sys._getAncestors(source);
                    for (var i = 0; i < ancestors.length; i++)
                    {
                        if (ancestors[i].fullname === target.fullname)
                        {
                            sys.memberCache.isTypeOf[key] = true;
                            return true;
                        }
                    }
                    sys.memberCache.isTypeOf[key] = false;
                }
            }
            return result;
        },

        /* property system */
        _accessProperty: function (property, value)
        {
            if (value !== undefined)
            {
                var curr = property.usrGetter.apply(this);
                if (curr !== value)
                {
                    if (value.isW$Binding)
                    {
                        // register the binding for property.
                        value.target = property;
                        sys._addBinding.call(this, value);
                    }
                    else
                    {
                        var postValue = property.usrSetter.call(this, value);
                        if (postValue !== undefined)
                            value = postValue;
                        this.triggerPropertyChanged(property.name, value, curr);
                    }
                }
                return this;
            }
            return property.usrGetter.apply(this);
        },

        _addBinding: function (binding)
        {
            var store = this._p.bindings = this._p.bindings || {};

            if (util.isEmpty(binding.target))
                throw "The target property of a binding cannot be empty";

            if (typeof binding.target === "string")
            {
                //procurar o membro
                var member = this.type.getProperty(binding.target);
                if (!member)
                    member = this.type.getField(binding.target);
                if (!member)
                    throw "The target is invalid. Field or property with name " + binding.target + " does not exists.";
                binding.target = member;
            }
            store[binding.target.name] = binding;
        },

        /*
        *
        *
        *  TYPE SYSTEM - Type definition internals
        *
        *
        *
        */


        // register a new Type definition;
        _define: function (fullname, inherit, members)
        {
            var existing = sys._getType(fullname);
            if (existing)
                throw "A type definition with name " + fullname + " already exists";

            var ancestor = sys._resolveType(inherit) || sys._root || sys._base;

            var name = fullname;
            var namespace = "";
            var idx = name.lastIndexOf('.');
            if (idx > -1)
            {
                namespace = name.substr(0, idx);
                name = name.substr(idx + 1);
            }

            var typeDef = {
                sys: {
                    fldInitLst: []
                },
                isType: true,
                name: name,
                fullname: fullname,
                namespace: namespace,
                ancestor: ancestor.isType ? ancestor : null,
                fn: null /* setted ahead */,
                proto: null /* setted ahead */,
                methods: {},
                fields: {},
                properties: {},
                events: {},
                getMethods: function ()
                {
                    return sys._getMethods(this);
                },

                getFields: function ()
                {
                    return sys._getFields(this);
                },

                getProperties: function ()
                {
                    return sys._getProperties(this);
                },

                getAncestors: function ()
                {
                    return sys._getAncestors(this);
                },

                isTypeOf: function (type)
                {
                    return sys._isTypeOf(this, type);
                },

                getEvents: function ()
                {
                    return sys._getEvents(this);
                },

                getMethod: function (name, flatten)
                {
                    return sys._getMethod(this, name, flatten);
                },

                getField: function (name, flatten)
                {
                    return sys._getField(this, name, flatten);
                },

                getProperty: function (name, flatten)
                {
                    return sys._getProperty(this, name, flatten);
                },

                getEvent: function (name, flatten)
                {
                    return sys._getEvent(this, name, flatten);
                },

                create: function ()
                {
                    var fn = this.fn;
                    function W$Instance(args)
                    {
                        return fn.apply(this, args);
                    }
                    W$Instance.prototype = this.proto;
                    return new W$Instance(arguments);
                },

                extend: function (members)
                {
                    return this.sys._extender.extend(members);
                }
            };
            typeDef.fn = sys._def_mbr_Initializer(ancestor.isType ? ancestor.proto : ancestor.prototype);
            typeDef.proto = typeDef.fn.prototype;
            typeDef.proto.type = typeDef;
            typeDef.fn.type = typeDef;

            var ns = this._getNamespace(typeDef.namespace, true);
            if (!ns[typeDef.name])
                ns[typeDef.name] = typeDef.fn;

            typeDef.sys._extender = new sys._TypeExtender(typeDef);
            typeDef.fn.extend = function (members) { return this.type.sys._extender.extend(members); };

            sys._registerType(typeDef);
            sys._extend(typeDef, members);
            return typeDef;
        },

        _def_mbr_Initializer: function (ancestorPrototype)
        {
            //initializer
            var initializer = function W$Constructor()
            {
                this._p = {};
                this._p.store = {};
                this._p.events = {};
                this.iid = (++sys._instanceId).toString(16);
                var fldInitLst = this.type.sys.fldInitLst;
                for (var i = 0, fldDef; fldDef = fldInitLst[i]; i++)
                    this[fldDef.name] = fldDef.defaultValue.apply(this);
                if (this.initialize)
                    this.initialize.apply(this, arguments);
            };
            //create object proto base
            var protoFn = function W$Proto() { };
            protoFn.prototype = ancestorPrototype;
            initializer.prototype = new protoFn();
            initializer.prototype.constructor = initializer;
            return initializer;
        },

        _TypeExtender: function (typeDef)
        {
            this.type = typeDef;
            this.method = function method(name, options)
            {
                var params = { methods: {} };
                params.methods[name] = options;
                sys._extend(this.type, params);
                return this;
            };
            this.property = function property(name, options)
            {
                var params = { properties: {} };
                params.properties[name] = options;
                sys._extend(this.type, params);
                return this;
            };
            this.field = function field(name, options)
            {
                var params = { fields: {} };
                params.fields[name] = options;
                sys._extend(this.type, params);
                return this;
            };
            this.event = function event(name, options)
            {
                var params = { events: {} };
                params.events[name] = options;
                sys._extend(this.type, params);
                return this;
            };
            this.extend = function extend(members)
            {
                if (members)
                    sys._extend(this.type, members);
                return this;
            };
            this.getType = function getType()
            {
                return this.type;
            }
        },

        // extend a type definition
        _extend: function (name, members)
        {
            var typeDef = sys._resolveType(name);
            if (!typeDef || !members)
                return;
            if (members.fields)
            {
                var fields = members.fields;
                for (var f in fields)
                {
                    if (fields.hasOwnProperty(f))
                    {
                        var value = fields[f];
                        var defValue;
                        if (util.isObject(value))
                            defValue = value.init;
                        else
                            defValue = value;
                        sys._def_mbr_Field(typeDef, f, defValue);
                    }
                }
            }
            if (members.properties)
            {
                var properties = members.properties;
                for (var p in properties)
                {
                    if (properties.hasOwnProperty(p))
                    {
                        var property = properties[p] || {};
                        sys._def_mbr_Property(typeDef, p, property.get, property.set, property.supressBinding);
                    }
                }
            }
            if (members.methods)
            {
                var methods = members.methods;
                for (var m in methods)
                {
                    if (methods.hasOwnProperty(m))
                    {
                        var fn = methods[m];
                        if (util.isFunction(fn))
                            sys._def_mbr_Method(typeDef, m, fn);
                    }
                }
            }
            if (members.events)
            {
                var events = members.events;
                for (var e in events)
                {
                    if (events.hasOwnProperty(e))
                    {
                        sys._def_mbr_Event(typeDef, e);
                    }
                }
            }
        },

        _def_mbr_Method: function (typeDef, name, fn)
        {
            var _fn = {
                owner: typeDef,
                parent: typeDef.ancestor,
                name: name
            };
            if (typeDef.proto.hasOwnProperty(name))
                _fn.prior = typeDef.proto[name];

            var method = this._registerMethod(typeDef, name, fn);
            typeDef.proto[name] = function ()
            {
                var ofn = this._p.fn;
                this._p.fn = _fn;
                try
                {
                    return fn.apply(this, arguments);
                }
                catch (err)
                {
                    var msg = "Error on method " + method.uniqueName + " of " + this.getInstanceInfo() + ": " + (err || '').toString();
                    throw msg;
                }
                finally
                {
                    this._p.fn = ofn;
                }
            }
        },

        _def_mbr_Field: function (typeDef, name, defaultValue)
        {
            if (!util.isFunction(defaultValue))
                typeDef.proto[name] = defaultValue;
            this._registerField(typeDef, name, defaultValue);
        },

        _def_mbr_Property: function (typeDef, name, getter, setter, supressBinding)
        {
            if (!name)
                return;
            var fieldName = "$_" + name;
            uname = name.substr(0, 1).toUpperCase();
            if (name.length > 1)
                uname += name.substr(1);

            var getterName = "get" + uname + "Property";
            var setterName = "set" + uname + "Property";

            if (!getter)
                getter = function ()
                {
                    return this[fieldName];
                };

            if (!setter)
                setter = function (value)
                {
                    this[fieldName] = value;
                };

            //property
            var method = function PropertyAccess(value)
            {
                return sys._accessProperty.call(this, property, value);
            };

            //set
            var setMethod = function PropertySet(value)
            {
                this[name].call(this, value);
                return this;
            };

            //register
            var property = sys._registerProperty(typeDef, name, method, setMethod, getter, setter, supressBinding);

            // <name>([value])
            this._def_mbr_Method(typeDef, name, method);

            // set<name>(<value>)
            this._def_mbr_Method(typeDef, "set" + uname, setMethod);

            // set<name>Property
            // get<name>Property
            this._def_mbr_Method(typeDef, getterName, getter);
            this._def_mbr_Method(typeDef, setterName, setter);
        },

        _def_mbr_Event: function (typeDef, name)
        {
            var eventName = name.substr(0, 1).toUpperCase()
            if (name.length > 1)
                eventName += name.substr(1);

            this._registerEvent(typeDef, name);

            this._def_mbr_Method(typeDef, "trigger" + eventName, function ()
            {
                sys._triggerEvent(this, name, arguments);
                return this;
            });

            this._def_mbr_Method(typeDef, "on" + eventName, function (callback)
            {
                sys._addEventListener(this, name, callback);
                return this;
            });
        },

        _addEventListener: function (instance, eventName, callback)
        {
            var slot = instance._p.events[eventName];
            if (!slot)
                slot = instance._p.events[eventName] = {};
            var token = util.uid();
            slot[token] = callback;
            return token;
        },

        _triggerEvent: function (target, eventName, args)
        {
            var slot = target._p.events[eventName];
            if (slot)
            {
                for (var token in slot)
                    if (slot.hasOwnProperty(token))
                        slot[token].apply(target, args);
            }
        }
    };
    wui.Sys = sys;

    //root class
    sys._root = sys._define("W$Object", null, {
        methods: {
            initialize: function () { },

            getType: function ()
            {
                return this.prototype.type;
            },

            dataBind: function (data)
            {
                if (data !== undefined)
                    this.data(data);

                if (this._p.bindings)
                {
                    for (var b in this._p.bindings)
                    {
                        var binding = this._p.bindings[b];
                        if (binding)
                            this.processBinding(binding, undefined, true);
                    }
                }
            },

            processBinding: function (binding, customData, auto)
            {
                if (!binding.target)
                    throw "Binding target not specified.";
                // target
                var context = {};
                context.instance = this;
                context.binding = binding;

                if (binding.target.isW$Property)
                {
                    if (binding.target.supressBinding && auto)
                        return;
                    else
                    {
                        context.setter = function (value)
                        {
                            binding.target.setter.call(context.instance, value);
                        }
                    }
                }
                else if (binding.target.isW$Field)
                {
                    context.setter = function (value)
                    {
                        context.instance[binding.target.name] = value;
                    };
                }

                var data = customData || this.data(), value;
                src = binding.source;
                if (util.isEmpty(src))
                    value = data;
                else if (typeof src === "string")
                {
                    if (src.startsWith("@")) // starts with @ - is a 
                    {
                        //find the first ':'
                        var idx = src.indexOf(':');
                        if (idx > -1)
                        {
                            var binder = src.substr(1, idx - 2).toLowerCase();
                            var expr = src.substr(idx + 1);
                        }
                        else
                            binder = src.substr(1);


                        if (util.isEmpty(binder) || sys.binders[binder] == undefined)
                            throw "Binder not found: " + binder;

                        binder.resolve(context, expr);
                    }
                    else
                    {
                        value = data == undefined || data == null ? data : data[binding.source];
                    }
                }
                else if (util.isFunction(src))
                    value = src.call(this, data);

                if (binding.args.length && util.isFunction(binding.args[0]))
                    value = binding.args[0].call(this, value);

                context.setter.call(this, value);
            },

            hasOwnData: function ()
            {
                return this._dataContext !== undefined && this._dataContext !== null;
            },

            set: function (options)
            {
                if (!options)
                    return;
                for (var o in options)
                {
                    if (options.hasOwnProperty(o) && o !== undefined && options[o] !== undefined)
                    {
                        var value = options[o];
                        if (this[o] !== undefined && util.isFunction(this[o]))
                        {
                            this[o].call(this, value);
                        }
                        else
                            throw "Member not found: " + o;
                    }
                }
                return this;
            },

            getInstanceInfo: function ()
            {
                return "[" + this.type.fullname + "#" + this.iid + "]";
            }

        },
        properties: {
            data: {
                get: function ()
                {
                    var o = this;
                    while (o && o.isW$Object)
                    {
                        if (o._dataContext)
                            return o._dataContext;
                        else if (o.parent)
                            o = o.parent();
                        else
                            return undefined;
                    }
                    return undefined;
                },
                set: function (value)
                {
                    this._dataContext = value;
                }
            }
        },

        events: {
            propertyChanged: { triggered: null }
        }
    });


    //type definition
    var type = ctx.Type = {};
    type.define = function (name, ancestor, members)
    {
        util.assertEmptyArg(name, "name");
        if (ancestor)
        {
            ancestor = sys._resolveType(ancestor);
            if (!ancestor)
            {
                throw "Ancestor type not found: type def name: " + name;
            }
        }
        return sys._define(name, ancestor, members);
    };

    type.getTypes = function ()
    {
        return sys.defCache.typesList.slice(0);
    };

    type.getType = function (name)
    {
        return sys._getType(name);
    };

    type.isTypeOf = function (sourceTypeNameOrDef, targetTypeNameOrDef)
    {
        return sys._isTypeOf(sourceTypeNameOrDef, targetTypeNameOrDef);
    };

    // global functions
    ctx.Binding = function (options /*, [extra args] */)
    {
        if (options)
        {
            if (typeof options === "string" || util.isFunction(options))
                options = { source: options };
        }
        else
            options = {};
        options.isW$Binding = true;
        options.args = [];
        for (var i = 1; i < arguments.length; i++)
            options.args[i - 1] = arguments[i];
        return options;
    };

    // Services
    ctx.Type.define("wui.component.ServicesManager", null, {
        fields: {
            _parent: null,
            _ctx: null,
            _services: function () { return {}; }
        },
        methods: {
            initialize: function (ctx)
            {
                this._ctx = ctx;
                this._parent = util.getFromParentWindow("Services");
            },
            setParentManager: function (parent)
            {
                this._parent = parent;
            },
            register: function (name, instance)
            {
                this._services[name] = instance;
            },
            get: function (name)
            {
                var result = this._services[name];
                if (!result && this._parent)
                    return this._parent.get(name);
                return result;
            }
        }
    });

    ctx.Services = new wui.component.ServicesManager(ctx);
};
__wuiSysFn(window);

(function WuiDomHelper(ctx)
{
    var wui = ctx.wui;
    var util = wui.Util;

    var DOMHelper = ctx.DOMHelper = {};
    DOMHelper.addScriptRef = function (url, doc, data, onloadCallback)
    {
        var headID = doc.getElementsByTagName("head")[0];
        var scriptNode = doc.createElement('script');
        scriptNode.type = 'text/javascript';
        scriptNode.onload = function ()
        {
            if (onloadCallback)
                onloadCallback(data);
            this.onload = null;
        };
        scriptNode.src = url;
        headID.appendChild(scriptNode);
    };

    DOMHelper.addStyleRef = function (url, doc, data, onloadCallback)
    {
        var headID = doc.getElementsByTagName("head")[0];
        var cssNode = doc.createElement('link');
        cssNode.type = 'text/css';
        cssNode.rel = 'stylesheet';
        cssNode.href = url;
        cssNode.media = 'screen';
        headID.appendChild(cssNode);
        if (onloadCallback)
        {
            setTimeout(function ()
            {
                onloadCallback(data);
            }, 13);
        }
    };

    DOMHelper.getUnit = function (p)
    {
        if (typeof p === "string")
        {
            if (p.length)
            {
                var uaux = p.substr(p.length - 1, 1);
                var ulen = 1;
                switch (uaux)
                {
                    case "x": ulen = 2; break;

                    case "*":
                    case "%": ulen = 1; break;

                    default:
                        ulen = 2;
                        p += "px";
                        break;
                }
                uaux = p.substr(0, p.length - ulen);
                if (!uaux)
                    uaux = "0";
                return { value: parseFloat(uaux), unit: p.substr(p.length - ulen, ulen) };
            }
            else
                return { value: undefined, unit: undefined };
        }
        else
            return { value: p, unit: "px" };
    };

    DOMHelper.calculatePixels = function (value, unit, total, partial)
    {
        switch (unit)
        {
            case "px": return value;
            case "%": return value * total * 0.01;
            case "*": return value == 0 ? partial : value * partial * 0.01;
        }
    };

    DOMHelper.getMetrics = function(el){
        if (!el) return { offsetLeft: 0, offsetTop: 0, width: 0, height: 0 };
        if (el.isMetrics)
            return el;
        if (el.isUiControl)
            el = el.el;
        else
            el = $(el);
        var offset = el.offset();
        var metrics = {
            isMetrics: true,
            offsetTop: offset.top,
            offsetLeft: offset.left,
            width: el.width(),
            height: el.height()
        };
        return metrics;
    };

})(window);

(function WuiLoader(ctx)
{
    var wui = ctx.wui;
    var util = wui.Util;

    Type.define("wui.component.Loader", null, {
        fields: {
            _resources: function () { return {}; },
            _queue: function () { return []; },
            _nested: function () { return []; },
            _dependBuf: function () { return []; },
            _paused: false,
            _loading: false,
            _running: false,
            RES_STATUS_UNLOADED: 0,
            RES_STATUS_LOADING: 1,
            RES_STATUS_lOADED: 2
        },
        methods: {
            initialize: function(){
                this.key = util.uid();
            },
            mapResource: function (arg /* name, array or plain obj */, type, url, onloadCallback)
            {
                if (util.isArray(arg))
                {
                    for (var i = 0, item; (item = arg[i]); i++)
                        this.mapResource(item);
                }
                else if (typeof arg === "string")
                {
                    this._resources[arg] = {
                        name: arg,
                        type: type,
                        url: url,
                        callback: onloadCallback,
                        status: this.RES_STATUS_UNLOADED
                    };
                }
                else
                    this._resources[arg.name] = arg;
            },

            dependsOn: function (/* [<componentName>, ...] */)
            {
                for (var i = 0, arg; (arg = arguments[i]); i++)
                {
                    if (util.isArray(arg))
                        this.dependsOn.apply(this, arg);
                    else
                        this._dependBuf.push(arg);
                }
            },

            getResource: function (name)
            {
                var resource = this._resources[name];
                if (!resource)
                {
                    var parentLoader = util.getFromParentWindow("Loader");
                    if (parentLoader)
                    {
                        resource = parentLoader.getResource(name);
                        if (resource)
                        {
                            this.mapResource(resource.name, resource.type, resource.url, resource.callback);
                            return this._resources[name];
                        }
                        else
                            throw "Resource not found: " + name;
                    }
                }
                return resource;
            },

            run: function (label, fn)
            {
                if (util.isFunction(label))
                {
                    fn = label;
                    label = "[unlabelled]";
                };

                if (!fn)
                    fn = function () { };

                var data = {
                    label: label,
                    fn: fn,
                    depends: this._dependBuf,
                    _left: [],
                    _ready: false
                };

                if (this._loading || this._running)
                {
                    this._nested.push(data);
                    //console.log("Nested " + data.label);
                }
                else
                {
                    this._queue.push(data);
                    //console.log("Scheduled: " + data.label);
                }

                this._dependBuf = [];
                if (!this._loading && !this._running && this._queue.length === 1)
                    this.dispatchQueue();
            },
            dispatchQueue: function ()
            {
                if (!this._queueTimer)
                {
                    var loader = this;
                    this._queueTimer = setTimeout(function ()
                    {
                        loader._queueTimer = 0;
                        loader.processQueue();
                    }, 1);
                }
            },
            _done: function (res)
            {
                res.status = this.RES_STATUS_LOADED;
                this.triggerResourceLoaded(res);
                if (res.callback)
                    res.callback(res);
                this.dispatchQueue();
            },
            done: function (resName)
            {
                this._done(this.getResource(resName));
            },
            processQueue: function ()
            {
                if (this._paused || this._loading && this._running)
                    return;
                var item = this._queue[0];
                if (!item._prepared)
                {
                    //console.log("Prepare " + item.label);
                    item._prepared = true;
                    //prerare item.
                    for (var i = 0, dep; (dep = item.depends[i]); i++)
                    {
                        var res = this.getResource(dep);
                        if (!res)
                            throw "Resource not found, map name: " + dep;
                        if (res.status !== this.RES_STATUS_LOADED)
                            item._left.push(res);
                    }
                    item._ready = item._left.length == 0;
                }
                while (item._left.length)
                {
                    res = item._left[0];
                    if (res.status === this.RES_STATUS_LOADED)
                    {
                        item._left.shift();
                        item._ready = item._left.length == 0;
                        continue;
                    }
                    if (res.status === this.RES_STATUS_LOADING)
                        break;
                    else if (res.status === this.RES_STATUS_UNLOADED)
                    {
                        //console.log("Load " + res.url);
                        res.status = this.RES_STATUS_LOADING;
                        var loader = this;
                        if (res.type === "css")
                            DOMHelper.addStyleRef(res.url, ctx.document, res, function (data) { loader._done(res); });
                        else if (res.type === "js")
                        {
                            var doc = ctx.document,
                            headID = doc.getElementsByTagName("head")[0],
                            scriptNode = doc.createElement('script');
                            scriptNode.type = 'text/javascript';
                            scriptNode.src = "/Shell/GetScriptResource/?rn=" + res.name + "&rl=" + res.url;
                            headID.appendChild(scriptNode);
                        }
                        return;
                    }

                }
                if (item._ready)
                {
                    if (this._nested.length)
                    {
                        //inserir os itens internos na fila
                        for (i = 0; i < this._nested.length; i++)
                            this._queue.splice(i, 0, this._nested[i]);
                        this._nested = [];
                        // processar novamente
                        this.dispatchQueue();
                        return;
                    }
                    this._running = true;
                    try
                    {
                        //console.log("Exec: " + item.label);
                        item.fn.call(ctx, ctx);
                    }
                    catch (err)
                    {
                        var msg = "Error on run " + item.label + " : " + (err || '').toString();
                        throw msg;
                    }
                    finally
                    {
                        this._running = false;
                        this._queue.shift(); // retirar da fila
                    }
                }
                else
                    return;
                this._loading = false;
                if (this._queue.length)
                    this.dispatchQueue();
            }
        },

        events: {
            resourceLoaded: null
        }
    });

    Services.register("Loader", new wui.component.Loader());
    ctx.Loader = Services.get("Loader");
})(window);

// será liberado quando o jquery carregar
Loader._paused = true;
$(function ()
{
    Loader._paused = false;
    Loader.processQueue();
});


