using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Client;

namespace Wing.Pipeline
{
    public interface IPipelineManager
    {
        void AddOperation(String name, Type paramType, Type resultType, Func<Object, IOperationContext, Object> func);
        void AddOperation<TParam, TResult>(String name, Func<TParam, IOperationContext, TResult> func);

        void SendMessage(IClientSession session, String messageId, Object data);
        void Broadcast(string messageId, Object value);

        Object ExecuteOperation(IClientSession session, String operation, Object parameter);
        String ExecuteOperationJSON(IClientSession session, String operation, String jsonParam);


        /// <summary>
        /// Returns the messages that are in session message buffer and clear the buffer.
        /// </summary>
        /// <returns></returns>
        IPipelineMessageItem[] GetMessages(IClientSession session, ref int lastMessageSeq, out bool valid);
    }
}
