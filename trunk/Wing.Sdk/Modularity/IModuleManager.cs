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
namespace Wing.Modularity
{
    /// <summary>
    /// Defines the interface for the service that will retrieve and initialize the application's modules.
    /// </summary>
    public interface IModuleManager
    {
        /// <summary>
        /// Initializes the modules marked as <see cref="InitializationMode.WhenAvailable"/> on the <see cref="ModuleCatalog"/>.
        /// </summary>
        void Run(String groupName);

        /// <summary>
        /// Initializes the module on the <see cref="ModuleCatalog"/> with the name <paramref name="moduleName"/>.
        /// </summary>
        /// <param name="moduleName">Name of the module requested for initialization.</param>
        void LoadModule(string moduleName);

        event EventHandler<ModuleManagerEventArgs> BeginLoadModules;
        event EventHandler<ModuleManagerEventArgs> EndLoadModules;
        event EventHandler<ModuleManagerEventArgs> ModuleInitialized;
        event EventHandler<ModuleManagerEventArgs> ModuleRunning;
    }

    public class ModuleManagerEventArgs : EventArgs
    {
        public ModuleInfo[] Modules { get; set; }
        public ModuleInfo CurrentModule { get; set; }
    }
}