using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Threading;

namespace Wing.Pipeline
{
    class MessageBufferItem : IPipelineMessageItem
    {
        private static int _msgId = 0;

        public MessageBufferItem(String messageId, Object data)
        {
            this.Id = Interlocked.Increment(ref _msgId);
            this.MessageId = messageId;
            this.Data = data;
        }

        public int Id { get; private set; }
        public string MessageId { get; private set; }
        public Object Data { get; private set; }
    }
}
