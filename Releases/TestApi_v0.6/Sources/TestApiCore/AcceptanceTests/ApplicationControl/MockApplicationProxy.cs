// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Runtime.Serialization;
using Microsoft.Test.ApplicationControl;

namespace Microsoft.Test.AcceptanceTests.ApplicationControl
{
    /// <summary>
    /// Mock implementation of IAutomatedApplicationImpl for unit testing
    /// </summary>
    public class MockApplicationProxy : IAutomatedApplicationImpl
    {
        public MockApplicationProxy()
        {
            this.MainWindowOpened += (s, e) => { throw new MockApplicationException(); };
            this.FocusChanged += (s, e) => { throw new MockApplicationException(); };
            this.Exited += (s, e) => { throw new MockApplicationException(); };
        }

        #region IAutomatedApplicationImpl Members
         
#pragma warning disable 0067
        public event EventHandler Exited;
        public event EventHandler FocusChanged;
        public event EventHandler MainWindowOpened;
#pragma warning restore 0067

        public object ApplicationDriver
        {
            get { throw new MockApplicationException(); }
        }

        public void Close()
        {
            throw new MockApplicationException();
        }              

        public bool IsMainWindowOpened
        {
            get { throw new MockApplicationException(); }
        }

        public object MainWindow
        {
            get { throw new MockApplicationException(); }
        }

        public void Start()
        {
            // does nothing
        }

        public void WaitForInputIdle(TimeSpan timeSpan)
        {
            throw new MockApplicationException();
        }

        public void WaitForInputIdle()
        {
            throw new MockApplicationException();
        }

        public void WaitForMainWindow(TimeSpan timeout)
        {
            throw new MockApplicationException();
        }

        public void WaitForWindow(string windowName, TimeSpan timeout)
        {
            throw new MockApplicationException();
        }

        #endregion
    }

    /// <summary>
    /// Mock factory for unit testing
    /// </summary>
    public class MockApplicationFactory : IAutomatedApplicationImplFactory
    {
        /// <summary>
        /// Just returns a new MockApplicationProxy
        /// </summary>
        public IAutomatedApplicationImpl Create(ApplicationSettings settings, AppDomain appDomain)
        {
            return new MockApplicationProxy();           
        }
    }

    /// <summary>
    /// Used to identify that a method in the MockApplicationProxy has been called
    /// </summary>
    [Serializable]
    public class MockApplicationException : Exception
    {
        /// <summary>
        /// Passes message parameter to base constructor.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        public MockApplicationException()
            : base("Test MockApplicationException message")
        {
        }

        /// <summary>
        /// Passes message parameter to base constructor.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        public MockApplicationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Passes parameters to base constructor.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception. </param>
        public MockApplicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// This constructor is required for deserializing this type across AppDomains.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected MockApplicationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
