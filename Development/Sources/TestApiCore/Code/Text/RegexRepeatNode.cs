// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Text;

namespace Microsoft.Test.Text
{
    /// <summary>
    /// Represents repetition token ( i.e. 'cde{2,3}' in abc[A-Z]|cde{2,3} ).
    /// </summary>
    class RegexRepeatNode : RegexNode
    {
        private int mMinRepeat;
        private int mMaxRepeat;
        private bool mSameValue; //Repeat same character?
        private RegexNode mRefNode; //The node to repeat
        public static int extraRepetitions = 10; //The additional number of times to repeat (approximates infinity)
        private RegexNode mReservedPath; //The child node that must be chosen.
        //If this is not null then the node must repeat at least once

        public RegexRepeatNode(RegexNode refNode, int minRepeat, int maxRepeat, bool sameValue)
        {
            //if this does not cover zero to infinity, then this node can be invalidated
            if (RegexCompiler.IsInvalidSection && (minRepeat > 0 || maxRepeat != -1))
            {
                RegexCompiler.InvalidableNodes.Add(this);
            }
            mMinRepeat = minRepeat;
            mMaxRepeat = maxRepeat;
            mSameValue = sameValue;
            mReservedPath = null;
            mRefNode = refNode;
            mRefNode.Parent = this;
        }

        public override void ReservePath(RegexNode child)
        {
            //this child (mRefNode) must be called when generating the string (cannot repeat zero times)
            mReservedPath = child;
            base.ReservePath(child);
        }
        public override string Generate(Random random)
        {
            int numRepeat;
            StringBuilder buffer = new StringBuilder();
            if (this == RegexCompiler.InvalidNode)
            {
                //randomly choose to repeat more or less than the given range
                int repeatMore = random.Next(2);
                if ((mMaxRepeat != -1 && 1 == repeatMore) || mMinRepeat == 0)
                {
                    //repeat more than the given range
                    checked
                    {
                        numRepeat = random.Next(mMaxRepeat + 1, mMaxRepeat + 11);
                    }
                }
                else
                {
                    //repeat less than the given range
                    numRepeat = random.Next(0, mMinRepeat);
                }
            }
            else
            {
                //repeat for some number inside the given range
                checked
                {
                    int maxRepeat = (mMaxRepeat == -1) ? mMinRepeat + extraRepetitions : mMaxRepeat;

                    //don't repeat zero times if the repeated node is on the invalidating path
                    int minRepeat = (mMinRepeat == 0 && mRefNode == mReservedPath) ? 1 : mMinRepeat;

                    numRepeat = (minRepeat < maxRepeat) ? random.Next(minRepeat, maxRepeat + 1) : minRepeat;
                }
            }
            string childStr;

            if (mRefNode is RegexTextNode) //If the referenced node is text node, only repeat the last character
            {
                childStr = mRefNode.Generate(random);
                buffer.Append(childStr.Substring(0, childStr.Length - 1));
                childStr = childStr[childStr.Length - 1].ToString(); //Get last character
                mSameValue = true;
            }
            else
            {
                childStr = mRefNode.Generate(random);
            }

            for (int i = 0; i < numRepeat; i++)
                buffer.Append(mSameValue ? childStr : mRefNode.Generate(random));

            return buffer.ToString();
        }
    }
}