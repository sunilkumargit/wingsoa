//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System;
using System.Globalization;
using Wing.Logging;
using Wing.Services;


namespace Wing.Modularity
{
    /// <summary>
    /// Implements the <see cref="IModuleInitializer"/> interface. Handles loading of a module based on a type.
    /// </summary>
    public class ModuleInitializer : IModuleInitializer
    {
        private readonly IServiceLocator serviceLocator;
        private readonly ILogger loggerFacade;

        /// <summary>
        /// Initializes a new instance of <see cref="ModuleInitializer"/>.
        /// </summary>
        /// <param name="serviceLocator">The container that will be used to resolve the modules by specifying its type.</param>
        /// <param name="loggerFacade">The logger to use.</param>
        public ModuleInitializer(IServiceLocator serviceLocator)
        {
            if (serviceLocator == null)
            {
                throw new ArgumentNullException("serviceLocator");
            }

            this.serviceLocator = serviceLocator;
            this.loggerFacade = serviceLocator.GetInstance<ILogManager>().GetSystemLogger();
        }

        protected void ExecuteModuleMethod(ModuleInfo moduleInfo, Action method)
        {
            try
            {
                method();
            }
            catch (Exception ex)
            {
                this.HandleModuleInitializationError(
                    moduleInfo,
                    moduleInfo.ModuleInstance != null ? moduleInfo.ModuleInstance.GetType().Assembly.FullName : null,
                    ex);
            }
        }

        /// <summary>
        /// Initializes the specified module.
        /// </summary>
        /// <param name="moduleInfo">The module to initialize</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catches Exception to handle any exception thrown during the initialization process with the HandleModuleInitializationError method.")]
        public void Initialize(ModuleInfo moduleInfo)
        {
            if (moduleInfo.ModuleInstance == null)
                moduleInfo.ModuleInstance = this.CreateModule(moduleInfo.ModuleType);

            ExecuteModuleMethod(moduleInfo, moduleInfo.ModuleInstance.Initialize);
        }

        public void RunModule(ModuleInfo moduleInfo)
        {
            if (moduleInfo.ModuleInstance == null)
                throw new ModuleInitializeException(String.Format("The module {0} has no instance", moduleInfo.ModuleName));
            if (moduleInfo.State == ModuleState.Initialized)
                ExecuteModuleMethod(moduleInfo, moduleInfo.ModuleInstance.Run);
        }

        /// <summary>
        /// Handles any exception ocurred in the module Initialization process,
        /// logs the error using the <seealso cref="ILoggerFacade"/> and throws a <seealso cref="ModuleInitializeException"/>.
        /// This method can be overriden to provide a different behavior. 
        /// </summary>
        /// <param name="moduleInfo">The module metadata where the error happenened.</param>
        /// <param name="assemblyName">The assembly name.</param>
        /// <param name="exception">The exception thrown that is the cause of the current error.</param>
        /// <exception cref="ModuleInitializeException"></exception>
        public virtual void HandleModuleInitializationError(ModuleInfo moduleInfo, string assemblyName, Exception exception)
        {
            Exception moduleException;

            if (exception is ModuleInitializeException)
            {
                moduleException = exception;
            }
            else
            {
                if (!string.IsNullOrEmpty(assemblyName))
                {
                    moduleException = new ModuleInitializeException(moduleInfo.ModuleName, assemblyName, exception.Message, exception);
                }
                else
                {
                    moduleException = new ModuleInitializeException(moduleInfo.ModuleName, exception.Message, exception);
                }
            }

            this.loggerFacade.LogException(String.Format("Error on {0} module {1}",
                    moduleInfo.State == ModuleState.Initializing ? "initialize" : "run",
                    moduleInfo.ModuleName), moduleException, Priority.High);
            throw moduleException;
        }

        /// <summary>
        /// Uses the container to resolve a new <see cref="IModule"/> by specifying its <see cref="Type"/>.
        /// </summary>
        /// <param name="typeName">The type name to resolve. This type must implement <see cref="IModule"/>.</param>
        /// <returns>A new instance of <paramref name="typeName"/>.</returns>
        protected virtual IModule CreateModule(string typeName)
        {
            Type moduleType = Type.GetType(typeName);
            if (moduleType == null)
            {
                throw new ModuleInitializeException(string.Format(CultureInfo.CurrentCulture, Messages.FailedToGetType, typeName));
            }

            return (IModule)this.serviceLocator.GetInstance(moduleType);
        }
    }
}
