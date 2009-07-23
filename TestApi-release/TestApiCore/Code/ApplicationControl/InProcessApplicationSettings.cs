using System;

namespace Microsoft.Test.ApplicationControl
{
    /// <summary>
    /// Configures an in-process test application.
    /// </summary>
    [Serializable]
    public class InProcessApplicationSettings : ApplicationSettings
    {
        /// <summary>
        /// The application path to test. 
        /// </summary>
        public string Path
        {
            get;
            set;
        }

        /// <summary>
        /// The type of test application to create.
        /// </summary>
        public InProcessApplicationType InProcessApplicationType
        {
            get;
            set;
        }        
    }
}
