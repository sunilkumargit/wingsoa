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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Security.Policy;
using Wing.Logging;

namespace Wing.Modularity
{
    /// <summary>
    /// Represets a catalog created from a directory on disk.
    /// </summary>
    /// <remarks>
    /// The directory catalog will scan the contents of a directory, locating classes that implement
    /// <see cref="IModule"/> and add them to the catalog based on contents in their associated <see cref="ModuleAttribute"/>.
    /// Assemblies are loaded into a new application domain with ReflectionOnlyLoad.  The application domain is destroyed
    /// once the assemblies have been discovered.
    /// 
    /// The diretory catalog does not continue to monitor the directory after it has created the initialze catalog.
    /// </remarks>
    [SecurityPermission(SecurityAction.LinkDemand)]
    [SecurityPermission(SecurityAction.InheritanceDemand)]
    public class DirectoryModuleCatalog2 : ModuleCatalog
    {
        public DirectoryModuleCatalog2(String path)
            : this()
        {
            ModulePath = path;
        }

        public DirectoryModuleCatalog2()
        {
            _logger = ServiceLocator.GetInstance<ILogManager>().GetSystemLogger();
        }

        /// <summary>
        /// Directory containing modules to search for.
        /// </summary>
        public string ModulePath { get; set; }

        /// <summary>
        /// Drives the main logic of building the child domain and searching for the assemblies.
        /// </summary>
        protected override void InnerLoad()
        {
            _logger.Log("Searching for modules on {0}".Templ(ModulePath), Category.Info, Priority.None);
            DirectoryInfo directory = new DirectoryInfo(ModulePath);

            if (string.IsNullOrEmpty(this.ModulePath))
                throw new InvalidOperationException(Messages.ModulePathCannotBeNullOrEmpty);

            if (!Directory.Exists(this.ModulePath))
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, Messages.DirectoryNotFound, this.ModulePath));

            //enlist loaded assemblies
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(asm => !(asm is System.Reflection.Emit.AssemblyBuilder))
                                .Where(asm => !asm.IsDynamic)
                                .Where(asm => !String.IsNullOrWhiteSpace(asm.Location))
                                .ToList();

            //enlist assemblies on modules path
            var assembliesToLoad = directory
                .GetFiles("*.dll")
                .Where(file => !loadedAssemblies.Any(assembly => Path.GetFileName(assembly.Location).EqualsIgnoreCase(file.Name)))
                .ToList();

            //load assemblies
            foreach (var asmFileInfo in assembliesToLoad)
            {
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.LoadFrom(asmFileInfo.FullName);
                    loadedAssemblies.Add(assembly);
                }
                catch (Exception ex)
                {
                    _logger.LogException("Error on load assembly {0}".Templ(asmFileInfo.FullName), ex, Priority.Low);
                    continue;
                }
            }

            _logger.Log("{0} assemblies founded.".Templ(loadedAssemblies.Count), Category.Debug, Priority.None);


            //enlist module types
            foreach (var asm in loadedAssemblies)
            {
                try
                {
                    _logger.Log("Attempt to load types from assembly {0}".Templ(asm.FullName), Category.Debug, Priority.Low);
                    var modulesInfo = asm.GetExportedTypes()
                        .Where(t => typeof(IModule).IsAssignableFrom(t))
                        .Where(t => t != typeof(IModule))
                        .Where(t => !t.IsAbstract)
                        .Where(t => !t.IsInterface)
                        .Select(type => CreateModuleInfo(type))
                        .ToList();
                    if (modulesInfo.Count > 0)
                    {
                        foreach (var module in modulesInfo)
                        {
                            _logger.Log("Module {0} discovered in assembly {1}".Templ(module.ModuleName, asm.FullName), Category.Debug, Priority.Low);
                            this.Items.Add(module);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogException("Attempt of loading types from assemly {0} failed.".Templ(asm.FullName), ex, Priority.Medium);
                    continue;
                }
            }

            _logger.Log("Module discovering finished: {0} modules found.".Templ(this.Items.Count), Category.Info, Priority.Low);
        }

        private static IModuleInfoBuilder _moduleInfoBuilder;
        private ILogger _logger;
        private static ModuleInfo CreateModuleInfo(Type type)
        {
            _moduleInfoBuilder = _moduleInfoBuilder ?? new ModuleInfoBuilder();
            return _moduleInfoBuilder.BuildModuleInfo(type);
        }
    }
}