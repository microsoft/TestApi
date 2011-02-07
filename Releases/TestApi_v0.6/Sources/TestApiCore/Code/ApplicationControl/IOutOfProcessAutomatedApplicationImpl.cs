// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;

namespace Microsoft.Test.ApplicationControl
{
    /// <summary>
    /// Defines the contract for an out of process AutomatedApplication.
    /// </summary>
    /// <remarks>
    /// Represents the 'Implemention' inteface for the AutomatedApplication bridge. As such, 
    /// this can vary from the public interface of AutomatedApplication.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Impl")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public interface IOutOfProcessAutomatedApplicationImpl : IAutomatedApplicationImpl
    {        
        /// <summary>
        /// The process associated with the application.
        /// </summary>
        Process Process { get; }      
    }
}
