﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace Wing.Client.Sdk
{
    public static class WorkContext
    {
        private static List<Action> _actions = new List<Action>();
        private static Object _lockObject = new Object();
        private static AutoResetEvent _semaphore = new AutoResetEvent(false);
        private static bool _started = false;

        public static void Sync(Action action)
        {
            lock (_lockObject)
            {
                _actions.Add(action);
            }
            _semaphore.Set();
        }

        public static void Async(Action action)
        {
            ThreadPool.QueueUserWorkItem((a) => action());
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
