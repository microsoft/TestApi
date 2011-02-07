// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Text;

namespace Microsoft.Test.Text
{
    /// <summary>
    /// Represents a text portion of a regex (i.e. 'abc' and 'cde' in regular expression abc[A-Z]|cde{2,3}.
    /// </summary>
    class RegexTextNode : RegexNode
    {
        private StringBuilder mNodeText;

        public RegexTextNode(string str)
        {
            if ((RegexCompiler.IsInvalidSection) && (!String.IsNullOrEmpty(str)))
            {
                RegexCompiler.InvalidableNodes.Add(this);
            }
            mNodeText = new StringBuilder(str);
        }

        public override string Generate(Random random)
        {
            if (this == RegexCompiler.InvalidNode)
            {
                //select a character
                int pos = random.Next(mNodeText.Length);

                //generate any other character using a negative SetNode
                RegexSetNode others = new RegexSetNode(false);
                others.AddChars(mNodeText[pos].ToString());

                //replace the character
                char backup = mNodeText[pos];
                mNodeText[pos] = others.Generate(random)[0];
                string result = mNodeText.ToString();

                //if this node is repeated it needs to be cleaned up for the next call
                mNodeText[pos] = backup;

                return result;
            }
            else
            {
                return mNodeText.ToString();
            }
        }
    }
}
