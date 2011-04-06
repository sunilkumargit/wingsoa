using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Mvc;
using System.Web.Mvc;

namespace Wing.Modules.ChatServer.Controllers
{
    public class ChatController : AbstractController
    {
        public JsonResult Signin()
        {
            var chatServer = ServiceLocator.GetInstance<IChatServer>();
            var user = chatServer.SignInUser(CurrentUser.Login, CurrentUser.Name, CurrentUser.Schema.Name);
            return Json(new ChatLogonResult(user, chatServer.GetContacts(user)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Send(ChatSendMessageRequest msg)
        {
            var chatServer = ServiceLocator.GetInstance<IChatServer>();
            IChatRoom room = null;
            if (msg.RoomId.HasValue())
                room = chatServer.GetRoom(msg.RoomId);
            if (room == null)
                room = chatServer.GetRoomFor(msg.From, msg.To, true);
            room.PostMessage(msg.SenderKey, msg.From, msg.Message);
            return Json(new ChatSendMessageResult() { RoomId = room.RoomId }, JsonRequestBehavior.DenyGet);
        }
    }
}
