// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.Text
{
    /// <summary>
    /// Inteface for specific string property class
    /// </summary>
    internal interface IStringProperty
    {
        /// <summary>
        /// Get next random code point or points that belongs to a specific
        /// string property. number of code points does not necessarily translate
        /// to number of chars since surrogate pair are two bytes
        /// </summary>
        string GetRandomCodePoints(int numOfProperty, int seed);

        /// <summary>
        /// Check if code point is in the property range
        /// </summary>
        bool IsInPropertyRange(int codePoint);
    }
}

