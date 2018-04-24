// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;

namespace Microsoft.Test.CommandLineParsing
{
    /// <summary>
    /// Defines whether a property value should contain all remaining properties which do not map to a named property in the object.
    /// Only valid on types of Dictionary&lt;string, string&gt;.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class AdditionalPropertiesAttribute : Attribute
    {
        /// <summary />
        public AdditionalPropertiesAttribute()
        {
        }
    }
}