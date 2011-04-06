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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Wing.Logging;


namespace Wing.Modularity
{
    /// <summary>
    /// Component responsible for coordinating the modules' type loading and module initialization process. 
    /// </summary>
    public partial class ModuleManager : IModuleManager
    {
        private readonly IModuleInitializer moduleInitializer;
        private readonly IModuleCatalog moduleCatalog;
        private readonly ILogger loggerFacade;
        private IEnumerable<IModuleTypeLoader> typeLoaders;
        private int _orderCount = 0;
        private bool _initialized = false;

        /// <summary>
        /// Initializes an instance of the <see cref="ModuleManager"/> class.
        /// </summary>
        /// <param name="moduleInitializer">Service used for initialization of modules.</param>
        /// <param name="moduleCatalog">Catalog that enumerates the modules to be loaded and initialized.</param>
        /// <param name="loggerFacade">Logger used during the load and initialization of modules.</param>
        public ModuleManager(IModuleInitializer moduleInitializer, IModuleCatalog moduleCatalog)
        {
            if (moduleInitializer == null)
            {
                throw new ArgumentNullException("moduleInitializer");
            }

            if (moduleCatalog == null)
            {
                throw new ArgumentNullException("moduleCatalog");
            }

            this.moduleInitializer = moduleInitializer;
            this.moduleCatalog = moduleCatalog;
            this.loggerFacade = ServiceLocator.GetInstance<ILogManager>().GetSystemLogger();
        }

        /// <summary>
        /// Initializes the modules marked as <see cref="InitializationMode.WhenAvailable"/> on the <see cref="ModuleCatalog"/>.
        /// </summary>
        public void Run()
        {
            lock (this)
            {
                if (!_initialized)
                {
                    this.moduleCatalog.Initialize();
                    _initialized = true;
                }
                this.LoadModules();
            }
        }

        /// <summary>
        /// Initializes the module on the <see cref="ModuleCatalog"/> with the name <paramref name="moduleName"/>.
        /// </summary>
        /// <param name="moduleName">Name of the module requested for initialization.</param>
        public void LoadModule(string moduleName)
        {
            IEnumerable<ModuleInfo> module = this.moduleCatalog.Modules.Where(m => m.ModuleName == moduleName);
            if (module == null || module.Count() != 1)
            {
                throw new ModuleNotFoundException(moduleName, string.Format(CultureInfo.CurrentCulture, Messages.ModuleNotFound, moduleName));
            }

            IEnumerable<ModuleInfo> modulesToLoad = this.moduleCatalog.CompleteListWithDependencies(module);

            this.LoadModuleTypes(modulesToLoad);
        }

        /// <summary>
        /// Handles any exception ocurred in the module typeloading process,
        /// logs the error using the <seealso cref="ILoggerFacade"/> and throws a <seealso cref="ModuleTypeLoadingException"/>.
        /// This method can be overriden to provide a different behavior. 
        /// </summary>
        /// <param name="moduleInfo">The module metadata where the error happenened.</param>
        /// <param name="exception">The exception thrown that is the cause of the current error.</param>
        /// <exception cref="ModuleTypeLoadingException"></exception>
        protected virtual void HandleModuleTypeLoadingError(ModuleInfo moduleInfo, Exception exception)
        {
            ModuleTypeLoadingException moduleTypeLoadingException = exception as ModuleTypeLoadingException;

            if (moduleTypeLoadingException == null)
            {
                moduleTypeLoadingException = new ModuleTypeLoadingException(moduleInfo.ModuleName, exception.Message, exception);
            }

            this.loggerFacade.Log(moduleTypeLoadingException.Message, Category.Exception, Priority.High);

            throw moduleTypeLoadingException;
        }

        private void LoadModules()
        {
            IEnumerable<ModuleInfo> modulesToLoadTypes = this.moduleCatalog.CompleteListWithDependencies(moduleCatalog.Modules);
            if (modulesToLoadTypes != null)
                this.LoadModuleTypes(modulesToLoadTypes);
        }

        private void LoadModuleTypes(IEnumerable<ModuleInfo> moduleInfos)
        {
            List<ModuleInfo> availableModules = new List<ModuleInfo>();
            List<ModuleInfo> loadedModules = new List<ModuleInfo>();

            if (moduleInfos == null)
            {
                return;
            }

            foreach (ModuleInfo moduleInfo in moduleInfos)
            {
                if (moduleInfo.State == ModuleState.NotStarted)
                {
                    moduleInfo.State = ModuleState.ReadyForInitialization;
                    availableModules.Add(moduleInfo);
                }
            }

            Action<ModuleCategory> initMudulesByCategoryAction = new Action<ModuleCategory>(category =>
            {
                bool keepLoading = true;
                while (keepLoading)
                {
                    keepLoading = false;
                    var tempList = availableModules.Where(m => m.State == ModuleState.ReadyForInitialization
                        && m.ModuleCategory == category);

                    foreach (ModuleInfo moduleInfo in tempList)
                    {
                        if (this.AreDependenciesLoaded(moduleInfo))
                        {
                            moduleInfo.State = ModuleState.Initializing;
                            this.InitializeModule(moduleInfo);
                            loadedModules.Add(moduleInfo);
                            keepLoading = true;
                        }
                    }
                }
            });

            initMudulesByCategoryAction(ModuleCategory.Core);
            initMudulesByCategoryAction(ModuleCategory.Init);
            initMudulesByCategoryAction(ModuleCategory.Common);

            for (var i = loadedModules.Count - 1; i > -1; i--)
                this.RunModule(loadedModules[i]);
        }

        private bool AreDependenciesLoaded(ModuleInfo moduleInfo)
        {
            IEnumerable<ModuleInfo> requiredModules = this.moduleCatalog.GetDependentModules(moduleInfo);
            if (requiredModules == null)
            {
                return true;
            }

            int notReadyRequiredModuleCount =
                requiredModules.Count(requiredModule =>
                    !(requiredModule.State == ModuleState.Initialized || requiredModule.State == ModuleState.Running));

            return notReadyRequiredModuleCount == 0;
        }

        private IModuleTypeLoader GetTypeLoaderForModule(ModuleInfo moduleInfo)
        {
            foreach (IModuleTypeLoader typeLoader in this.ModuleTypeLoaders)
            {
                if (typeLoader.CanLoadModuleType(moduleInfo))
                {
                    return typeLoader;
                }
            }

            throw new ModuleTypeLoaderNotFoundException(moduleInfo.ModuleName, String.Format(CultureInfo.CurrentCulture, Messages.NoRetrieverCanRetrieveModule, moduleInfo.ModuleName), null);
        }

        private void InitializeModule(ModuleInfo moduleInfo)
        {
            if (moduleInfo.State == ModuleState.Initializing)
            {
                moduleInfo.LoadOrder = ++_orderCount;

                loggerFacade.Log(String.Format("Initializing module {0}, Category: {1}, Priority: {2}, SortKey: {3}, Order: {4}",
                        moduleInfo.ModuleName,
                        moduleInfo.ModuleCategory.ToString(),
                        moduleInfo.ModulePriority.ToString(),
                        moduleInfo.LoadOrderIndex.ToString(),
                        moduleInfo.LoadOrder.ToString()),
                        Category.Debug, Priority.High);

                this.moduleInitializer.Initialize(moduleInfo);
                moduleInfo.State = ModuleState.Initialized;
            }
        }

        private void RunModule(ModuleInfo moduleInfo)
        {
            if (moduleInfo.State == ModuleState.Initialized)
            {
                loggerFacade.Log(String.Format("Running module {0}, Order: {1}",
                        moduleInfo.ModuleName,
                        moduleInfo.LoadOrder.ToString()),
                        Category.Debug, Priority.High);

                this.moduleInitializer.RunModule(moduleInfo);
                moduleInfo.State = ModuleState.Running;
            }
        }
    }
}
