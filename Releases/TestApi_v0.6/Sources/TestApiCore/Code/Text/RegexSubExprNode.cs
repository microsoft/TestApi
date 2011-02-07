// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.Text
{
    /// <summary>
    /// This node represents a subexpression i.e. anything in parentheses.
    /// For example (abc) is a subexpression with one node in it.
    /// </summary>
    class RegexSubExpressionNode : RegexNode
    {
        RegexNode mRefNode;
        public string Name; //Identifies subexpression by name, used for named backreferences

        public RegexSubExpressionNode(RegexNode subExpr)
        {
            mRefNode = subExpr;
            mRefNode.Parent = this;
        }

        public override string Generate(Random random)
        {
            return mRefNode.Generate(random);
        }
    }
}