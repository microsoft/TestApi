using System;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Test.ApplicationControl
{
    /// <summary>
    /// Factory for a WpfApplication implementation that AutomatedApplication
    /// will consume.
    /// </summary>
    public class WpfInProcessApplicationFactory : IAutomatedApplicationImplFactory
    {
        /// <summary>
        /// Factory method for creating the IAutomatedApplicationImpl instance 
        /// to be used by AutomatedApplication.
        /// </summary>
        /// <param name="settings">The settings needed to create the specific instance</param>
        /// <param name="appDomain">
        /// The AppDomain to create the implementation in.  This will be null for scenarios
        /// where separate AppDomain is not specified.
        /// </param>
        /// <returns>Returns the application implementation of Wpf for an InProcessApplication</returns>
        public IAutomatedApplicationImpl Create(ApplicationSettings settings, AppDomain appDomain)
        {
            IAutomatedApplicationImpl appImp = null;
            if (settings != null)
            {
                if (appDomain != null)
                {
                    appImp = (WpfApplicationImpl)appDomain.CreateInstanceAndUnwrap(
                        Assembly.GetExecutingAssembly().GetName().Name,
                        typeof(WpfApplicationImpl).FullName,
                        false,
                        BindingFlags.CreateInstance,
                        null,
                        new object[] { settings as WpfInProcessApplicationSettings },
                        CultureInfo.InvariantCulture,
                        null,
                        null);
                }
                else
                {
                    appImp = new WpfApplicationImpl(settings as WpfInProcessApplicationSettings);
                }
            }

            return appImp;
        }
    }
}