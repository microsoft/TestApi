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