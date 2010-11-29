// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Test.VariationGeneration.Constraints;

namespace Microsoft.Test.VariationGeneration
{
    /// <summary>
    /// Contains all the parameters and constraints for the system under test and produces a 
    /// set of variations using combinatorial test techniques.
    /// </summary>
    /// <remarks>
    /// Exhaustively testing all possible inputs to any nontrivial software component is generally impossible
    /// due to the enormous number of variations. Combinatorial testing is one approach to achieve high coverage
    /// with a much smaller set of variations. Pairwise, the most common combinatorial strategy, tests every possible 
    /// pair of values. Higher orders of combinations (3-wise, 4-wise, etc.) can also be used for higher coverage
    /// at the expense of more variations. See <a href="http://pairwise.org">Pairwise Testing</a> and 
    /// <a href="http://www.pairwise.org/docs/pnsqc2006/PNSQC%20140%20-%20Jacek%20Czerwonka%20-%20Pairwise%20Testing%20-%20BW.pdf">
    /// Pairwise Testing in Real World</a> for more resources.
    /// </remarks>
    /// 
    /// <example>
    /// <p>
    /// The following example demonstrates creating variations for a vacation planner with a signature like this:
    /// CallVacationPlanner(string destination, int hotelQuality, string activity). It demonstrates that certain
    /// activities are only available for certain destinations.
    /// </p>
    /// <code>
    /**
            Parameter destination = 
                new Parameter("Destination") { "Whistler", "Hawaii", "Las Vegas" };

            Parameter hotelQuality = 
                new Parameter("Hotel Quality") { 5, 4, 3, 2, 1 };

            Parameter activity = 
               new Parameter("Activity") { "gambling", "swimming", "shopping", "skiing" };

            List&lt;Parameter&gt; parameters = new List&lt;Parameter&gt; { destination, hotelQuality, activity };
            List&lt;Constraint&gt; constraints = new List&lt;Constraint&gt;
            {
                new IfThenConstraint
                {
                    If = destination.Equal("Whistler").Or(destination.Equal("Hawaii")),
                    Then = activity.NotEqual("gambling")                        
                },
                new IfThenConstraint
                {
                    If = destination.Equal("Las Vegas").Or(destination.Equal("Hawaii")),
                    Then = activity.NotEqual("skiing")
                },
                new IfThenConstraint
                {
                    If = destination.Equal("Whistler"),
                    Then = activity.NotEqual("swimming")
                },
            };

            Model model = new Model(parameters, constraints);

            // call the method under test with each generated variation
            foreach (var variation in model.GenerateVariations())
            {
                CallVacationPlanner(
                    (string)variation[destination.Name], 
                    (int)variation[hotelQuality.Name], 
                    (string)variation[activity.Name]);
            }
     */
    /// </code>
    /// </example>
    /// <example>
    /// <p>
    /// The following example demonstrates creating variations for a vacation planner that adds weights and tags
    /// to certain values.  Adding weights changes the frequency a value will occur.  Adding tags allows expected values
    /// to be added to variations.
    /// </p>
    /// <code>
    /**
            Parameter destination = 
                new Parameter("Destination") 
                { 
                    "Whistler", 
                    "Hawaii",
                    // specify that Las Vegas should be emphasized
                    new ParameterValue("Las Vegas") { Weight = 5.0 },
                    // specify the expected result when Cleveland is specified
                    new ParameterValue("Cleveland") { Tag = false }
                };

            Parameter hotelQuality = 
                new Parameter("Hotel Quality") { 5, 4, 3, 2, 1 };

            Parameter activity = 
                new Parameter("Activity") { "gambling", "swimming", "shopping", "skiing" };

            Parameter parameters = new List&lt;Parameter&gt; { destination, hotelQuality, activity };

            Model model = new Model(parameters) { DefaultVariationTag = true };

            foreach (var variation in model.GenerateVariations(2))
            {
                if (CallVacationPlanner(
                        (string)variation[destination.Name],
                        (int)variation[hotelQuality.Name],
                        (string)variation[activity.Name]) != (bool)variation.Tag)
                {
                    Console.WriteLine("Variation failed.");
                }
            }
     */
    /// </code>
    /// </example>
    public class Model
    {
        /// <summary>
        /// Initializes a new model with the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public Model(IEnumerable<Parameter> parameters) : this(parameters, null)
        {
        }

        /// <summary>
        /// Initializes a new model with the specified parameters and constraints.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="constraints">The constraints.</param>
        public Model(IEnumerable<Parameter> parameters, IEnumerable<Constraint> constraints)
        {
            if (parameters != null)
            {
                this.parameters.AddRange(parameters);
            }

            if (constraints != null)
            {
                this.constraints.AddRange(constraints);
            }
        }

        List<Parameter> parameters = new List<Parameter>();

        /// <summary>
        /// The parameters in the model.
        /// </summary>
        public IList<Parameter> Parameters { get { return parameters; } }

        List<Constraint> constraints = new List<Constraint>();

        /// <summary>
        /// The constraints in the model.
        /// </summary>
        public ICollection<Constraint> Constraints { get { return constraints; } }

        /// <summary>
        /// The default tag on generated variations, set when no value in the variation has been tagged.  Default is null.
        /// </summary>
        public object DefaultVariationTag { get; set; }

        /// <summary>
        /// Generate an order-wise set of variations using a constant seed.
        /// </summary>
        /// <param name="order">The order or the combinations selected (2 is every pair, 3 is every triple, etc). Must be between 1 and the number of parameters.</param>
        /// <returns>The variations.</returns>
        public virtual IEnumerable<Variation> GenerateVariations(int order)
        {
            return GenerateVariations(order, defaultSeedValue);
        }

        /// <summary>
        /// Generate and order-wise set of variations using the specified seed for random generation.
        /// </summary>
        /// <param name="order">The order or the combinations selected (2 is every pair, 3 is every triple, etc). Must be between 1 and the number of parameters.</param>
        /// <param name="seed">The seed used for random generation.</param>
        /// <returns>The variations.</returns>
        public virtual IEnumerable<Variation> GenerateVariations(int order, int seed)
        {
            if (order < 1 || order > Parameters.Count)
            {
                throw new ArgumentOutOfRangeException("order", order, "order must be between 1 and the number of parameters.");
            }

            return VariationGenerator.GenerateVariations(this, order, seed);
        }

        private const int defaultSeedValue = 12345;
    }
}
