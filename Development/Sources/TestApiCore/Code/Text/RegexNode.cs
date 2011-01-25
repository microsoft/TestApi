// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.Text
{
    /// <summary>
    /// Base class for regex elements.
    /// </summary>
    abstract class RegexNode
    {
        public RegexNode Parent; //the parent node that will call Generate on this node

        //Generates a string matching the regex element or not matching if this is RegexCompiler.invalidNode
        //returns - A string which generated based on the regular expression of this node
        public abstract string Generate(Random random);

        //Required for GenerateInvalid.
        //Marks along the RegEx tree to ensure that 'child' is part of the generated string
        //child - The child node that must be part of the generated string
        public virtual void ReservePath(RegexNode child)
        {
            if (Parent != null)
            {
                Parent.ReservePath(this);
            }
        }

        //Assert in parsing
        //b - Value that must be true for assert to pass
        //message - Message to throw if the assert fails
        static public void AssertParse(bool b, string message)
        {
            if (!b)
                throw new ArgumentException("Regex parse error: " + message);
        }
    }
}