// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Test.ApplicationControl
{
    /// <summary>
    /// Represents the event args passed to AutomatedApplication focus changed events.
    /// </summary>
    public class AutomatedApplicationFocusChangedEventArgs : AutomatedApplicationEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the AutomatedApplicationFocusChangedEventArgs
        /// class.
        /// </summary>
        /// <param name="automatedApp">
        /// The AutomatedApplication data to pass to the listeners.
        /// </param>
        /// <param name="newFocusedElement">
        /// The new focused element data to pass the listeners. This can be an AutomationElement 
        /// for an out-of-process scenario or a UIElement for an in-process WPF scenario.
        /// </param>
        public AutomatedApplicationFocusChangedEventArgs(AutomatedApplication automatedApp, object newFocusedElement)
            : base(automatedApp)
        {
            NewFocusedElement = newFocusedElement;
        }

        /// <summary>
        /// The new focused element passed to the listeners.
        /// </summary>
        public object NewFocusedElement
        {
            get;
            set;
        }
    }
}
