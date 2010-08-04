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

namespace Wing.Modularity
{
    /// <summary>
    /// Defines the contract for the modules deployed in the application.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Notifies the module to be initialized.
        /// </summary>
        void Initialize();


        /// <summary>
        /// Notifies the module that has been initialized. This method is invoked in reverse order of Initialze.
        /// The last module initialized will be the first module that Initialized() will be called.
        /// </summary>
        void Initialized();


        /// <summary>
        /// Last phase of module initialization process. Is called after Initialized() in normal order.
        /// </summary>
        void Run();
    }
}