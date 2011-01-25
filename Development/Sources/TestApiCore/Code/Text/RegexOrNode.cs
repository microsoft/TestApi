// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;

namespace Microsoft.Test.Text
{
    /// <summary>
    /// Represents a group of tokens one of which will be generated .
    /// For example abc[A-Z]|cde{2,3} is a reornode with 2 children.
    /// </summary>
    class RegexOrNode : RegexNode
    {
        public List<RegexNode> Children = new List<RegexNode>();
        private RegexNode mReservedPath = null; //The child node that this Or Node must choose
        //Chosen node is random if this is null

        public override void ReservePath(RegexNode child)
        {
            //this child (in Children) must be called when generating the string
            mReservedPath = child;
            base.ReservePath(child);
        }
        public override string Generate(Random random)
        {
            if (mReservedPath != null)
            {
                //call the reserved path
                return mReservedPath.Generate(random);
            }
            else
            {
                //call a random path
                return Children[random.Next(Children.Count)].Generate(random);
            }
        }
    }
}
