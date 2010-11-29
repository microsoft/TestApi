// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Test.VariationGeneration;
using Microsoft.Test.VariationGeneration.Constraints;
using Xunit;

namespace Tests
{
    public class VariationGenerationTests
    {
        /// <summary>
        /// This test creates a pairwise set of variations for a vacation planner service.  There
        /// are constraints on what activities are available at which destination.
        /// </summary>
        [Fact]
        public void SampleTest()
        {
            var destination = new Parameter<string>("Destination") 
            { 
                "Whistler", 
                "Hawaii",
                "Las Vegas"
            };

            var hotelQuality = new Parameter<int>("HotelQuality") { 5, 4, 3, 2, 1 };

            var activity = new Parameter<string>("Activity") { "gambling", "swimming", "shopping", "skiing" };

            var parameters = new List<ParameterBase> { destination, hotelQuality, activity };
            var constraints = new List<Constraint<VacationVariation>>
            {
                Constraint<VacationVariation>
                    .If(v => v.Destination == "Whistler" || v.Destination == "Hawaii")
                    .Then(v => v.Activity != "gambling"),
                Constraint<VacationVariation>
                    .If(v => v.Destination == "Las Vegas" || v.Destination == "Hawaii")
                    .Then(v => v.Activity != "skiing"),
                Constraint<VacationVariation>
                    .If(v => v.Destination == "Whistler")
                    .Then(v => v.Activity != "swimming"),
            };

            var model = new Model<VacationVariation>(parameters, constraints);

            foreach (var variation in model.GenerateVariations(2))
            {
                Assert.True(
                    CallVacationPlanner(
                        variation.Destination,
                        variation.HotelQuality,
                        variation.Activity));
                
            }
        }

        /// <summary>
        /// This test creates a pairwise set of variations for a vacation planner service.  There
        /// are constraints on what activities are available at which destination.  Also expected results
        /// are specified using the Tag mechanism.
        /// </summary>
        [Fact]
        public void SampleWithExpectedResultsTest()
        {
            var destination = new Parameter<string>("Destination") 
            { 
                "Whistler", 
                "Hawaii",
                // specify that Las Vegas should be emphasized
                new ParameterValue<string>("Las Vegas") { Weight = 5.0 },
                // specify the expected result when Cleveland is specified
                new ParameterValue<string>("Cleveland") { Tag = Results.ReturnsFalse }
            };

            var hotelQuality = new Parameter<int>("HotelQuality") 
            { 5, 
              4,
              3,
              2,
              1,
              // specify the expected result when -1 is specified
              new ParameterValue<int>(-1){ Tag = Results.ThrowsOutOfRangeException } 
            };

            var activity = new Parameter<string>("Activity") { "gambling", "swimming", "shopping", "skiing" };

            var parameters = new List<ParameterBase> { destination, hotelQuality, activity };
            var constraints = new List<Constraint<Variation>>
            {
                Constraint<Variation>
                    .If(v => destination.GetValue(v) == "Whistler" || destination.GetValue(v) == "Hawaii")
                    .Then(v => activity.GetValue(v) != "gambling"),
                Constraint<Variation>
                    .If(v => destination.GetValue(v) == "Las Vegas" || destination.GetValue(v) == "Hawaii")
                    .Then(v => activity.GetValue(v) != "skiing"),
                Constraint<Variation>
                    .If(v => destination.GetValue(v) == "Whistler")
                    .Then(v => activity.GetValue(v) != "swimming"),
            };

            Model model = new Model(parameters, constraints) { DefaultVariationTag = Results.Default };

            foreach (var variation in model.GenerateVariations(2))
            {
                switch ((Results)variation.Tag)
                {
                    case Results.ReturnsFalse:
                        Assert.False(
                            CallVacationPlanner(
                                destination.GetValue(variation),
                                hotelQuality.GetValue(variation),
                                activity.GetValue(variation)));
                        break;
                    case Results.ThrowsOutOfRangeException:
                        Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
                            CallVacationPlanner(
                                destination.GetValue(variation),
                                hotelQuality.GetValue(variation),
                                activity.GetValue(variation)));
                        break;
                    default:
                        Assert.True(
                            CallVacationPlanner(
                                destination.GetValue(variation),
                                hotelQuality.GetValue(variation),
                                activity.GetValue(variation)));
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

    public class VacationVariation
    {
        public string Destination { get; set; }
        public int HotelQuality { get; set; }
        public string Activity { get; set; }
    }
}
