using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Wing.Worker
{
    class WorkerServicesManager : IWorkerServicesManager
    {
        private Dictionary<String, IWorkerServiceController> _services = new Dictionary<string, IWorkerServiceController>();
        private List<WorkerServiceController> _processQueue = new List<WorkerServiceController>();
        private Object __lockObject = new object();
        private AutoResetEvent _processEvent = new AutoResetEvent(false);

        public IWorkerServiceController RegisterService(string serviceName, IWorkerService instance, IWorkerServiceRecoveryPolicy defaultRecoveryPolicy)
        {
            Assert.EmptyString(serviceName, "serviceName");
            Assert.NullArgument(instance, "instance");
            lock (__lockObject)
            {
                if (GetService(serviceName, false) != null)
                    throw new InvalidOperationException("Já existe um worker service registrado com este nome: " + serviceName);
                var controller = new WorkerServiceController(this, serviceName, instance, 
                    defaultRecoveryPolicy ?? new DefaultWorkerRecoveryPolicy());
                _services[controller.ServiceName] = controller;
                ScheduleProcess(controller);
            }
            return _services[serviceName];
        }

        private IWorkerServiceController GetService(String name, bool throwException)
        {
            IWorkerServiceController result = null;
            name = name.ToLower();
            if (!_services.TryGetValue(name, out result) && throwException)
                throw new Exception("Não existe um worker service com este nome: " + name);
            return result;
        }

        public void StartService(string serviceName)
        {
            Assert.EmptyString(serviceName, "serviceName");
            GetService(serviceName, true).Start();
        }

        public void StopService(string serviceName)
        {
            Assert.EmptyString(serviceName, "serviceName");
            GetService(serviceName, true).Stop();
        }

        public IEnumerable<IWorkerServiceController> GetRegisteredServices()
        {
            return _services.Values;
        }

        public void Start()
        {
            var thread = new Thread(ProcessLoop);
            thread.IsBackground = true;
            thread.Name = "Worker Services manager";
            thread.Start();
        }

        private void ProcessLoop()
        {
            WorkerServiceController controller = null;
            while (true)
            {
                _processEvent.WaitOne();
                while (_processQueue.Count > 0)
                {
                    lock (__lockObject)
                    {
                        if (_processQueue.Count == 0)
                            break;
                        controller = _processQueue[0];
                        _processQueue.RemoveAt(0);
                    }
                    switch (controller.Status)
                    {
                        case WorkerServiceStatus.NotInitialized:
                            controller.InitializeInternal();
                            _processQueue.AddIfNotExists(controller);
                            break;
                        case WorkerServiceStatus.Sleeping:
                            controller.ExecuteInternal();
                            break;
                    }
                }
            }
        }

        internal void ScheduleProcess(WorkerServiceController workerServiceController)
        {
            lock (__lockObject)
            {
                _processQueue.AddIfNotExists(workerServiceController);
            }
            _processEvent.Set();
        }
    }
}
