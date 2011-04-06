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
using System.Collections.ObjectModel;

namespace Wing.Modularity
{
    /// <summary>
    /// Defines the metadata that describes a module.
    /// </summary>
#if (!SILVERLIGHT)
    [Serializable]
#endif
    public partial class ModuleInfo : IModuleCatalogItem
    {
        private Object __lockObject = new Object();
        private static int _loadOrderSeqCounter = 0;

        private int _loadOrderSeq = 0;

        /// <summary>
        /// Initializes a new empty instance of <see cref="ModuleInfo"/>.
        /// </summary>
        public ModuleInfo()
            : this(null, null, new string[0]) { }

        /// <summary>
        /// Initializes a new instance of <see cref="ModuleInfo"/>.
        /// </summary>
        /// <param name="name">The module's name.</param>
        /// <param name="type">The module <see cref="Type"/>'s AssemblyQualifiedName.</param>
        /// <param name="dependsOn">The modules this instance depends on.</param>
        public ModuleInfo(string name, string type, params string[] dependsOn)
        {
            lock (__lockObject)
            {
                _loadOrderSeqCounter = _loadOrderSeqCounter + 1;
                _loadOrderSeq = _loadOrderSeqCounter;
            }
            this.ModuleCategory = Modularity.ModuleCategory.Common;
            this.ModulePriority = Modularity.ModulePriority.Normal;
            this.ModuleName = name;
            this.ModuleType = type;
            this.DependsOn = new Collection<string>();
            foreach (string dependency in dependsOn)
            {
                this.DependsOn.Add(dependency);
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ModuleInfo"/>.
        /// </summary>
        /// <param name="name">The module's name.</param>
        /// <param name="type">The module's type.</param>
        public ModuleInfo(string name, string type)
            : this(name, type, new string[0])
        {
        }

        /// <summary>
        /// Gets or sets the name of the module.
        /// </summary>
        /// <value>The name of the module.</value>
        public string ModuleName { get; set; }

        /// <summary>
        /// Gets or sets the module <see cref="Type"/>'s AssemblyQualifiedName.
        /// </summary>
        /// <value>The type of the module.</value>
        public string ModuleType { get; set; }

        /// <summary>
        /// Gets or sets the list of modules that this module depends upon.
        /// </summary>
        /// <value>The list of modules that this module depends upon.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "The setter is here to work around a Silverlight issue with setting properties from within Xaml.")]
        public Collection<string> DependsOn { get; set; }

        /// <summary>
        /// Reference to the location of the module assembly.
        /// <example>The following are examples of valid <see cref="ModuleInfo.Ref"/> values:
        /// http://myDomain/ClientBin/MyModules.xap for remote module in Silverlight
        /// file:///c:/MyProject/Modules/MyModule.dll for a loose DLL in WPF.
        /// </example>
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// Gets or sets the state of the <see cref="ModuleInfo"/> with regards to the module loading and initialization process.
        /// </summary>
        public ModuleState State { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ModuleCategory"/> for this module
        /// </summary>
        public ModuleCategory ModuleCategory { get; set; }

        /// <summary>
        /// Gets or sets a description for this module. 
        /// </summary>
        public string ModuleDescription { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ModulePriority"/> for this module
        /// </summary>
        public ModulePriority ModulePriority { get; set; }

        public int LoadOrderIndex { get { return ((int)ModuleCategory * 10000) + ((int)ModulePriority * 1000) + _loadOrderSeq; } }

        public int LoadOrder { get; set; }

        public IModule ModuleInstance { get; set; }
    }
}
