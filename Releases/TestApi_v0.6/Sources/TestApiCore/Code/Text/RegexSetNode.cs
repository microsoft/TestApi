// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.Text
{
    /// <summary>
    /// Represents a set of characters inside [ ]
    /// For example [a-z].
    /// </summary>
    class RegexSetNode : RegexNode
    {
        private int mMapSize = 128;
        private byte[] mMap = new byte[128]; //Indicates which characters are present in the set
        private bool mPositiveSet;           //If false, the characters added by the user are excluded
        private int mNumChoices;             //Reflects number of possible characters that can be chosen in the set

        public RegexSetNode(bool positiveSet)
        {
            if (RegexCompiler.IsInvalidSection)
            {
                RegexCompiler.InvalidableNodes.Add(this);
            }

            mPositiveSet = positiveSet;
            mNumChoices = mPositiveSet ? 0 : mMapSize;  //In a negative set all characters can be chosen
        }

        //Expands the set range to cover unicode characters
        private void ExpandToUnicodeRange()
        {
            byte[] mNewMap = new byte[char.MaxValue + 1];
            Array.Copy(mMap, 0, mNewMap, 0, 128);

            if (!mPositiveSet)
                mNumChoices += char.MaxValue + 1 - 128;

            mMapSize = char.MaxValue + 1;
            mMap = mNewMap;
        }

        public void AddChars(string chars)
        {
            //mark the added characters and update the number of available choices
            foreach (char c in chars.ToCharArray())
            {
                if (c > mMapSize - 1)
                    ExpandToUnicodeRange();

                if (mMap[c] == 0)
                {
                    mMap[c] = 1;
                    mNumChoices += mPositiveSet ? 1 : -1;
                }
            }

            //check if this set still has invalid characters available
            if ((mPositiveSet && mNumChoices == mMapSize) || (!mPositiveSet && mNumChoices == 0))
            {
                //can never be invalid
                RegexCompiler.InvalidableNodes.Remove(this);
            }
        }

        //Add the chars in alphabet from start to end to the set
        public void AddRange(char start, char end)
        {
            RegexNode.AssertParse((start < end) && end <= char.MaxValue, "Invalid range specified in char set");

            if (end > mMapSize)
                ExpandToUnicodeRange();

            //mark the added characters and update the number of available choices
            for (long c = start; c <= end; c++)
            {
                if (mMap[c] == 0)
                {
                    mMap[c] = 1;
                    mNumChoices += mPositiveSet ? 1 : -1;
                }
            }

            //check if this set still has invalid characters available
            if ((mPositiveSet && mNumChoices == mMapSize) || (!mPositiveSet && mNumChoices == 0))
            {
                //can never be invalid
                RegexCompiler.InvalidableNodes.Remove(this);
            }
        }

        public override string Generate(Random random)
        {
            if (this == RegexCompiler.InvalidNode)
            {
                RegexNode.AssertParse(mNumChoices > 0, "No valid range specified in char set");

                //select from the elements that are not available (elements that are invalid)
                int randIndex = random.Next(mMapSize - mNumChoices);

                int i = -1;
                while (randIndex >= 0)  //seek to the available element 
                {
                    i++;
                    //invert positive and negative sets
                    if ((mPositiveSet && mMap[i] == 0) || (!mPositiveSet && mMap[i] == 1))
                        randIndex--;

                }

                return Convert.ToChar(i).ToString();
            }
            else
            {
                RegexNode.AssertParse(mNumChoices > 0, "No valid range specified in char set");
                //select from the elements that are available
                int randIndex = random.Next(mNumChoices);

                int i = -1;
                while (randIndex >= 0)  //seek to the available element 
                {
                    i++;
                    if ((mPositiveSet && mMap[i] == 1) || (!mPositiveSet && mMap[i] == 0))
                        randIndex--;

                }

                return Convert.ToChar(i).ToString();
            }
        }
    }
}
