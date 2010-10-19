using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Wing.ServiceLocation;
using Wing.Client.Sdk;

namespace Wing.UnityServiceLocator
{
    [System.Diagnostics.DebuggerStepThrough]
    public class UnityServiceLocator : IServiceLocator
    {
        private IUnityContainer _container;

        public UnityServiceLocator(IUnityContainer container)
        {
            _container = container ?? new UnityContainer();
        }

        private TResult InvokeContainer<TResult>(Func<TResult> action)
        {
            return _container.Resolve<ISyncBroker>().Sync<TResult>(action);
        }

        #region IServiceLocator Members

        public object GetInstance(Type serviceType)
        {
            return InvokeContainer<Object>(() => _container.Resolve(serviceType));
        }

        public object GetInstance(Type serviceType, string key)
        {
            return InvokeContainer<Object>(() => _container.Resolve(serviceType, key));
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return InvokeContainer<IEnumerable<Object>>(() => _container.ResolveAll(serviceType));
        }

        public TService GetInstance<TService>()
        {
            return InvokeContainer<TService>(() => _container.Resolve<TService>());
        }

        public TService GetInstance<TService>(string key)
        {
            return InvokeContainer<TService>(() => _container.Resolve<TService>(key));
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return InvokeContainer<IEnumerable<TService>>(() => _container.ResolveAll<TService>());
        }

        public void Register<TService, TImpl>(string key, bool asSingleton) where TImpl : TService
        {
            _container.RegisterType<TService, TImpl>(key,
                asSingleton ? (LifetimeManager)(new ContainerControlledLifetimeManager()) : new TransientLifetimeManager());
        }

        public void Register<TService, TImpl>(string key) where TImpl : TService
        {
            Register<TService, TImpl>(key, false);
        }

        public void Register<TService, TImpl>() where TImpl : TService
        {
            Register<TService, TImpl>(false);
        }

        public void Register<TService, TImpl>(bool asSingleton) where TImpl : TService
        {
            _container.RegisterType<TService, TImpl>(asSingleton ? (LifetimeManager)(new ContainerControlledLifetimeManager()) : new TransientLifetimeManager());
        }

        public void Register(Type service, Type impl, string key, bool asSingleton)
        {
            _container.RegisterType(service, impl, key, asSingleton ? (LifetimeManager)(new ContainerControlledLifetimeManager()) : new TransientLifetimeManager());
        }

        public void Register(Type service, Type impl, string key)
        {
            Register(service, impl, key, false);
        }

        public void Register(Type service, Type impl, bool asSingleton)
        {
            _container.RegisterType(service, impl, asSingleton ? (LifetimeManager)(new ContainerControlledLifetimeManager()) : new TransientLifetimeManager());
        }

        public void Register(Type service, Type impl)
        {
            Register(service, impl, false);
        }

        public void Register<TService>(TService impl, string key)
        {
            _container.RegisterInstance<TService>(key, impl, new ContainerControlledLifetimeManager());
        }

        public void Register<TService>(TService impl)
        {
            _container.RegisterInstance<TService>(impl, new ContainerControlledLifetimeManager());
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            return GetInstance(serviceType);
        }

        #endregion
    }
}
