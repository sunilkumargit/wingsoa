using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Threading;

namespace Wing.Client.Sdk
{
    public static class TaskContext
    {
        private static List<Action> _actions = new List<Action>();
        private static Object _lockObject = new Object();
        private static AutoResetEvent _semaphore = new AutoResetEvent(false);
        private static bool _started = false;

        public static void Execute(Action action)
        {
            lock (_lockObject)
            {
                _actions.Add(action);
            }
            _semaphore.Set();
        }

        internal static void Start()
        {
            if (!_started)
                ThreadPool.QueueUserWorkItem((a) => ExecuteActions());
            _started = true;
        }

        private static void ExecuteActions()
        {
            bool wait = false;
            while (true)
            {
                if (wait)
                    _semaphore.WaitOne();
                Action item = null;
                lock (_lockObject)
                {
                    if (_actions.Count == 0)
                    {
                        wait = true;
                        continue;
                    }
                    item = _actions[0];
                    _actions.RemoveAt(0);
                }
                item();
                wait = false;
            }
        }
    }
}
