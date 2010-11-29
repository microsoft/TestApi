// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Test.VariationGeneration;
using Microsoft.Test.VariationGeneration.Constraints;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class VariationGenerationTests
    {
        /// <summary>
        /// This test creates a pairwise set of variations for a vacation planner service.  There
        /// are constraints on what activities are available at which destination.
        /// </summary>
        [TestMethod]
        public void SampleTest()
        {
            var destination = new Parameter("Destination") 
            { 
                "Whistler", 
                "Hawaii",
                "Las Vegas"
            };

            var hotelQuality = new Parameter("Hotel Quality") { 5, 4, 3, 2, 1 };

            var activity = new Parameter("Activity") { "gambling", "swimming", "shopping", "skiing" };

            var parameters = new List<Parameter> { destination, hotelQuality, activity };
            var constraints = new List<Constraint>
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

            foreach (var variation in model.GenerateVariations(2))
            {
                Assert.IsTrue(
                    CallVacationPlanner(
                        (string)variation[destination.Name],
                        (int)variation[hotelQuality.Name],
                        (string)variation[activity.Name]));
            }
        }

        /// <summary>
        /// This test creates a pairwise set of variations for a vacation planner service.  There
        /// are constraints on what activities are available at which destination.
        /// </summary>
        [TestMethod]
        public void SampleWithExpectedResultsTest()
        {
            var destination = new Parameter("Destination") 
            { 
                "Whistler", 
                "Hawaii",
                // specify that Las Vegas should be emphasized
                new ParameterValue("Las Vegas") { Weight = 5.0 },
                // specify the expected result when Cleveland is specified
                new ParameterValue("Cleveland") { Tag = Results.ReturnsFalse }
            };

            var hotelQuality = new Parameter("Hotel Quality") 
            { 5, 
              4,
              3,
              2,
              1,
              // specify the expected result when -1 is specified
              new ParameterValue(-1){ Tag = Results.ThrowsOutOfRangeException } 
            };

            var activity = new Parameter("Activity") { "gambling", "swimming", "shopping", "skiing" };

            var parameters = new List<Parameter> { destination, hotelQuality, activity };
            var constraints = new List<Constraint>
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

            Model model = new Model(parameters, constraints) { DefaultVariationTag = Results.Default };

            foreach (var variation in model.GenerateVariations(2))
            {
                switch ((Results)variation.Tag)
                {
                    case Results.ReturnsFalse:
                        Assert.IsFalse(
                            CallVacationPlanner(
                                (string)variation[destination.Name],
                                (int)variation[hotelQuality.Name],
                                (string)variation[activity.Name]));
                        break;
                    case Results.ThrowsOutOfRangeException:
                        try
                        {
                            CallVacationPlanner(
                                (string)variation[destination.Name],
                                (int)variation[hotelQuality.Name],
                                (string)variation[activity.Name]);
                            Assert.Fail("Expected exception not thrown.");
                        }
                        catch (Exception e)
                        {
                            Assert.IsInstanceOfType(e, typeof(ArgumentOutOfRangeException));
                        }
                        break;
                    default:
                        Assert.IsTrue(
                            CallVacationPlanner(
                                (string)variation[destination.Name],
                                (int)variation[hotelQuality.Name],
                                (string)variation[activity.Name]));
                        break;
                }
            }
        }

        /// <summary>
        /// This is the function under test.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="hotelQuality">The hotel quality.</param>
        /// <param name="activity">The activity.</param>
        /// <returns>Whether the planner was successful.</returns>
        public static bool CallVacationPlanner(string destination, int hotelQuality, string activity)
        {
            if (hotelQuality < 1 || hotelQuality > 5)
            {
                throw new ArgumentOutOfRangeException("hotelQuality");
            }

            return destination == "Cleveland" ? false : true;
        }
    }

    public enum Results
    {
        Default,
        ReturnsFalse,
        ThrowsOutOfRangeException
    }
}
