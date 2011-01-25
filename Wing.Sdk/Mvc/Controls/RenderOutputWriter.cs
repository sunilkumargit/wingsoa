using System;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;

namespace Wing.Mvc.Controls
{
    [System.Diagnostics.DebuggerStepThrough]
    public class RenderOutputWriter
    {
        private bool _writeLog;
        public RenderContext Context { get; private set; }

        private TimeSpan RenderCost { get; set; }

        public RenderOutputWriter(ControllerBase controller, ViewDataDictionary viewData = null, bool writeLog = true)
        {
            Context = new RenderContext(controller, viewData);
            _writeLog = writeLog;
        }

        public void Render(HtmlObject control)
        {
            try
            {
                var renderStart = DateTime.Now;
                control.Render(Context);
                RenderCost = DateTime.Now - renderStart;
            }
            catch (Exception e)
            {
                //gravar na saida mais dados sobre o erro.
                Exception ex = new Exception(String.Format("Erro ao renderizar o controle: \n Pilha: {0}", GetStackString()), e);
                throw ex;
            }
        }

        private String GetStackString()
        {
            String res = "";
            foreach (HtmlObject control in Context.RenderStack)
                res += res.Length == 0 ? control.GetType().Name : " |> " + control.GetType().Name;
            return res;
        }

        public void WriteStyles()
        {
            if (Context.StyleLinks.Count == 0)
                return;
            var respHtml = new HtmlTextWriter(Context.Response.Output);

            foreach (var style in Context.StyleLinks)
            {
                if (!String.IsNullOrEmpty(style.Content))
                {
                    respHtml.AddAttribute("type", "text/css");
                    if (!String.IsNullOrEmpty(style.Media))
                        respHtml.AddAttribute("media", style.Media);
                    respHtml.RenderBeginTag("style");
                    respHtml.Write(style.Content);
                    respHtml.RenderEndTag();
                }
                else if (!String.IsNullOrEmpty(style.Url))
                {
                    respHtml.AddAttribute("rel", "stylesheet");
                    if (!String.IsNullOrEmpty(style.Media))
                        respHtml.AddAttribute("media", style.Media);
                    respHtml.AddAttribute("href", style.Url);
                    respHtml.RenderBeginTag("link");
                    respHtml.RenderEndTag();
                }
                else if (!String.IsNullOrEmpty(style.FromFile))
                {
                    respHtml.AddAttribute("type", "text/css");
                    if (!String.IsNullOrEmpty(style.Media))
                        respHtml.AddAttribute("media", style.Media);
                    respHtml.RenderBeginTag("style");
                    respHtml.Write(File.ReadAllText(Context.MapPath(style.FromFile)));
                    respHtml.RenderEndTag();
                }
            }

        }

        public void WriteScripts()
        {
            var respHtml = new HtmlTextWriter(Context.Response.Output);

            //styles
            WriteStyles();

            var gblScript = Context.GlobalScript.ToString();
            var pgScript = Context.Script.ToString();

            if (!String.IsNullOrEmpty(gblScript) || !String.IsNullOrEmpty(pgScript))
            {
                respHtml.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
                respHtml.RenderBeginTag("script");
                respHtml.WriteLine(gblScript);
                respHtml.WriteLine("$(document).ready(function(){");
                respHtml.WriteLine(pgScript);
                respHtml.WriteLine("});");
                respHtml.RenderEndTag();
            }
        }

        public void WriteBodyContent()
        {
            Context.Response.Write(Context.BodyContent);
#if !DEBUG
            if (_writeLog)
            {
#endif
            Context.Response.Write(String.Format("<!-- cost: {0}, length: {1}, renderizations: {2}, session id: {3} -->",
                this.RenderCost.TotalMilliseconds,
                this.Context.BodyContent.Length,
                this.Context.Renderizations,
                this.Context.RenderSessionId));
#if !DEBUG
            }
#endif
        }
    }
}