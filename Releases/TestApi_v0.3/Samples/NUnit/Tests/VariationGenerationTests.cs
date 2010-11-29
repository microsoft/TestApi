using System.Collections.Generic;
using Microsoft.Test.VariationGeneration;
using Microsoft.Test.VariationGeneration.Constraints;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class VariationGenerationTests
    {
        /// <summary>
        /// This test creates a pairwise set of variations for a vacation planner service.  There
        /// are constraints on what activities are available at which destination.
        /// </summary>
        [Test]
        public void SampleTest()
        {
            var destination = new Parameter("Destination") { "Whistler", "Hawaii", "Las Vegas" };

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

            foreach (var variation in model.GenerateVariations())
            {
                Assert.IsTrue(
                    CallVacationPlanner((string)variation[destination.Name],
                                        (int)variation[hotelQuality.Name],
                                        (string)variation[activity.Name]));
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
            return true;
        }
    }
}
