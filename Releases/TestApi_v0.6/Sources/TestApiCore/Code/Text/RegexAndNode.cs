// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.Text
{
    /// <summary>
    /// Represents a group of tokens, each of which will be generated.
    /// For example abc[A-Z] is represented by a reandnode which contains two children nodes
    /// </summary>
    class RegexAndNode : RegexNode
    {
        public List<RegexNode> Children = new List<RegexNode>();

        public override string Generate(Random random)
        {
            StringBuilder buffer = new StringBuilder();

            foreach (RegexNode node in Children)
            {
                buffer.Append(node.Generate(random));
            }
            return buffer.ToString();
        }
    }
}
