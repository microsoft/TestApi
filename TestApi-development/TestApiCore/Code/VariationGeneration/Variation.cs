// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Test.VariationGeneration
{
    /// <summary>
    /// Represents a tuple with a single value for every <see cref="Parameter"/> 
    /// in the <see cref="Model"/>. The Model produces these using combinatorial test techniques.
    /// </summary>
    /// <remarks>
    /// Exhaustively testing all possible inputs to any nontrivial software component is generally impossible
    /// due to the enormous number of variations. Combinatorial testing is one approach to achieve high coverage
    /// with a much smaller set of variations. Pairwise, the most common combinatorial strategy, test every possible 
    /// pair of values.  Higher orders of combinations (3-wise, 4-wise, etc.) can also be used for higher coverage
    /// at the expense of more variations. See <a href="http://pairwise.org">Pairwise Testing</a> and 
    /// <a href="http://www.pairwise.org/docs/pnsqc2006/PNSQC%20140%20-%20Jacek%20Czerwonka%20-%20Pairwise%20Testing%20-%20BW.pdf">
    /// Pairwise Testing in Real World</a> for more resources.
    /// </remarks>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Scope = "type", Target = "Microsoft.Test.VariationGeneration.Variation", Justification="The suggested name VariationDictionary is confusing.")]        
    [SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Scope = "type", Target = "Microsoft.Test.VariationGeneration.Variation", Justification = "Not currently used across app domains and this pollutes the om.")]
    public class Variation : Dictionary<string, object>
    {               


    }
}
