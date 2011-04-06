using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Wing.Client;

namespace Wing.Pipeline
{
    class PipelineManager : IPipelineManager
    {
        private Dictionary<string, OperationWrapper> _operations = new Dictionary<string, OperationWrapper>();
        private Dictionary<string, List<IPipelineMessageItem>> _messages = new Dictionary<string, List<IPipelineMessageItem>>();
        private Object __lockObject = new Object();

        private const string SESSION_SLOT_NAME = "_pipeline_session_";

        public void AddOperation(string name, Type paramType, Type resultType, Func<Object, IOperationContext, Object> func)
        {
            Assert.EmptyString(name, "name");
            Assert.NullArgument(paramType, "paramType");
            Assert.NullArgument(resultType, "resultType");
            Assert.NullArgument(func, "func");
            lock (__lockObject)
            {
                if (!_operations.ContainsKey(name))
                {
                    _operations[name] = new OperationWrapper(name, paramType, resultType, func);
                }
                else
                    throw new InvalidOperationException("Já existe um operação com o nome {0} no pipeline.".Templ(name));
            }
        }

        public void AddOperation<TParam, TResult>(string name, Func<TParam, IOperationContext, TResult> func)
        {
            AddOperation(name, typeof(TParam), typeof(TResult), (param, ctx) =>
                {
                    return (TResult)func.Invoke((TParam)param, ctx);
                });
        }

        public void SendMessage(IClientSession session, string messageId, object data)
        {
            lock (__lockObject)
            {
                List<IPipelineMessageItem> buffer = null;
                if (!_messages.TryGetValue(session.SessionId, out buffer))
                {
                    buffer = new List<IPipelineMessageItem>();
                    _messages[session.SessionId] = buffer;
                }
                buffer.Insert(0, new MessageBufferItem(messageId, data));
                if (buffer.Count > 511)
                    buffer.RemoveAt(buffer.Count - 1);
            }
        }

        public IPipelineMessageItem[] GetMessages(IClientSession session, ref int lastMsgSeq, out bool valid)
        {
            lock (__lockObject)
            {
                valid = true;
                List<IPipelineMessageItem> buffer = null;
                if (!_messages.TryGetValue(session.SessionId, out buffer)
                    || buffer.Count == 0
                    || buffer[0].Id == lastMsgSeq)
                    return null;
                else if (lastMsgSeq > 0 && buffer[buffer.Count - 1].Id > lastMsgSeq)
                {
                    valid = false;
                    return null;
                }
                var lst = lastMsgSeq;
                lastMsgSeq = buffer[0].Id;
                return buffer.TakeWhile(i => i.Id > lst).Reverse().ToArray();
            }
        }

        public void Broadcast(String messageId, object data)
        {
            foreach (var session in ServiceLocator.GetInstance<IClientSessionManager>().Sessions)
                session.SendMessage(messageId, data);
        }

        private object InvokeOperation(IClientSession session, string operation, object parameter, Func<OperationWrapper, OperationContext, Object> resultFunc)
        {
            Assert.NullArgument(session, "session");
            Assert.EmptyString(operation, "operation");
            Assert.NullArgument(resultFunc, "resultFunc");
            OperationWrapper wrapper = null;
            if (_operations.TryGetValue(operation, out wrapper))
            {
                var context = new OperationContext(session, wrapper, parameter);
                return resultFunc.Invoke(wrapper, context);
            }
            else
                throw new PipelineException("Operation not found: ".Templ(operation));

        }

        public object ExecuteOperation(IClientSession session, string operation, object parameter)
        {
            return InvokeOperation(session, operation, parameter, (op, ctx) =>
            {
                return op.Function.Invoke(parameter, ctx);
            });
        }

        public string ExecuteOperationJSON(IClientSession session, string operation, string jsonParam)
        {
            return InvokeOperation(session, operation, jsonParam, (op, ctx) =>
            {
                var jsSerializer = new JavaScriptSerializer();

                Object inputParam = null;
                // parse input param
                try
                {
                    inputParam = jsSerializer.Deserialize(jsonParam, op.ParamType);
                }
                catch (Exception ex)
                {
                    throw new PipelineException("Could not parse input param of operation {0}.".Templ(operation), ex);
                }

                // call 
                var result = op.Function.Invoke(inputParam, ctx);

                //serialize output param
                try
                {
                    result = jsSerializer.Serialize(result);
                }
                catch (Exception ex)
                {
                    throw new PipelineException("Could not parse output param of operation {0}.".Templ(operation), ex);
                }

                return result;
            }).AsString();
        }
    }
}
