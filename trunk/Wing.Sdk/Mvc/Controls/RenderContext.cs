using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;


namespace Wing.Mvc.Controls
{
    [System.Diagnostics.DebuggerStepThrough]
    public sealed class RenderContext
    {
        private static int _renderSessionIdGen = 0;

        private List<HtmlObject> _innerRenderStack;
        private StringBuilder _body = new StringBuilder();
        private StringWriter _script;
        private StringWriter _globalScript;
        private StyleLinkCollection _styleLinks;
        private int _renderSessionId;
        private ViewDataDictionary _viewData;

        public RenderContext(ControllerBase controller, ViewDataDictionary viewData = null)
        {
            this._renderSessionId = Interlocked.Increment(ref _renderSessionIdGen);
            this.Controller = controller;
            _innerRenderStack = new List<HtmlObject>();
            this.RenderStack = new ReadOnlyCollection<HtmlObject>(_innerRenderStack);
            this.Document = new HtmlTextWriter(new StringWriter(_body));
            this.Document.Indent = 3;
            _viewData = viewData ?? controller.ViewData;
        }

        private RenderContext(RenderContext parentContext)
        {
            this.ParentContext = parentContext;
            this.Controller = parentContext.Controller;
            this.RenderStack = parentContext.RenderStack;
            this._innerRenderStack = parentContext._innerRenderStack;
            this.Document = new HtmlTextWriter(new StringWriter(_body));
        }

        public Object Model { get { return ViewData.Model; } }
        public ViewDataDictionary ViewData { get { return _viewData; } }
        public String MapPath(string path)
        {
            return HttpContext.Server.MapPath(path);
        }
        public Object this[String key]
        {
            get { return ViewData[key]; }
            set { ViewData[key] = value; }
        }
        public HttpRequestBase Request
        {
            get { return HttpContext.Request; }
        }
        public HttpResponseBase Response
        {
            get { return HttpContext.Response; }
        }
        public String RawValueFromProvider(String key)
        {
            ValueProviderResult res = null;
            if (!Controller.ValueProvider.TryGetValue(key, out res) || res.RawValue == null)
                return "";
            if (res.RawValue.GetType().IsArray)
            {
                String strRes = "";
                Array arrayRes = (Array)res.RawValue;
                foreach (Object item in arrayRes)
                    strRes += item == null
                        ? ""
                        : (strRes.Length > 0
                            ? ";"
                            : "") + item.ToString();
                return strRes;
            }
            return res.RawValue.ToString();
        }
        public bool SetFlag(String flagName)
        {
            var flagKey = String.Format("__render_context_flag_" + flagName);
            var result = !this.ViewData[flagKey].As<bool>();
            this.ViewData[flagKey] = true;
            return result;
        }
        public ControllerBase Controller { get; private set; }
        public ControllerContext ControllerContext { get { return Controller.ControllerContext; } }
        public HttpContextBase HttpContext { get { return ControllerContext.HttpContext; } }
        public HttpSessionStateBase Session { get { return HttpContext.Session; } }
        public HtmlObject Root { get { return RenderStack.Count == 0 ? null : RenderStack[0]; } }
        public HtmlObject Parent { get { return RenderStack.Count < 2 ? null : RenderStack[RenderStack.Count - 2]; } }
        public HtmlObject FindParent(Predicate<HtmlObject> matchCriteria)
        {
            Assert.NullArgument(matchCriteria, "matchCriteria");
            var i = RenderStack.Count;
            while (--i > -1)
            {
                if (matchCriteria(RenderStack[i]))
                    return RenderStack[i];
            }
            return null;
        }
        public HtmlObject FindParent(Type type)
        {
            return FindParent((c) => type.IsAssignableFrom(c.GetType()));
        }
        public HtmlObject FindParent<TType>() where TType : HtmlControl
        {
            return FindParent(typeof(TType));
        }
        public HtmlObject FindParent(String id)
        {
            return FindParent((c) => id.Equals(c.Id));
        }
        public object CurrentDataItem
        {
            get
            {
                object result = null;
                for (var i = RenderStack.Count - 1; i >= 0 && result == null; i--)
                    result = RenderStack[i].DataContext;
                return result;
            }
        }
        public RenderContext ParentContext { get; private set; }
        public ReadOnlyCollection<HtmlObject> RenderStack { get; private set; }

        public HtmlTextWriter Document { get; private set; }
        public String BodyContent { get { return _body.ToString(); } }
        public TextWriter Script { get { return _script ?? (_script = new StringWriter()); } }
        public TextWriter GlobalScript { get { return _globalScript ?? (_globalScript = new StringWriter()); } }
        public StyleLinkCollection StyleLinks { get { return _styleLinks ?? (_styleLinks = new StyleLinkCollection()); } }
        internal int _renderizations = 0;

        public int Renderizations { get { return _renderizations; } }

        public RenderContext CreateChildContext()
        {
            return new RenderContext(this);
        }

        public void JoinToParent()
        {
            if (ParentContext != null)
            {
                ParentContext.Document.Write(this.BodyContent);
                if (_script != null)
                    ParentContext.Script.Write(_script.ToString());
                if (_globalScript != null)
                    ParentContext.GlobalScript.Write(_globalScript.ToString());
            }
        }

        internal void PushControl(HtmlObject control)
        {
            _innerRenderStack.Add(control);
        }
        internal void PopControl()
        {
            if (_innerRenderStack.Count > 0)
                _innerRenderStack.RemoveAt(_innerRenderStack.Count - 1);
        }

        public int RenderSessionId { get { return _renderSessionId; } }
    }

}
