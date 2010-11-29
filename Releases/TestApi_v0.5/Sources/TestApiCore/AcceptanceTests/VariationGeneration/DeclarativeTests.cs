// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Test.VariationGeneration;
using Xunit;

namespace Microsoft.Test.AcceptanceTests.VariationGeneration
{
    interface ITestVariation
    {
        int IntValue { get; set; }

        bool BoolValue { get; set; }

        string StringValue { get; set; }
    }

    public class SimpleVariation : ITestVariation
    {
        [Parameter(0, 5, 10)]
        public int IntValue { get; set; }

        [Parameter(true)]
        public bool BoolValue { get; set; }

        [Parameter("There", "be", "giants")]
        public string StringValue { get; set; }
    }

    public class EquivalenceClassVariation : ITestVariation
    {
        [Parameter(0)]
        [Parameter(5)]
        [Parameter(10)]
        public int IntValue
        {
            get;
            set;
        }

        [Parameter(true)]
        public bool BoolValue
        {
            get;
            set;
        }

        [Parameter("There")]
        [Parameter("be")]
        [Parameter("giants")]
        public string StringValue
        {
            get;
            set;
        }
    }

    #region Weighted classes tests
    public abstract class TestWeightVariation : ITestVariation
    {
        [Parameter(0)]
        [Parameter(5)]
        [Parameter(10)]
        public int IntValue
        {
            get;
            set;
        }

        public abstract bool BoolValue
        {
            get;
            set;
        }

        [Parameter("a", "b", "c", "d", "e", "f")]
        public string StringValue
        {
            get;
            set;
        }
    }

    public class LessWeightedVariation : TestWeightVariation
    {
        [Parameter(true, Weight = .5F)]
        [Parameter(false)]
        public override bool BoolValue
        {
            get;
            set;
        }
    }

    public class MoreWeightedVariation : TestWeightVariation
    {
        [Parameter(true, Weight = 10F)]
        [Parameter(false)]
        public override bool BoolValue
        {
            get;
            set;
        }
    }

    public class NonWeightedVariation : TestWeightVariation
    {
        [Parameter(true)]
        [Parameter(false)]
        public override bool BoolValue
        {
            get;
            set;
        }
    }
    #endregion

    public class DeclarativeModelTest
    {
        static Dictionary<T, bool> GetExpectedVariations<T>() where T : ITestVariation, new()
        {
            Dictionary<T, bool> expectedVariations = new Dictionary<T, bool>();

            Parameter<int> intValue = new Parameter<int>("IntValue");
            intValue.Add(0);
            intValue.Add(5);
            intValue.Add(10);
            Parameter<bool> boolValue = new Parameter<bool>("BoolValue");
            boolValue.Add(true);
            Parameter<string> stringValue = new Parameter<string>("StringValue");
            stringValue.Add("There");
            stringValue.Add("be");
            stringValue.Add("giants");
            List<ParameterBase> parameters = new List<ParameterBase>() { intValue, boolValue, stringValue };

            Model<Variation> model = new Model<Variation>(parameters);
            foreach (Variation variation in model.GenerateVariations(2))
            {
                expectedVariations.Add(new T()
                {
                    BoolValue = (bool)variation["BoolValue"],
                    IntValue = (int)variation["IntValue"],
                    StringValue = (string)variation["StringValue"]
                },
                false);
            }
            return expectedVariations;
        }

        static void TestGeneration<T>() where T : ITestVariation, new()
        {
            Dictionary<T, bool> expectedVariations = GetExpectedVariations<T>();
            Model<T> model = new Model<T>();
            T[] variations = model.GenerateVariations(2).ToArray();
            Assert.True(variations.Length == expectedVariations.Count);

            // attempt to verify that every returned variation is found
            // in expected variation dictionary
            foreach (T variation in variations)
            {
                foreach (T expectedVariation in expectedVariations.Keys.ToArray())
                {
                    if (variation.BoolValue == expectedVariation.BoolValue &&
                        variation.IntValue == expectedVariation.IntValue &&
                        variation.StringValue == expectedVariation.StringValue &&
                        expectedVariations[expectedVariation] == false)
                    {
                        expectedVariations[expectedVariation] = true;
                    }
                }
            }

            // success if each expected variation was returned exactly once
            Assert.True(expectedVariations.Values.All(v => v));
        }

        /// <summary>
        /// Verifies that simple model declared through attributes generated correctly.
        /// </summary>
        [Fact]
        public void TestSimple()
        {
            TestGeneration<SimpleVariation>();
        }

        /// <summary>
        /// Verifies that equivalence classes declared through attributes function correctly.
        /// </summary>
        [Fact]
        public void TestEquivalenceClasses()
        {
            TestGeneration<EquivalenceClassVariation>();
        }

        /// <summary>
        /// Verifies that parameter value weights declared through attributes function correctly.
        /// </summary>
        [Fact]
        public void TestWeightedPartitions()
        {
            Model<MoreWeightedVariation> modelMoreWeight = new Model<MoreWeightedVariation>();
            ITestVariation[] variationsMoreWeight = modelMoreWeight.GenerateVariations(2).ToArray();
            int countMoreWeight = variationsMoreWeight.Count(v => v.BoolValue == true);

            Model<LessWeightedVariation> modelLessWeight = new Model<LessWeightedVariation>();
            ITestVariation[] variationsLessWeight = modelLessWeight.GenerateVariations(2).ToArray();
            int countLessWeight = variationsLessWeight.Count(v => v.BoolValue == true);

            Model<NonWeightedVariation> modelNoWeight = new Model<NonWeightedVariation>();
            ITestVariation[] variationsNoWeight = modelNoWeight.GenerateVariations(2).ToArray();
            int countNoWeight = variationsNoWeight.Count(v => v.BoolValue == true);
            
            Assert.True(countLessWeight < countNoWeight);
            Assert.True(countMoreWeight > countNoWeight);
        }
    }
}
