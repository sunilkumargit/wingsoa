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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Wing.Utils;


namespace Wing.Modularity
{
    /// <summary>
    /// The <see cref="ModuleCatalog"/> holds information about the modules that can be used by the 
    /// application. Each module is described in a <see cref="ModuleInfo"/> class, that records the 
    /// name, type and location of the module. 
    /// 
    /// It also verifies that the <see cref="ModuleCatalog"/> is internally valid. That means that
    /// it does not have:
    /// <list>
    ///     <item>Circular dependencies</item>
    ///     <item>Missing dependencies</item>
    ///     <item>
    ///         Invalid dependencies, such as a Module that's loaded at startup that depends on a module 
    ///         that might need to be retrieved.
    ///     </item>
    /// </list>
    /// The <see cref="ModuleCatalog"/> also serves as a baseclass for more specialized Catalogs .
    /// </summary>
    public class ModuleCatalog : IModuleCatalog
    {
        private readonly ModuleCatalogItemCollection items;
        private bool isLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleCatalog"/> class.
        /// </summary>
        public ModuleCatalog()
        {
            this.items = new ModuleCatalogItemCollection();
            this.items.CollectionChanged += this.ItemsCollectionChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleCatalog"/> class while providing an 
        /// initial list of <see cref="ModuleInfo"/>s.
        /// </summary>
        /// <param name="modules">The initial list of modules.</param>
        public ModuleCatalog(IEnumerable<ModuleInfo> modules)
            : this()
        {
            foreach (ModuleInfo moduleInfo in modules)
            {
                this.Items.Add(moduleInfo);
            }
        }

        /// <summary>
        /// Gets the items in the <see cref="ModuleCatalog"/>. This property is mainly used to add <see cref="ModuleInfoGroup"/>s or 
        /// <see cref="ModuleInfo"/>s through XAML. 
        /// </summary>
        /// <value>The items in the catalog.</value>
        public Collection<IModuleCatalogItem> Items
        {
            get { return this.items; }
        }

        /// <summary>
        /// Gets or sets a value that remembers whether the <see cref="ModuleCatalog"/> has been validated already. 
        /// </summary>
        protected bool Validated { get; set; }

        /// <summary>
        /// Returns the list of <see cref="ModuleInfo"/>s that are not contained within any <see cref="ModuleInfoGroup"/>. 
        /// </summary>
        /// <value>The groupless modules.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Groupless")]
        public IEnumerable<ModuleInfo> Modules
        {
            get
            {
                return this.Items.OfType<ModuleInfo>();
            }
        }


        /// <summary>
        /// Loads the catalog if necessary.
        /// </summary>
        public void Load()
        {
            this.isLoaded = true;
            this.InnerLoad();
        }

        /// <summary>
        /// Return the list of <see cref="ModuleInfo"/>s that <paramref name="moduleInfo"/> depends on.
        /// </summary>
        /// <remarks>
        /// If  the <see cref="ModuleCatalog"/> was not yet validated, this method will call <see cref="Validate"/>.
        /// </remarks>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to get the </param>
        /// <returns>An enumeration of <see cref="ModuleInfo"/> that <paramref name="moduleInfo"/> depends on.</returns>
        public virtual IEnumerable<ModuleInfo> GetDependentModules(ModuleInfo moduleInfo)
        {
            this.EnsureCatalogValidated();

            return this.GetDependentModulesInner(moduleInfo);
        }

        /// <summary>
        /// Adds a <see cref="ModuleInfo"/> to the <see cref="ModuleCatalog"/>.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to add.</param>
        /// <returns>The <see cref="ModuleCatalog"/> for easily adding multiple modules.</returns>
        public virtual IModuleCatalog AddModule(ModuleInfo moduleInfo)
        {
            this.Items.Add(moduleInfo);
            return this;
        }

        /// <summary>
        /// Returns a list of <see cref="ModuleInfo"/>s that contain both the <see cref="ModuleInfo"/>s in 
        /// <paramref name="modules"/>, but also all the modules they depend on. 
        /// </summary>
        /// <param name="modules">The modules to get the dependencies for.</param>
        /// <returns>
        /// A list of <see cref="ModuleInfo"/> that contains both all <see cref="ModuleInfo"/>s in <paramref name="modules"/>
        /// but also all the <see cref="ModuleInfo"/> they depend on.
        /// </returns>
        public virtual IEnumerable<ModuleInfo> CompleteListWithDependencies(IEnumerable<ModuleInfo> modules)
        {
            if (modules == null)
            {
                throw new ArgumentNullException("modules");
            }

            this.EnsureCatalogValidated();

            List<ModuleInfo> completeList = new List<ModuleInfo>();
            List<ModuleInfo> pendingList = modules.OrderBy(k => k, new ComparerDelegate<ModuleInfo>((m1, m2) => m1.LoadOrderIndex - m2.LoadOrderIndex)).ToList();

            while (pendingList.Count > 0)
            {
                ModuleInfo moduleInfo = pendingList[0];

                foreach (ModuleInfo dependency in this.GetDependentModules(moduleInfo))
                {
                    if (!completeList.Contains(dependency) && !pendingList.Contains(dependency))
                    {
                        pendingList.Add(dependency);
                    }
                }

                pendingList.RemoveAt(0);
                completeList.Add(moduleInfo);
            }

            return completeList;
        }

        /// <summary>
        /// Validates the <see cref="ModuleCatalog"/>.
        /// </summary>
        /// <exception cref="ModularityException">When validation of the <see cref="ModuleCatalog"/> fails.</exception>
        public virtual void Validate()
        {
            this.ValidateUniqueModules();
            this.ValidateDependencyGraph();
            this.ValidateCategoryDependencies();
            this.Validated = true;
        }

        /// <summary>
        /// Adds a groupless <see cref="ModuleInfo"/> to the catalog.
        /// </summary>
        /// <param name="moduleType"><see cref="Type"/> of the module to be added.</param>
        /// <param name="dependsOn">Collection of module names (<see cref="ModuleInfo.ModuleName"/>) of the modules on which the module to be added logically depends on.</param>
        /// <returns>The same <see cref="ModuleCatalog"/> instance with the added module.</returns>
        public ModuleCatalog AddModule(Type moduleType, params string[] dependsOn)
        {
            return this.AddModule(moduleType, dependsOn);
        }

        /// <summary>
        /// Adds a groupless <see cref="ModuleInfo"/> to the catalog.
        /// </summary>
        /// <param name="moduleName">Name of the module to be added.</param>
        /// <param name="moduleType"><see cref="Type"/> of the module to be added.</param>
        /// <param name="initializationMode">Stage on which the module to be added will be initialized.</param>
        /// <param name="dependsOn">Collection of module names (<see cref="ModuleInfo.ModuleName"/>) of the modules on which the module to be added logically depends on.</param>
        /// <returns>The same <see cref="ModuleCatalog"/> instance with the added module.</returns>
        public ModuleCatalog AddModule(string moduleName, string moduleType, params string[] dependsOn)
        {
            return this.AddModule(moduleName, moduleType, null, dependsOn);
        }

        /// <summary>
        /// Adds a groupless <see cref="ModuleInfo"/> to the catalog.
        /// </summary>
        /// <param name="moduleName">Name of the module to be added.</param>
        /// <param name="moduleType"><see cref="Type"/> of the module to be added.</param>
        /// <param name="refValue">Reference to the location of the module to be added assembly.</param>
        /// <param name="initializationMode">Stage on which the module to be added will be initialized.</param>
        /// <param name="dependsOn">Collection of module names (<see cref="ModuleInfo.ModuleName"/>) of the modules on which the module to be added logically depends on.</param>
        /// <returns>The same <see cref="ModuleCatalog"/> instance with the added module.</returns>
        public ModuleCatalog AddModule(string moduleName, string moduleType, string refValue, params string[] dependsOn)
        {
            if (moduleName == null)
            {
                throw new ArgumentNullException("moduleName");
            }

            if (moduleType == null)
            {
                throw new ArgumentNullException("moduleType");
            }

            ModuleInfo moduleInfo = new ModuleInfo(moduleName, moduleType);
            moduleInfo.DependsOn.AddRange(dependsOn);
            moduleInfo.Ref = refValue;
            this.Items.Add(moduleInfo);
            return this;
        }

        /// <summary>
        /// Initializes the catalog, which may load and validate the modules.
        /// </summary>
        /// <exception cref="ModularityException">When validation of the <see cref="ModuleCatalog"/> fails, because this method calls <see cref="Validate"/>.</exception>
        public virtual void Initialize()
        {
            if (!this.isLoaded)
            {
                this.Load();
            }

            this.Validate();
        }

        /// <summary>
        /// Checks for cyclic dependencies, by calling the dependencysolver. 
        /// </summary>
        /// <param name="modules">the.</param>
        /// <returns></returns>
        protected static string[] SolveDependencies(IEnumerable<ModuleInfo> modules)
        {
            ModuleDependencySolver solver = new ModuleDependencySolver();

            foreach (ModuleInfo data in modules)
            {
                solver.AddModule(data.ModuleName);

                if (data.DependsOn != null)
                {
                    foreach (string dependency in data.DependsOn)
                    {
                        solver.AddDependency(data.ModuleName, dependency);
                    }
                }
            }

            if (solver.ModuleCount > 0)
            {
                return solver.Solve();
            }

            return new string[0];
        }

        /// <summary>
        /// Ensures that all the dependencies within <paramref name="modules"/> refer to <see cref="ModuleInfo"/>s
        /// within that list.
        /// </summary>
        /// <param name="modules">The modules to validate modules for.</param>
        /// <exception cref="ModularityException">
        /// Throws if a <see cref="ModuleInfo"/> in <paramref name="modules"/> depends on a module that's 
        /// not in <paramref name="modules"/>.
        /// </exception>
        protected static void ValidateDependencies(IEnumerable<ModuleInfo> modules)
        {
            var moduleNames = modules.Select(m => m.ModuleName).ToList();
            foreach (ModuleInfo moduleInfo in modules)
            {
                if (moduleInfo.DependsOn != null && moduleInfo.DependsOn.Except(moduleNames).Any())
                {
                    throw new ModularityException(
                        moduleInfo.ModuleName,
                        String.Format(CultureInfo.CurrentCulture, Messages.ModuleDependenciesNotMetInGroup, moduleInfo.ModuleName));
                }
            }
        }

        /// <summary>
        /// Does the actual work of loading the catalog.  The base implementation does nothing.
        /// </summary>
        protected virtual void InnerLoad()
        {
        }

        /// <summary>
        /// Makes sure all modules have an Unique name. 
        /// </summary>
        /// <exception cref="DuplicateModuleException">
        /// Thrown if the names of one or more modules are not unique. 
        /// </exception>
        protected virtual void ValidateUniqueModules()
        {
            List<string> moduleNames = this.Modules.Select(m => m.ModuleName).ToList();

            string duplicateModule = moduleNames.FirstOrDefault(
                m => moduleNames.Count(m2 => m2 == m) > 1);

            if (duplicateModule != null)
            {
                throw new DuplicateModuleException(duplicateModule, String.Format(CultureInfo.CurrentCulture, Messages.DuplicatedModule, duplicateModule));
            }
        }

        /// <summary>
        /// Ensures that there are no cyclic dependencies. 
        /// </summary>
        protected virtual void ValidateDependencyGraph()
        {
            SolveDependencies(this.Modules);
        }

        protected virtual void ValidateCategoryDependencies()
        {
            foreach (ModuleInfo data in this.Modules)
            {
                var dependents = this.GetDependentModulesInner(data);
                //if a dependent module category is greater than the module category, then the dependency
                //is not allowed.
                foreach (var dependent in dependents)
                {
                    if (((int)dependent.ModuleCategory) > ((int)data.ModuleCategory))
                    {
                        throw new InvalidCategoryDependencyException(data.ModuleName,
                            String.Format(CultureInfo.CurrentCulture, Messages.InvalidCategoryDependency, data.ModuleName, dependent.ModuleName, dependent.ModuleName, dependent.ModuleCategory.ToString()));
                    }
                }
            }
        }

        /// <summary>
        /// Returns the <see cref="ModuleInfo"/> on which the received module dependens on.
        /// </summary>
        /// <param name="moduleInfo">Module whose dependant modules are requested.</param>
        /// <returns>Collection of <see cref="ModuleInfo"/> dependants of <paramref name="moduleInfo"/>.</returns>
        protected virtual IEnumerable<ModuleInfo> GetDependentModulesInner(ModuleInfo moduleInfo)
        {
            return this.Modules.Where(dependantModule => moduleInfo.DependsOn.Contains(dependantModule.ModuleName));
        }

        /// <summary>
        /// Ensures that the catalog is validated.
        /// </summary>
        protected virtual void EnsureCatalogValidated()
        {
            if (!this.Validated)
            {
                this.Validate();
            }
        }

        private void ItemsCollectionChanged(object sender, EventArgs e)
        {
            if (this.Validated)
            {
                this.EnsureCatalogValidated();
            }
        }

        private class ModuleCatalogItemCollection : Collection<IModuleCatalogItem>
        {
            public event EventHandler CollectionChanged;

            protected override void InsertItem(int index, IModuleCatalogItem item)
            {
                base.InsertItem(index, item);

                this.OnNotifyCollectionChanged(new EventArgs() { });
            }

            protected void OnNotifyCollectionChanged(EventArgs eventArgs)
            {
                if (this.CollectionChanged != null)
                {
                    this.CollectionChanged(this, eventArgs);
                }
            }
        }
    }
}
