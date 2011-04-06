using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace Wing.Host
{
    public partial class Restart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            File.SetLastWriteTime(Server.MapPath("~/web.config"), DateTime.Now);
        }
    }
}