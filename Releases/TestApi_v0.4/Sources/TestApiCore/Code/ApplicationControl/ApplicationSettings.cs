using System;

namespace Microsoft.Test.ApplicationControl
{
    /// <summary>
    /// Configuration info for an AutomatedApplication
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