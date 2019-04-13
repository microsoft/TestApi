// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.ApplicationControl
{
    /// <summary>
    /// Provides configuration information for an <see cref="AutomatedApplication"/>.
    /// </summary>
    [Serializable]
    public class ApplicationSettings
    {
        /// <summary>
        /// The interface used for creation of the AutomatedApplicationImplementation.
        /// </summary>
        public IAutomatedApplicationImplFactory ApplicationImplementationFactory
        {
            get;
            set;
        }
    }
}