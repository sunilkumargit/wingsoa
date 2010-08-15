using System;
using System.Collections.Generic;

namespace Wing.ServiceLocation
{
    /// <summary>
    /// The generic Service Locator interface. This interface is used
    /// to retrieve services (instances identified by type and an [optional]
    /// name) from a container.
    /// </summary>
    public interface IServiceLocator : IServiceProvider
    {
        /// <summary>
        /// Get an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <exception cref="ActivationException">if there is an error resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        object GetInstance(Type serviceType);

        /// <summary>
        /// Get an instance of the given named <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <param name="key">Name the object was registered with.</param>
        /// <exception cref="ActivationException">if there is an error resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        object GetInstance(Type serviceType, string key);

        /// <summary>
        /// Get all instances of the given <paramref name="serviceType"/> currently
        /// registered in the container.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>A sequence of instances of the requested <paramref name="serviceType"/>.</returns>
        IEnumerable<object> GetAllInstances(Type serviceType);

        /// <summary>
        /// Get an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        TService GetInstance<TService>();

        /// <summary>
        /// Get an instance of the given named <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <param name="key">Name the object was registered with.</param>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        TService GetInstance<TService>(string key);

        /// <summary>
        /// Get all instances of the given <typeparamref name="TService"/> currently
        /// registered in the container.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>A sequence of instances of the requested <typeparamref name="TService"/>.</returns>
        IEnumerable<TService> GetAllInstances<TService>();

        /// <summary>
        /// Register a reference of the given <typeparamref name="TService"/> implemented by <typeparamref name="TImpl"/>
        /// </summary>
        /// <typeparam name="TService">Type representing the service in service locator (Usually an interface)</typeparam>
        /// <typeparam name="TImpl">The concrete type that implements <paramref name="TService"/></typeparam>
        /// <param name="key">The key for this service. Can be null.</param>
        /// <param name="asSingleton">Register this service as a singleton or not? If true, only one instance will be created on the first request and used for subsequent requests for this service.</param>
        void Register<TService, TImpl>(String key, bool asSingleton) where TImpl : TService;

        /// <summary>
        /// Register a transient reference of the given <typeparamref name="TService"/> implemented by <typeparamref name="TImpl"/>
        /// </summary>
        /// <typeparam name="TService">Type representing the service in service locator (Usually an interface)</typeparam>
        /// <typeparam name="TImpl">The concrete type that implements <paramref name="TService"/></typeparam>
        /// <param name="key">The key for this service. Can be null.</param>
        void Register<TService, TImpl>(String key) where TImpl : TService;

        /// <summary>
        /// Register a transient reference of the given <typeparamref name="TService"/> implemented by <typeparamref name="TImpl"/>
        /// </summary>
        /// <typeparam name="TService">Type representing the service in service locator (Usually an interface)</typeparam>
        /// <typeparam name="TImpl">The concrete type that implements <paramref name="TService"/></typeparam>
        void Register<TService, TImpl>() where TImpl : TService;

        /// <summary>
        /// Register a reference of the given <typeparamref name="TService"/> implemented by <typeparamref name="TImpl"/>
        /// </summary>
        /// <typeparam name="TService">Type representing the service in service locator (Usually an interface)</typeparam>
        /// <typeparam name="TImpl">The concrete type that implements <paramref name="TService"/></typeparam>
        /// <param name="asSingleton">Register this service as a singleton or not? If true, only one instance will be created on the first request and used for subsequent requests for this service.</param>
        void Register<TService, TImpl>(bool asSingleton) where TImpl : TService;

        /// <summary>
        /// Register a reference of the given <typeparamref name="service"/> implemented by <typeparamref name="impl"/>
        /// </summary>
        /// <param name="service">Type representing the service in service locator (Usually an interface)</param>
        /// <param name="impl">The concrete type that implements <paramref name="service"/></param>
        /// <param name="key">The key for this service. Can be null.</param>
        /// <param name="asSingleton">Register this service as a singleton or not? If true, only one instance will be created on the first request and used for subsequent requests for this service.</param>
        void Register(Type service, Type impl, String key, bool asSingleton);

        /// <summary>
        /// Register a transient reference of the given <typeparamref name="service"/> implemented by <typeparamref name="impl"/>
        /// </summary>
        /// <param name="service">Type representing the service in service locator (Usually an interface)</param>
        /// <param name="impl">The concrete type that implements <paramref name="service"/></param>
        /// <param name="key">The key for this service. Can be null.</param>
        void Register(Type service, Type impl, String key);

        /// <summary>
        /// Register a reference of the given <typeparamref name="service"/> implemented by <typeparamref name="impl"/>
        /// </summary>
        /// <param name="service">Type representing the service in service locator (Usually an interface)</param>
        /// <param name="impl">The concrete type that implements <paramref name="service"/></param>
        /// <param name="asSingleton">Register this service as a singleton or not? If true, only one instance will be created on the first request and used for subsequent requests for this service.</param>
        void Register(Type service, Type impl, bool asSingleton);

        /// <summary>
        /// Register a transient reference of the given <typeparamref name="service"/> implemented by <typeparamref name="impl"/>
        /// </summary>
        /// <param name="service">Type representing the service in service locator (Usually an interface)</param>
        /// <param name="impl">The concrete type that implements <paramref name="service"/></param>
        void Register(Type service, Type impl);

        /// <summary>
        /// Register a singleton instance of the given <typeparamref name="TService"/> implemented by <typeparamref name="impl"/>
        /// </summary>
        /// <param name="TService">Type representing the service in service locator (Usually an interface)</param>
        /// <param name="impl">The concrete type that implements <paramref name="service"/></param>
        /// <param name="key">The key for this service. Can be null.</param>
        void Register<TService>(TService impl, String key);

        /// <summary>
        /// Register a singleton instance of the given <typeparamref name="TService"/> implemented by <typeparamref name="impl"/>
        /// </summary>
        /// <param name="TService">Type representing the service in service locator (Usually an interface)</param>
        /// <param name="impl">The concrete type that implements <paramref name="service"/></param>
        void Register<TService>(TService impl);
    }
}
