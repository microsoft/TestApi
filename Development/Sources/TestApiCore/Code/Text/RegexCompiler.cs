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
    /// Transforms given regex into set of connected nodes.
    /// </summary>
    class RegexCompiler
    {
        //InvalidableNodes is used to determine which nodes can generate non-matching strings
        //Since RegexSetNode fills character spaces after creation, it may have to remove itself from this list later
        //TODO: Could be implemented as an int maxID if the number of invalidable nodes does not decrease
        //Where all invalidable nodes would have an ID from 1 to maxID
        public static bool IsInvalidSection = false;
        public static List<RegexNode> InvalidableNodes = new List<RegexNode>(); //list of nodes that can be invalid
        public static RegexNode InvalidNode = null; //node selected to generate invalid text
        StringBuilder mRegex;   //Regex that is being processed
        List<RegexNode> mBackRefs = new List<RegexNode>();       //Holds indexed backreferences
        List<RegexNode> mNamedBackRefs = new List<RegexNode>();  //Holds named backreferences
        int mIndex = -1;            //Index of current char being processed
        char mCurrent = '0';        //Current char being processed
        bool mParseDone = false;    //Parse complete?

        public void AssertParse(bool b, string message) //Assert in parsing
        {
            if (!b)
            {
                throw new ArgumentException("Regex parse error at index " + mIndex + ": " + message);
            }
        }

        //This function must be used instead of trying to get backref from dictionary
        //Named references should be indexed according to regex spec in .net. This function handles that.
        RegexNode GetBackRef(int index)
        {
            try
            {
                //Backreference indexes are 1 based
                return (index <= mBackRefs.Count) ? mBackRefs[index - 1]
                                                    : mNamedBackRefs[index - mBackRefs.Count - 1];
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        //Retrieve a backreference based on its name
        RegexNode GetBackRef(string name)
        {
            foreach (RegexSubExpressionNode node in mNamedBackRefs)
            {
                if (node.Name.Equals(name))
                {
                    return node;
                }
            }

            return null;
        }

        //Move onto next char for processing
        private void NextChar()
        {
            if (mIndex < mRegex.Length - 1)
            {
                mCurrent = mRegex[++mIndex];
            }
            else
            {
                mParseDone = true;
            }
        }

        //Parse the character preceded by an escape character
        public char EscapeValue()
        {
            int value = 0;

            if (Char.ToLower(mCurrent) == 'x')   //Hexadecimal
            {
                NextChar();

                AssertParse(Uri.IsHexDigit(mCurrent), "Invalid escape character.");

                while (Uri.IsHexDigit(mCurrent) && (!mParseDone))
                {
                    value *= 16;
                    value += Char.IsDigit(mCurrent) ? mCurrent - '0' : Char.ToLower(mCurrent) - 'a' + 10;
                    NextChar();
                }
            }
            else if (mCurrent == '0')    //Octal
            {
                NextChar();

                AssertParse(mCurrent >= '0' && mCurrent <= '7', "Invalid escape character.");

                while (mCurrent >= '0' && mCurrent <= '7' && (!mParseDone))
                {
                    value *= 8;
                    value += mCurrent - '0';
                    NextChar();
                }
            }
            else if (Char.IsDigit(mCurrent))    //Decimal
            {
                while (Char.IsDigit(mCurrent) && (!mParseDone))
                {
                    value *= 10;
                    value += mCurrent - '0';
                    NextChar();
                }
            }
            else
            {
                AssertParse(false, "Invalid escape character.");
            }

            return (char)value;
        }

        //Parse the set character preceded by an escape character
        private char EscapeSetChar()
        {
            char c = '0';

            if (Char.ToLower(mCurrent) == 'x' || Char.IsDigit(mCurrent))
            {
                return EscapeValue();
            }

            switch (mCurrent)
            {
                case '^': c = '^'; break;
                case '*': c = '*'; break;
                case '\\': c = '\\'; break;
                case 'r': c = '\r'; break;
                case 'a': c = '\a'; break;
                case 'b': c = '\b'; break;
                case 'e': c = '\x1B'; break; //ESC key
                case 'n': c = '\n'; break;
                case 't': c = '\t'; break;
                case 'f': c = '\f'; break;
                case 'v': c = '\v'; break;
                case '-': c = '-'; break;
                case '[': c = '['; break;
                case ']': c = ']'; break;
                case '?': c = '?'; break;
                case '|': c = '|'; break;
                default:
                    AssertParse(false, "Invalid escape inside of set.");
                    break;
            }

            NextChar();

            return c;
        }

        //Compiles set char, also handles characters specified with escape chars
        private char CompileSetChar()
        {
            char val = mCurrent;
            NextChar();
            AssertParse(val != '-', "Invalid character inside set.");
            return (val == '\\') ? EscapeSetChar() : val;
        }

        //Compile the regex given in pattern parameter
        public RegexNode Compile(string pattern)
        {
            mRegex = new StringBuilder(pattern);
            mParseDone = false;
            NextChar();
            return CompileExpr();
        }

        //Compile the expression i.e. main body or expr in paranthesis
        public RegexNode CompileExpr()
        {
            RegexNode branch = CompileBranch();

            if (mCurrent != '|')
            {
                return branch;
            }

            RegexOrNode expr = new RegexOrNode();
            expr.Children.Add(branch);
            branch.Parent = expr;

            while (mCurrent == '|')
            {
                NextChar();
                RegexNode nextBranch = CompileBranch();
                expr.Children.Add(nextBranch);
                nextBranch.Parent = expr;
            }

            return expr;
        }

        //Compile node starting with |
        public RegexNode CompileBranch()
        {
            RegexNode piece = CompilePiece();

            if (mParseDone || mCurrent == '|' || mCurrent == ')')
            {
                return piece;
            }

            RegexAndNode andNode = new RegexAndNode();
            andNode.Children.Add(piece);
            piece.Parent = andNode;

            while (!(mParseDone || mCurrent == '|' || mCurrent == ')'))
            {
                RegexNode nextPiece = CompilePiece();
                andNode.Children.Add(nextPiece);
                nextPiece.Parent = andNode;
            }

            return andNode;
        }

        //Compile token followed by *+?{}
        public RegexNode CompilePiece()
        {
            RegexNode node = null;

            //store the old invalidating state for restoring after this node
            bool oldInvalidState = RegexCompiler.IsInvalidSection;
            //check if we want to invalidate the 'atom' node and subnodes
            if (mCurrent == '\\' && mRegex[mIndex + 1] == 'i') //entering invalidating nodes section
            {
                NextChar();
                NextChar();
                //invalidate the following node and subnodes
                RegexCompiler.IsInvalidSection = true;
            }

            RegexNode atom = CompileAtom();

            //revert the invalidating state
            RegexCompiler.IsInvalidSection = oldInvalidState;

            //check special case of invalidating a repeating node
            //have to confirm with "*+?{" to verify that it's not another type of node (that parses elsewhere)
            if (mCurrent == '\\' && mRegex[mIndex + 1] == 'i' && "*+?{".Contains(mRegex[mIndex + 2].ToString()))
            {
                NextChar();
                NextChar();
                //invalidate the repeating node
                RegexCompiler.IsInvalidSection = true;
            }

            const int MAXREPEAT = -1; //value representing infinity

            switch (mCurrent)
            {
                case '*': //zero or more repetition
                    node = new RegexRepeatNode(atom, 0, MAXREPEAT, false);
                    NextChar();
                    break;
                case '+': //one or more repetition
                    node = new RegexRepeatNode(atom, 1, MAXREPEAT, false);
                    NextChar();
                    break;
                case '?': //zero or one repetition
                    node = new RegexRepeatNode(atom, 0, 1, false);
                    NextChar();
                    break;
                case '{': //Min and max repetition limits defined
                    int nMin = 0;
                    int nMax = 0;
                    bool sameChar = false;
                    NextChar();

                    if (mCurrent == '=')
                    {
                        sameChar = true;
                        NextChar();
                    }

                    int closeIndex = mRegex.ToString().IndexOf('}', mIndex);
                    AssertParse(closeIndex != -1, "Expected '}'");

                    string[] repeatTokens = mRegex.ToString().Substring(mIndex, closeIndex - mIndex).
                                            Split(new char[] { ',' });

                    if (repeatTokens.Length == 1)
                    {
                        nMin = nMax = int.Parse(repeatTokens[0]);
                    }
                    else if (repeatTokens.Length == 2)
                    {
                        nMin = int.Parse(repeatTokens[0]);
                        //check for {n,} case
                        if (repeatTokens[1].Length > 0)
                        {
                            nMax = int.Parse(repeatTokens[1]);
                        }
                        else
                        {
                            nMax = MAXREPEAT; //only lower bound specified
                        }
                    }
                    else
                    {
                        AssertParse(false, "Repeat values cannot be parsed");
                    }

                    AssertParse(nMin <= nMax || repeatTokens[1].Length == 0, "Max repeat is less than min repeat");
                    mIndex = closeIndex;
                    NextChar();
                    node = new RegexRepeatNode(atom, nMin, nMax, sameChar);
                    break;
                default:
                    node = atom;
                    break;
            }

            //revert invalidation after generating the repeating node
            RegexCompiler.IsInvalidSection = oldInvalidState;

            return node;
        }

        //Compile token 
        public RegexNode CompileAtom()
        {
            RegexNode atom = null;
            RegexSetNode set = null;
            int start = 0;
            int end = 0;

            AssertParse(!mParseDone, "Reached end of string. No element found.");
            AssertParse(!("|)?+*{}".Contains(mCurrent.ToString())), "No element found.");

            switch (mCurrent)
            {
                case '.': //Any single char
                    atom = set = new RegexSetNode(true);
                    set.AddRange(Convert.ToChar(0), Convert.ToChar(127));
                    NextChar();
                    break;
                case '[': //Positive or negative set
                    NextChar();
                    atom = CompileSet();
                    break;
                case '(': //Sub expression
                    int refIndex = 0; //-2 -> don't capture, -1 -> named capture, 0-> indexed capture
                    NextChar();

                    //By default, subexpressions must be captured for future reference, 
                    if (mCurrent == '?')
                    {
                        NextChar();
                        if (mCurrent == ':') //If sub expression begins with ?: it means don't store reference
                        {
                            NextChar();
                            refIndex = -2;
                        }
                        else //Named backreference, extract backreference name
                        {
                            ExtractBackrefName(ref start, ref end);
                            refIndex = -1;
                        }
                    } //else use indexed backreference

                    atom = new RegexSubExpressionNode(CompileExpr());
                    AssertParse(mCurrent == ')', "Expected ')'");
                    NextChar();

                    if (refIndex == -1) //Named backreference
                    {
                        (atom as RegexSubExpressionNode).Name = mRegex.ToString().Substring(start, end - start + 1);
                        mNamedBackRefs.Add(atom);
                    }
                    else if (refIndex == 0) //Indexed backreference
                    {
                        mBackRefs.Add(atom);
                    }

                    break;
                case '^':
                case '$':
                    atom = new RegexTextNode(String.Empty);
                    NextChar();
                    break;
                case '\\':
                    NextChar();

                    if (Char.ToLower(mCurrent) == 'x' || Char.ToLower(mCurrent) == 'u' || mCurrent == '0')
                    {
                        atom = new RegexTextNode(EscapeValue().ToString());
                    }
                    else if (Char.IsDigit(mCurrent))
                    {
                        atom = GetBackRef((int)EscapeValue());
                        AssertParse(atom != null, "Couldn't find back reference");
                        atom = new RegexSubExpressionNode(atom);
                    }
                    else if (mCurrent == 'k') //referencing a backreference by name
                    {
                        NextChar();
                        ExtractBackrefName(ref start, ref end);
                        atom = GetBackRef(mRegex.ToString().Substring(start, end - start + 1));
                        AssertParse(atom != null, "Couldn't find back reference");
                        atom = new RegexSubExpressionNode(atom); //Create a copy of the referenced node
                    }
                    else
                    {
                        atom = CompileSimpleMacro(mCurrent);
                        NextChar();
                    }
                    break;
                default:
                    int closeIndex = mRegex.ToString().IndexOfAny("-*+?(){}\\[]^$.|".ToCharArray(), mIndex + 1);

                    if (closeIndex == -1)
                    {
                        mParseDone = true;
                        closeIndex = mRegex.Length - 1;
                        atom = new RegexTextNode(mRegex.ToString().Substring(mIndex, closeIndex - mIndex + 1));
                    }
                    else
                    {
                        atom = new RegexTextNode(mRegex.ToString().Substring(mIndex, closeIndex - mIndex));
                    }

                    mIndex = closeIndex;
                    mCurrent = mRegex[mIndex];
                    break;
            }

            return atom;
        }

        //Parse backreference name in form of <name> or 'name'
        public void ExtractBackrefName(ref int start, ref int end)
        {
            char tChar = mCurrent;
            AssertParse(tChar == '\'' || tChar == '<', "Backref must begin with ' or <.");

            //Set the expected end character, if start char is < then expect >, otherwise expect '
            if (tChar == '<')
            {
                tChar = '>';
            }

            NextChar();

            AssertParse((Char.ToLower(mCurrent) >= 'a' && Char.ToLower(mCurrent) <= 'z') || mCurrent == '_',
                                "Invalid characters in backreference name.");
            start = mIndex;
            NextChar();

            while (mCurrent == '_' || Char.IsLetterOrDigit(mCurrent))
            {
                NextChar();
            }

            AssertParse(mCurrent == tChar, "Name end not found.");
            end = mIndex;
            NextChar();
        }

        //Compile a character set (i.e expressions like [abc], [A-Z])
        public RegexNode CompileSet()
        {
            RegexNode atom = null;
            char cStart, cEnd;
            RegexSetNode set;

            if (mCurrent == ':')
            {
                NextChar();
                int closeIndex = mRegex.ToString().IndexOf(":]", mIndex);
                atom = CompileMacro(mIndex, closeIndex - mIndex);
                mIndex = closeIndex;
                NextChar();
                NextChar();
                return atom;
            }

            if (mCurrent == '^')
            {
                atom = set = new RegexSetNode(false);
                NextChar();
            }
            else
            {
                atom = set = new RegexSetNode(true);
            }

            if (mCurrent == '-' || mCurrent == ']') //if - or ] are specified as the first char, escape is not required
            {
                set.AddChars(mCurrent.ToString());
                NextChar();
            }

            while ((!mParseDone) && (mCurrent != ']'))
            {
                cStart = CompileSetChar();

                if (mCurrent == '-')
                {
                    NextChar();
                    AssertParse(!mParseDone && mCurrent != ']', "End of range is not specified.");
                    cEnd = CompileSetChar();
                    set.AddRange(cStart, cEnd);
                }
                else
                {
                    set.AddChars(cStart.ToString());
                }
            }

            AssertParse(mCurrent == ']', "Expected ']'.");
            NextChar();
            return atom;
        }

        //Compile \d \D \s \S etc.
        public RegexNode CompileSimpleMacro(char c)
        {
            RegexNode node = null;
            RegexSetNode set = null;

            if (@"[]{}()*-+.?\|".Contains(c.ToString()))
            {
                return new RegexTextNode(c.ToString());
            }

            switch (c)
            {
                case 'd': // [0-9]
                    node = set = new RegexSetNode(true);
                    set.AddRange('0', '9');
                    break;
                case 'D': // [^0-9]
                    node = set = new RegexSetNode(false);
                    set.AddRange('0', '9');
                    break;
                case 's':
                    node = set = new RegexSetNode(true);
                    set.AddChars(" \r\n\f\v\t");
                    break;
                case 'S':
                    node = set = new RegexSetNode(false);
                    set.AddChars(" \r\n\f\v\t");
                    break;
                case 'w': // [a-zA-Z0-9_]
                    node = set = new RegexSetNode(true);
                    set.AddRange('a', 'z');
                    set.AddRange('A', 'Z');
                    set.AddRange('0', '9');
                    set.AddChars("_");
                    break;
                case 'W': // [^a-zA-Z0-9_]
                    node = set = new RegexSetNode(false);
                    set.AddRange('a', 'z');
                    set.AddRange('A', 'Z');
                    set.AddRange('0', '9');
                    set.AddChars("_");
                    break;
                case 'f':
                    node = new RegexTextNode("\f");
                    break;
                case 'n':
                    node = new RegexTextNode("\n");
                    break;
                case 'r':
                    node = new RegexTextNode("\r");
                    break;
                case 't':
                    node = new RegexTextNode("\t");
                    break;
                case 'v':
                    node = new RegexTextNode("\v");
                    break;
                case 'A':
                case 'Z':
                case 'z':
                    node = new RegexTextNode(String.Empty);
                    break;
                default:
                    AssertParse(false, "Invalid escape.");
                    break;
            }

            return node;
        }

        //Compile [:alpha:] [:punct:] etc
        public RegexNode CompileMacro(int index, int len)
        {
            AssertParse(len >= 0, "Cannot parse macro.");
            string substr = mRegex.ToString().Substring(index, len);
            string expanded = null;

            switch (substr)
            {
                case "alnum": expanded = "[a-zA-Z0-9]"; break;
                case "alpha": expanded = "[a-zA-Z]"; break;
                case "upper": expanded = "[A-Z]"; break;
                case "lower": expanded = "[a-z]"; break;
                case "digit": expanded = "[0-9]"; break;
                case "xdigit": expanded = "[A-F0-9a-f]"; break;
                case "space": expanded = "[ \t]"; break;
                case "print": expanded = "[\\x20-\\x7F]"; break;
                case "punct": expanded = "[,;.!'\"]"; break;
                case "graph": expanded = "[\\x80-\\xFF]"; break;
                case "cntrl": expanded = "[\x00-\x1F\x7F]"; break;
                case "blank": expanded = "[ \t\r\n\f]"; break;
                case "guid": expanded = "[A-F0-9]{8}(-[A-F0-9]{4}){3}-[A-F0-9]{12}"; break;
                default: AssertParse(false, "Cannot parse macro."); break;
            }

            RegexCompiler subcompiler = new RegexCompiler();
            return subcompiler.Compile(expanded);
        }
    }
}
