// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.ApplicationControl
{
    /// <summary>
    /// Factory for a UIAutomation implementation that AutomatedApplication
    /// will consume.
    /// </summary>
    public class UIAutomationOutOfProcessApplicationFactory : IAutomatedApplicationImplFactory
    {
        /// <summary>
        /// Factory method for creating the IAutomatedApplicationImpl instance 
        /// to be used by AutomatedApplication.
        /// </summary>
        /// <param name="settings">The settings needed to create the specific instance</param>
        /// <param name="appDomain">The UIAutomation app proxy does not require initialization on a separate appdomain</param>
        /// <returns>Returns the application implementation of UIAutomation for an OutOfProcessApplication</returns>
        public IAutomatedApplicationImpl Create(ApplicationSettings settings, AppDomain appDomain)
        {
            IAutomatedApplicationImpl appImp = null;
            if (settings != null)
            {
                appImp = new UIAutomationApplicationImpl(settings as OutOfProcessApplicationSettings);                                
            }

            return appImp;
        }
    }
}