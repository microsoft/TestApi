//RegExGen.cs -- Includes all the methods for regular expression string generator
//Author - Cagri Aslan (caslan), David Henry (v-dahenr) 03/2007
//The code is based on the algorithm in randstrgen tool

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

class RegExGen
{
    //The random number generator used for all random choices
    internal static Random rndGen = new Random();

    //Sets the seed for the random number generator
    //seed - new seed for the random number generator
    public static void SetSeed( int seed )
    {
        rndGen = new Random(seed);
    }

    //Generates a string based on the given regular expression
    //if any nodes are prepended with \i, then one of these nodes will be chosen
    //at random to be invalidated
    //regex - Regular expression used to generate the string
    //returns - The generated string
    public static string NextString( string regex )
    {
        //reset the static variables
        RECompiler.IsInvalidSection = false;
        RECompiler.InvalidNode = null;
        RECompiler.InvalidableNodes.Clear();

        //construct the RegEx tree
        RECompiler compiler = new RECompiler();
        RENode node = compiler.Compile(regex);

        //search for a signal to invalidate a node
        if (regex.IndexOf("\\i") != -1)
        {
            //something should have been invalidated
            //select a node to invalidate
            if (RECompiler.InvalidableNodes.Count == 0)
            {
                throw new ArgumentException("Asked to generate invalid: Impossible to invalidate");
            }
            RECompiler.InvalidNode = RECompiler.InvalidableNodes[RegExGen.rndGen.Next(RECompiler.InvalidableNodes.Count)];

            //Mark REOrNodes and RERepeatNodes to ensure that the invalid node will be part of the string
            RECompiler.InvalidNode.ReservePath(null);
        }
        
        //generate and return the string
        string result = node.Generate();

        if (RECompiler.InvalidNode != null)
        {
            //confirm that the generated string is invalid (e.g. [a-z]|[^a-z] will always fail)
            Regex compare = new Regex("^" + regex.Replace("\\i", "") + "$");
            if (compare.IsMatch(result))
            {
                throw new ArgumentException(regex + ": Did not generate invalid string: " + result);
            }
        }

        return result;
    }

}
