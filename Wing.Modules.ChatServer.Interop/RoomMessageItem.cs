using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Modules.ChatServer
{
    public class RoomMessageItem
    {
        public String SenderKey { get; set; }
        public String RoomId { get; set; }
        public String Sender { get; set; }
        public String To { get; set; }
        public String Message { get; set; }
    }
}
