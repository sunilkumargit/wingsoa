using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;
using System.Web.Script.Serialization;

namespace Wing.Mvc.Binders
{
    public class DefaultJSONModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (!IsJSONRequest(controllerContext))
            {
                return base.BindModel(controllerContext, bindingContext);
            }

            // Get the JSON data that's been posted
            var request = controllerContext.HttpContext.Request;
            var jsonStringData = new StreamReader(request.InputStream).ReadToEnd();

            // Use the built-in serializer to do the work for us
            return new JavaScriptSerializer()
                .Deserialize(jsonStringData, bindingContext.ModelMetadata.ModelType);
        }

        private static bool IsJSONRequest(ControllerContext controllerContext)
        {
            var contentType = controllerContext.HttpContext.Request.ContentType ?? "";
            return contentType.Contains("application/json");
        }
    }
}
