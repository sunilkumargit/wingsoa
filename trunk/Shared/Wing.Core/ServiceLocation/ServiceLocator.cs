namespace Wing.ServiceLocation
{
    /// <summary>
    /// This class provides the ambient container for this application. If your
    /// framework defines such an ambient container, use ServiceLocator.Current
    /// to get it.
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public static class ServiceLocator
    {
        private static ServiceLocatorProvider currentProvider;
        private static IServiceLocator currentLocator;

        /// <summary>
        /// The current ambient container.
        /// </summary>
        public static IServiceLocator GetCurrent()
        {
            return (currentLocator ?? (currentLocator = currentProvider()));
        }

        /// <summary>
        /// Set the delegate that is used to retrieve the current container.
        /// </summary>
        /// <param name="newProvider">Delegate that, when called, will return
        /// the current ambient container.</param>
        public static void SetLocatorProvider(ServiceLocatorProvider newProvider)
        {
            currentProvider = newProvider;
        }

        /// <summary>
        /// Get an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <exception cref="ActivationException">if there is an error resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        public static object GetInstance(System.Type serviceType)
        {
            return GetCurrent().GetInstance(serviceType);
        }

        /// <summary>
        /// Get an instance of the given named <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <param name="key">Name the object was registered with.</param>
        /// <exception cref="ActivationException">if there is an error resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        public static object GetInstance(System.Type serviceType, string key)
        {
            return GetCurrent().GetInstance(serviceType, key);
        }

        /// <summary>
        /// Get all instances of the given <paramref name="serviceType"/> currently
        /// registered in the container.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>A sequence of instances of the requested <paramref name="serviceType"/>.</returns>
        public static System.Collections.Generic.IEnumerable<object> GetAllInstances(System.Type serviceType)
        {
            return GetCurrent().GetAllInstances(serviceType);
        }

        /// <summary>
        /// Get an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        public static TService GetInstance<TService>()
        {
            return GetCurrent().GetInstance<TService>();
        }

        /// <summary>
        /// Get an instance of the given named <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <param name="key">Name the object was registered with.</param>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        public static TService GetInstance<TService>(string key)
        {
            return GetCurrent().GetInstance<TService>(key);
        }

        /// <summary>
        /// Get all instances of the given <typeparamref name="TService"/> currently
        /// registered in the container.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>A sequence of instances of the requested <typeparamref name="TService"/>.</returns>
        public static System.Collections.Generic.IEnumerable<TService> GetAllInstances<TService>()
        {
            return GetCurrent().GetAllInstances<TService>();
        }

        /// <summary>
        /// Register a reference of the given <typeparamref name="TService"/> implemented by <typeparamref name="TImpl"/>
        /// </summary>
        /// <typeparam name="TService">Type representing the service in service locator (Usually an interface)</typeparam>
        /// <typeparam name="TImpl">The concrete type that implements <paramref name="TService"/></typeparam>
        /// <param name="key">The key for this service. Can be null.</param>
        /// <param name="asSingleton">Register this service as a singleton or not? If true, only one instance will be created on the first request and used for subsequent requests for this service.</param>
        public static void Register<TService, TImpl>(string key, bool asSingleton) where TImpl : TService
        {
            GetCurrent().Register<TService, TImpl>(key, asSingleton);
        }

        /// <summary>
        /// Register a transient reference of the given <typeparamref name="TService"/> implemented by <typeparamref name="TImpl"/>
        /// </summary>
        /// <typeparam name="TService">Type representing the service in service locator (Usually an interface)</typeparam>
        /// <typeparam name="TImpl">The concrete type that implements <paramref name="TService"/></typeparam>
        /// <param name="key">The key for this service. Can be null.</param>
        public static void Register<TService, TImpl>(string key) where TImpl : TService
        {
            GetCurrent().Register<TService, TImpl>(key);
        }

        /// <summary>
        /// Register a transient reference of the given <typeparamref name="TService"/> implemented by <typeparamref name="TImpl"/>
        /// </summary>
        /// <typeparam name="TService">Type representing the service in service locator (Usually an interface)</typeparam>
        /// <typeparam name="TImpl">The concrete type that implements <paramref name="TService"/></typeparam>
        public static void Register<TService, TImpl>() where TImpl : TService
        {
            GetCurrent().Register<TService, TImpl>();
        }

        /// <summary>
        /// Register a reference of the given <typeparamref name="TService"/> implemented by <typeparamref name="TImpl"/>
        /// </summary>
        /// <typeparam name="TService">Type representing the service in service locator (Usually an interface)</typeparam>
        /// <typeparam name="TImpl">The concrete type that implements <paramref name="TService"/></typeparam>
        /// <param name="asSingleton">Register this service as a singleton or not? If true, only one instance will be created on the first request and used for subsequent requests for this service.</param>
        public static void Register<TService, TImpl>(bool asSingleton) where TImpl : TService
        {
            GetCurrent().Register<TService, TImpl>(asSingleton);
        }

        /// <summary>
        /// Register a reference of the given <typeparamref name="service"/> implemented by <typeparamref name="impl"/>
        /// </summary>
        /// <param name="service">Type representing the service in service locator (Usually an interface)</param>
        /// <param name="impl">The concrete type that implements <paramref name="service"/></param>
        /// <param name="key">The key for this service. Can be null.</param>
        /// <param name="asSingleton">Register this service as a singleton or not? If true, only one instance will be created on the first request and used for subsequent requests for this service.</param>
        public static void Register(System.Type service, System.Type impl, string key, bool asSingleton)
        {
            GetCurrent().Register(service, impl, key, asSingleton);
        }

        /// <summary>
        /// Register a transient reference of the given <typeparamref name="service"/> implemented by <typeparamref name="impl"/>
        /// </summary>
        /// <param name="service">Type representing the service in service locator (Usually an interface)</param>
        /// <param name="impl">The concrete type that implements <paramref name="service"/></param>
        /// <param name="key">The key for this service. Can be null.</param>
        public static void Register(System.Type service, System.Type impl, string key)
        {
            GetCurrent().Register(service, impl, key);
        }

        /// <summary>
        /// Register a reference of the given <typeparamref name="service"/> implemented by <typeparamref name="impl"/>
        /// </summary>
        /// <param name="service">Type representing the service in service locator (Usually an interface)</param>
        /// <param name="impl">The concrete type that implements <paramref name="service"/></param>
        /// <param name="asSingleton">Register this service as a singleton or not? If true, only one instance will be created on the first request and used for subsequent requests for this service.</param>
        public static void Register(System.Type service, System.Type impl, bool asSingleton)
        {
            GetCurrent().Register(service, impl, asSingleton);
        }

        /// <summary>
        /// Register a transient reference of the given <typeparamref name="service"/> implemented by <typeparamref name="impl"/>
        /// </summary>
        /// <param name="service">Type representing the service in service locator (Usually an interface)</param>
        /// <param name="impl">The concrete type that implements <paramref name="service"/></param>
        public static void Register(System.Type service, System.Type impl)
        {
            GetCurrent().Register(service, impl);
        }

        /// <summary>
        /// Register a singleton instance of the given <typeparamref name="TService"/> implemented by <typeparamref name="impl"/>
        /// </summary>
        /// <param name="TService">Type representing the service in service locator (Usually an interface)</param>
        /// <param name="impl">The concrete type that implements <paramref name="service"/></param>
        /// <param name="key">The key for this service. Can be null.</param>
        public static void Register<TService>(TService impl, string key)
        {
            GetCurrent().Register<TService>(impl, key);
        }

        /// <summary>
        /// Register a singleton instance of the given <typeparamref name="TService"/> implemented by <typeparamref name="impl"/>
        /// </summary>
        /// <param name="TService">Type representing the service in service locator (Usually an interface)</param>
        /// <param name="impl">The concrete type that implements <paramref name="service"/></param>
        public static void Register<TService>(TService impl)
        {
            GetCurrent().Register<TService>(impl);
        }


        /// <summary>
        /// Get an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <exception cref="ActivationException">if there is an error resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        public static object GetService(System.Type serviceType)
        {
            return GetInstance(serviceType);
        }
    }
}
