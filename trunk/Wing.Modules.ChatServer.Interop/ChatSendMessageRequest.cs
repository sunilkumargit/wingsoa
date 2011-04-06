using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Modules.ChatServer
{
    public class ChatSendMessageRequest
    {
        public String RoomId { get; set; }
        public String From { get; set; }
        public String To { get; set; }
        public String Message { get; set; }
        public String SenderKey { get; set; }
    }
}
