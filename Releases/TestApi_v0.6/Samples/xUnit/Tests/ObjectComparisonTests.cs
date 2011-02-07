// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Test.ObjectComparison;
using Xunit;

namespace Tests
{
    public class ObjectComparisonTests
    {
        /// <summary>
        /// Compare two Person objects with differing 
        /// property values and get a collection of 
        /// mismatches.
        [Fact]
        public void CompareObjects()
        {
            // Create two objects with differing property values
            Person leftObject = new Person()
            {
                Name = "Person1",
                Age = 15,
                Parent = new Person()
                {
                    Name = "ParentOfPerson1",
                }
            };

            Person rightObject = new Person()
            {
                Name = "Person2",
                Age = 15,
                Parent = new Person()
                {
                    Name = "ParentOfPerson2",
                }
            };

            // Create the object graph factory
            ObjectGraphFactory factory = new PublicPropertyObjectGraphFactory();

            // Create a comparer using the factory
            ObjectComparer comparer = new ObjectComparer(factory);

            // Compare the objects
            IEnumerable<ObjectComparisonMismatch> mismatches;
            bool match = comparer.Compare(leftObject, rightObject, out mismatches);

            // Validate the mismatches when the objects do not match
            string[] expectedMismatches = new string[]
            {
                "ObjectValuesDoNotMatch:Left=RootObject.Parent.Name(ParentOfPerson1) Right=RootObject.Parent.Name(ParentOfPerson2)",
                "ObjectValuesDoNotMatch:Left=RootObject.Name(Person1) Right=RootObject.Name(Person2)",
            };

            string[] actualMismatches = StringFromMismatches(mismatches);

            Assert.False(match);
            Assert.True(actualMismatches.Length == expectedMismatches.Length);
            for (int index = 0; index < expectedMismatches.Length; index++)
            {
                Assert.Equal(expectedMismatches[index], actualMismatches[index]);
            }
        }

        private static string[] StringFromMismatches(IEnumerable<ObjectComparisonMismatch> mismatches)
        {
            List<string> outputLines = new List<string>();
            foreach (ObjectComparisonMismatch mismatch in mismatches)
            {
                string message = String.Format(CultureInfo.InvariantCulture,
                    "{0}:Left={1}({2}) Right={3}({4})",
                    mismatch.MismatchType,
                    mismatch.LeftObjectNode == null ? "Null" : mismatch.LeftObjectNode.QualifiedName,
                    mismatch.LeftObjectNode == null ? "Null" : mismatch.LeftObjectNode.ObjectValue ?? "Null",
                    mismatch.RightObjectNode == null ? "Null" : mismatch.RightObjectNode.QualifiedName,
                    mismatch.RightObjectNode == null ? "Null" : mismatch.RightObjectNode.ObjectValue ?? "Null");

                outputLines.Add(message);
            }

            return outputLines.ToArray();
        }
    }

    #region Test Type

    /// <summary>
    /// Person type with public properties Name, Age 
    /// and Parent.
    /// </summary>
    public class Person
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public Person Parent { get; set; }
    }

    #endregion
}