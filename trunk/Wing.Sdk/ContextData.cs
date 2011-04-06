using System;
using System.Collections;
using System.Web;

namespace Wing
{
    [System.Diagnostics.DebuggerStepThrough]
    public static class ContextData
    {
        // Contexto de thread, usado em aplicações multi-thread ou single-thread que não estejam num
        // contexto Http.
        [ThreadStatic]
        private static Hashtable _threadContextItems;

        private const string HTTP_CTX_SLOT_NAME = "_context_data_items_";

        private static Hashtable InternalContext
        {
            get
            {
                if (HttpContext.Current == null)
                    return (_threadContextItems = _threadContextItems ?? new Hashtable());
                else
                {
                    if (HttpContext.Current.Items[HTTP_CTX_SLOT_NAME] == null)
                        HttpContext.Current.Items[HTTP_CTX_SLOT_NAME] = new Hashtable();
                    return (Hashtable)HttpContext.Current.Items[HTTP_CTX_SLOT_NAME];
                }
            }
        }


        public static Object Get(String key)
        {
            return InternalContext[key];
        }

        public static void Set(String key, Object value)
        {
            InternalContext[key] = value;
        }

        public static bool Exists(String key)
        {
            return InternalContext.ContainsKey(key);
        }

        public static int Count { get { return InternalContext.Count; } }

        public static void Remove(String key)
        {
            InternalContext.Remove(key);
        }

        public static void Clear()
        {
            InternalContext.Clear();
        }
    }
}
