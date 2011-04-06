using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Wing.Pipeline;
using System.Web.Script.Serialization;

namespace Wing.Mvc.Controllers
{
    public class ShellController : AbstractController
    {
        private void SetUser()
        {
            ViewData["userId"] = CurrentUser.Login;
            ViewData["username"] = CurrentUser.Name;
            ViewData["email"] = CurrentUser.Email;
        }

        public ActionResult Index()
        {
            if (!IsLoggedIn)
                return RedirectToAction("Index", "Login", new { fwd = "/Shell", usr = "system", pwd = "" });
            SetUser();
            return View("ShellLoader");
        }

        public ActionResult ContentView()
        {
            SetUser();
            return View("ShellContentView");
        }

        #region PIPELINE
        private void AddMessages(PipelineResultPack pack, IPipelineMessageItem[] messages)
        {
            if (messages == null || messages.Length == 0)
                return;
            var jsSerializer = new JavaScriptSerializer();
            foreach (var message in messages)
            {
                var resultItem = new PipelineResultPackItem();
                resultItem.type = "msg";
                resultItem.opname = message.MessageId;
                resultItem.opid = message.Id.ToString();

                //converter os dados
                try
                {
                    resultItem.data = jsSerializer.Serialize(message.Data);
                }
                catch (Exception ex)
                {
                    resultItem.error = true;
                    resultItem.message = "Could not convert message data of message {0} to JSON format, exception: {1}".Templ(message.MessageId, ex.ToString());
                }

                pack.data.Add(resultItem);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SyncPipe(PipelineRequestPack pack)
        {
            if (pack.id == 0)
                return null;
            else
            {
                var result = new PipelineResultPack() { id = pack.id };
                var pipeline = ServiceLocator.GetInstance<IPipelineManager>();
                var msgSeq = pack.lastMsgSeq;

                var valid = false;
                AddMessages(result, pipeline.GetMessages(ClientSession.Current, ref msgSeq, out valid));

                //processar o pack
                foreach (var dataItem in pack.data)
                {
                    if (dataItem.type == "opcall")
                    {
                        var resultItem = new PipelineResultPackItem()
                        {
                            type = "opresult",
                            opid = dataItem.opid,
                            opname = dataItem.opname
                        };
                        result.data.Add(resultItem);

                        try
                        {
                            resultItem.data = pipeline.ExecuteOperationJSON(ClientSession.Current, dataItem.opname, dataItem.data);
                        }
                        catch (Exception ex)
                        {
                            resultItem.error = true;
                            resultItem.message = ex.ToString();
                        }
                    }
                }

                AddMessages(result, pipeline.GetMessages(ClientSession.Current, ref msgSeq, out valid));
                return Json(result);
            }
        }

        public class PipelineRequestPack
        {
            public int id;
            public int lastMsgSeq;
            public List<PipelineRequestPackDataItem> data = new List<PipelineRequestPackDataItem>();
        }

        public struct PipelineRequestPackDataItem
        {
            public string type;
            public string opname;
            public string opid;
            public string data;
        }

        public class PipelineResultPack
        {
            public int id;
            public List<PipelineResultPackItem> data = new List<PipelineResultPackItem>();
        }

        public class PipelineResultPackItem
        {
            public string type;
            public string opid;
            public string opname;
            public string data;
            public bool error;
            public string message;
        }
        #endregion

        #region RESOURCES
        public ActionResult GetScriptResource(String rn, String rl)
        {
            var resourceName = rn;
            var resourceLocation = rl;
            var filePath = Server.MapPath(rl);
            var fileContents = System.IO.File.ReadAllText(filePath);
            var result = new StringBuilder();
            result.AppendLine("try { ");
            result.AppendLine(fileContents);
            result.AppendLine("}");
            result.AppendLine("catch (err) { throw 'Error on load script " + rn + ": ' + (err || '').toString(); }");
            result.AppendLine("finally { Loader.done('" + rn + "'); }");
            return File(Encoding.UTF8.GetBytes(result.ToString()), "text/javascript", String.Format("{0}.js", rn));
        }
        #endregion
    }
}
