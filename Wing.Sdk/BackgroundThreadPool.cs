using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Wing
{
    public static class BackgroundThreadPool
    {
        private static AutoResetEvent _queueEventHandler = new AutoResetEvent(false);
        private static AutoResetEvent _availableEventHandler = new AutoResetEvent(false);
        private static Queue<Action> _execQueue = new Queue<Action>();
        private static Object __lockObject = new Object();
        private static Queue<ThreadWrapper> _availableThreads = new Queue<ThreadWrapper>();
        private static bool _stopped = false;
        private static int _minThreads = 3;
        private static int _maxThreads = 10;
        private static int WAIT_AVAILABLE_INTERVAL = 500;
        internal volatile static int _currentThreadCount = 0;

        static BackgroundThreadPool()
        {
            /*
            var thread = new Thread(Process);
            thread.Name = "BackgroundThreadPool Manager";
            thread.IsBackground = true;
            thread.Start();
            */
        }

        private static void Process()
        {
            while (!_stopped)
            {
                _queueEventHandler.WaitOne();
                while (_execQueue.Count > 0)
                {
                    Action action = null;
                    lock (__lockObject)
                    {
                        if (_execQueue.Count > 0)
                        {
                            action = _execQueue.Dequeue();
                        }
                    }
                    if (action == null)
                        break;
                    ThreadWrapper wrapper = null;
                    lock (__lockObject)
                    {
                        if (_availableThreads.Count > 0)
                            wrapper = _availableThreads.Dequeue();
                        else if (_currentThreadCount < _maxThreads)
                            wrapper = CreateWrapper();
                    }
                    if (wrapper != null)
                        wrapper.DoWork(action);
                }
            }
        }

        private static ThreadWrapper CreateWrapper()
        {
            _currentThreadCount++;
            return new ThreadWrapper();
        }

        public static void Enqueue(Action action)
        {
            ThreadPool.QueueUserWorkItem((state) => action());
            /*
            Assert.NullArgument(action, "action");
            lock (__lockObject)
            {
                _execQueue.Enqueue(action);
            }
            _queueEventHandler.Set();
             */
        }

        private static void WorkDone(ThreadWrapper wrapper)
        {
            lock (__lockObject)
            {
                _availableThreads.Enqueue(wrapper);
            }
            _availableEventHandler.Set();
        }

        private static void Faulted(ThreadWrapper wrapper)
        {
            _currentThreadCount--;
        }

        private class ThreadWrapper
        {
            private AutoResetEvent _waitHandle = new AutoResetEvent(false);
            private Action _action = null;

            public ThreadWrapper()
            {
                var thread = new Thread(ProcessWork);
                thread.Name = "BackgroundThreadPool worker thread #" + BackgroundThreadPool._currentThreadCount.ToString();
                thread.IsBackground = true;
            }

            private void ProcessWork()
            {
                Exception _currentException = null;
                while (!BackgroundThreadPool._stopped)
                {
                    _waitHandle.WaitOne();
                    try
                    {
                        _action.Invoke();
                        BackgroundThreadPool.WorkDone(this);
                    }
                    catch (Exception ex)
                    {
                        _currentException = ex;
                        BackgroundThreadPool.Faulted(this);
                        break;
                    }
                }
                if (_currentException != null)
                    throw _currentException;
            }

            public void DoWork(Action action)
            {
                _action = action;
                _waitHandle.Set();
            }
        }
    }
}
