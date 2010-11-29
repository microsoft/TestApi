// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Test.ObjectComparison;
using Xunit;

namespace Microsoft.Test.AcceptanceTests.ObjectComparison
{
    /// <summary>
    /// Tests for the ObjectComparer API
    /// </summary>
    public class ObjectComparerTests
    {
        #region Basic comparison tests

        [Fact]
        public void CompareBasicTypesMaxValues()
        {
            BasicTypes leftObject = new BasicTypes()
            {
                BoolPrimitive = true,
                BytePrimitive = byte.MaxValue,
                CharValue = char.MaxValue,
                DoublePrimitive = double.MaxValue,
                FloatPrimitive = float.MaxValue,
                IntPrimitive = int.MaxValue,
                LongPrimitive = long.MaxValue,
                ShortPrimitive = short.MaxValue,
                StringPrimitive = string.Empty,
                TimeSpanValue = TimeSpan.MaxValue,
            };

            BasicTypes rightObject = leftObject.Clone();

            ObjectGraphFactory factory = new PublicPropertyObjectGraphFactory();
            ObjectComparer comparer = new ObjectComparer(factory);
            bool match = comparer.Compare(leftObject, rightObject);

            Assert.True(match, "Basic types did not match");
        }

        [Fact]
        public void CompareBasicTypesMinValues()
        {
            BasicTypes leftObject = new BasicTypes()
            {
                BoolPrimitive = false,
                BytePrimitive = byte.MinValue,
                CharValue = char.MinValue,
                DoublePrimitive = double.MinValue,
                FloatPrimitive = float.MinValue,
                IntPrimitive = int.MinValue,
                LongPrimitive = long.MinValue,
                ShortPrimitive = short.MinValue,
                StringPrimitive = "some string",
                TimeSpanValue = TimeSpan.MinValue,
            };

            BasicTypes rightObject = new BasicTypes()
            {
                BoolPrimitive = true,
                BytePrimitive = byte.MaxValue,
                CharValue = char.MaxValue,
                DoublePrimitive = double.MaxValue,
                FloatPrimitive = float.MaxValue,
                IntPrimitive = int.MaxValue,
                LongPrimitive = long.MaxValue,
                ShortPrimitive = short.MaxValue,
                StringPrimitive = string.Empty,
                TimeSpanValue = TimeSpan.MaxValue,
            };

            ObjectGraphFactory factory = new PublicPropertyObjectGraphFactory();
            ObjectComparer comparer = new ObjectComparer(factory);
            bool match = comparer.Compare(leftObject, rightObject);

            Assert.False(match, "Basic types matched when they should not");            
        }

        [Fact]
        public void BasicTypesDiffer()
        {
            BasicTypes leftObject = new BasicTypes()
            {
                BoolPrimitive = false,
                BytePrimitive = byte.MinValue,
                CharValue = char.MinValue,
                DoublePrimitive = double.MinValue,
                FloatPrimitive = float.MinValue,
                IntPrimitive = int.MinValue,
                LongPrimitive = long.MinValue,
                ShortPrimitive = short.MinValue,
                StringPrimitive = "some string",
                TimeSpanValue = TimeSpan.MinValue,
            };

            BasicTypes rightObject = leftObject.Clone();

            ObjectGraphFactory factory = new PublicPropertyObjectGraphFactory();
            ObjectComparer comparer = new ObjectComparer(factory);
            bool match = comparer.Compare(leftObject, rightObject);

            Assert.True(match, "Basic types did not match");
        }

        [Fact]
        public void CompareList()
        {
            List<string> leftObject = new List<string>()
            {
                "Hello",
                "World",
            };

            List<string> rightObject = new List<string>()
            {
                "Hello",
                "World",
            };

            ObjectGraphFactory factory = new PublicPropertyObjectGraphFactory();
            ObjectComparer comparer = new ObjectComparer(factory);
            bool match = comparer.Compare(leftObject, rightObject);

            Assert.True(match, "List<string> did not match");
        }

        [Fact]
        public void CompareDictionary()
        {
            Dictionary<int, string> leftObject = new Dictionary<int, string>()
            {
                { 10, "Hello" },
                { 20, "World" },
            };

            Dictionary<int, string> rightObject = new Dictionary<int, string>()
            {
                { 10, "Hello" },
                { 20, "World" },
            };

            ObjectGraphFactory factory = new PublicPropertyObjectGraphFactory();
            ObjectComparer comparer = new ObjectComparer(factory);
            bool match = comparer.Compare(leftObject, rightObject);

            Assert.True(match, "Dictionary<int, string> did not match");
        }

        [Fact]
        public void TestSimpleClassHierarchy()
        {
            var leftObject = new Element()
            {
                Name = "root",
                Content = new Element()
                {
                    Name = "element1",
                    Content = new Element()
                        {
                            Name = "element2",
                        }
                }
            };

            var rightObject = new Element()
            {
                Name = "root",
                Content = new Element()
                {
                    Name = "element1",
                    Content = new Element()
                    {
                        Name = "element2",
                    }
                }
            };

            ObjectGraphFactory factory = new PublicPropertyObjectGraphFactory();
            ObjectComparer comparer = new ObjectComparer(factory);
            bool match = comparer.Compare(leftObject, rightObject);

            Assert.True(match, "objects did not match");
        }

        [Fact]
        public void CompareIEnumerable()
        {
            Element leftObject = new Element()
            {
                Name = "root",
                Content = new List<string>
                {
                    "hello1",
                    "hello2",
                    "hello3"
                }
            };

            Element rightObject = new Element()
            {
                Name = "root",
                Content = new List<string>
                {
                    "hello1",
                    "hello2",
                    "hello3"
                }
            };

            ObjectGraphFactory factory = new PublicPropertyObjectGraphFactory();
            ObjectComparer comparer = new ObjectComparer(factory);
            bool match = comparer.Compare(leftObject, rightObject);

            Assert.True(match, "objects did not match");
        }

        [Fact(Timeout=1000)]
        public void CompareObjectWithLoopInFirstLevel()
        {
            Element leftObject = new Element()
            {
                Name = "RootElement",
            };
            // child points to parent 
            leftObject.Content = leftObject;

            Element rightObject = new Element()
            {
                Name = "RootElement",
            };
            // child points to parent 
            rightObject.Content = rightObject;

            ObjectGraphFactory factory = new PublicPropertyObjectGraphFactory();
            ObjectComparer comparer = new ObjectComparer(factory);
            bool match = comparer.Compare(leftObject, rightObject);

            Assert.True(match, "object with loop did not match");
        }

        [Fact(Timeout = 1000)]
        public void CompareObjectWithLoopInSecondLevel()
        {
            Element leftObject = new Element()
            {
                Name = "RootElement",
                Content = new Element()
                {
                    Name = "ChildElement"
                }
            };

            // child points to root 
            ((Element)leftObject.Content).Content = leftObject;

            Element rightObject = new Element()
            {
                Name = "RootElement",
                Content = new Element()
                {
                    Name = "ChildElement"
                }
            };

            // child points to parent 
            ((Element)rightObject.Content).Content = rightObject;

            ObjectGraphFactory factory = new PublicPropertyObjectGraphFactory();
            ObjectComparer comparer = new ObjectComparer(factory);
            bool match = comparer.Compare(leftObject, rightObject);

            Assert.True(match, "object with loop did not match");
        }

        [Fact(Timeout = 1000)]
        public void CompareObjectWithSharedNodes()
        {
            Element sharedLeftObject = new Element() { Name = "shared" };
            Element leftObject = new Element()
            {
                Name = "RootElement",
                Content = new List<Element>() 
                { 
                   sharedLeftObject,
                   sharedLeftObject,
                },
            };

            Element sharedRightObject = new Element() { Name = "shared" };
            Element rightObject = new Element()
            {
                Name = "RootElement",
                Content = new List<Element>() 
                { 
                   sharedRightObject,
                   sharedRightObject,
                },
            };

            ObjectGraphFactory factory = new PublicPropertyObjectGraphFactory();
            ObjectComparer comparer = new ObjectComparer(factory);
            bool match = comparer.Compare(leftObject, rightObject);

            Assert.True(match, "object with loop did not match");
        }

        [Fact]
        public void ComparePropertyThrows()
        {
            Element leftObject = new Element()
            {
                Content = new TypeWithPropertyThatThrows(),
            };

            Element rightObject = new Element()
            {
                Content = new TypeWithPropertyThatThrows(),
            };

            ObjectGraphFactory factory = new PublicPropertyObjectGraphFactory();
            ObjectComparer comparer = new ObjectComparer(factory);
            bool match = comparer.Compare(leftObject, rightObject);

            ObjectToConsole(leftObject, factory);

            Assert.True(match, "objects did not match");
        }

        #endregion

        #region Mismatch tests

        [Fact]
        public void CompareObjectValuesDoNotMatch()
        {
            var leftObject = new Element()
            {
                Name = "Content1"
            };

            var rightObject = new Element()
            {
                Name = "Content2"
            };

            ObjectGraphFactory factory = new PublicPropertyObjectGraphFactory();
            ObjectComparer comparer = new ObjectComparer(factory);
            IEnumerable<ObjectComparisonMismatch> mismatches;
            bool match = comparer.Compare(leftObject, rightObject, out mismatches);
            
            string[] expectedMismatches = new string[]
            {
                "ObjectValuesDoNotMatch:Left=RootObject.Name(Content1) Right=RootObject.Name(Content2)",
            };
            string[] actualMismatches = StringFromMismatches(mismatches);

            Assert.False(match);
            Assert.True(actualMismatches.Length == expectedMismatches.Length);
            for (int index = 0; index < expectedMismatches.Length; index++)
            {
                Assert.Equal(expectedMismatches[index], actualMismatches[index]);
            }
        }

        [Fact]
        public void CompareMissingNodes()
        {
            var leftObject = new Element()
            {
                Name = "Content1",
                Content = new Element()
                {
                    Name = "OnlyOnLeft",
                }
            };

            var rightObject = new Element()
            {
                Name = "Content1"
            };

            ObjectGraphFactory factory = new PublicPropertyObjectGraphFactory();
            ObjectComparer comparer = new ObjectComparer(factory);
            IEnumerable<ObjectComparisonMismatch> mismatches;
            bool match = comparer.Compare(leftObject, rightObject, out mismatches);

            string[] expectedMismatches = new string[]
            {
                "ObjectValuesDoNotMatch:Left=RootObject.Content(Microsoft.Test.AcceptanceTests.ObjectComparison.Element) Right=RootObject.Content(Null)",
                "MissingRightNode:Left=RootObject.Content.Content(Null) Right=Null(Null)",
                "MissingRightNode:Left=RootObject.Content.Name(OnlyOnLeft) Right=Null(Null)",
            };

            string[] actualMismatches = StringFromMismatches(mismatches);

            Assert.False(match);
            Assert.True(actualMismatches.Length == expectedMismatches.Length);
            for (int index = 0; index < expectedMismatches.Length; index++)
            {
                Assert.Equal(expectedMismatches[index], actualMismatches[index]);
            }
        }

        [Fact]
        public void CompareTypesDoNotMatch()
        {
            var leftObject = new Element()
            {
                Content = 32,
            };

            var rightObject = new Element()
            {
                Content = "stringvalue",
            };

            ObjectGraphFactory factory = new PublicPropertyObjectGraphFactory();
            ObjectComparer comparer = new ObjectComparer(factory);
            IEnumerable<ObjectComparisonMismatch> mismatches;
            bool match = comparer.Compare(leftObject, rightObject, out mismatches);

            string[] expectedMismatches = new string[]
            {
                "ObjectTypesDoNotMatch:Left=RootObject.Content(32) Right=RootObject.Content(stringvalue)",
            };

            string[] actualMismatches = StringFromMismatches(mismatches);

            Assert.False(match);
            Assert.True(actualMismatches.Length == expectedMismatches.Length);
            for (int index = 0; index < expectedMismatches.Length; index++)
            {
                Assert.Equal(expectedMismatches[index], actualMismatches[index]);
            }
        }

        [Fact]
        public void CompareIncorrectChildCount()
        {
            var leftObject = new Element()
            {
                Content = new string[]
                {
                    "String1",
                    "String2",
                },
            };

            var rightObject = new Element()
            {
                Content = new string[]
                {
                    "String1",
                },
            };

            ObjectGraphFactory factory = new PublicPropertyObjectGraphFactory();
            ObjectComparer comparer = new ObjectComparer(factory);
            IEnumerable<ObjectComparisonMismatch> mismatches;
            bool match = comparer.Compare(leftObject, rightObject, out mismatches);

            string[] expectedMismatches = new string[]
            {
               "RightNodeHasFewerChildren:Left=RootObject.Content(System.String[]) Right=RootObject.Content(System.String[])",
               "MissingRightNode:Left=RootObject.Content.IEnumerable1(String2) Right=Null(Null)",
               "ObjectValuesDoNotMatch:Left=RootObject.Content.LongLength(2) Right=RootObject.Content.LongLength(1)",
               "ObjectValuesDoNotMatch:Left=RootObject.Content.Length(2) Right=RootObject.Content.Length(1)",
            };

            string[] actualMismatches = StringFromMismatches(mismatches);

            Assert.False(match);
            Assert.True(actualMismatches.Length == expectedMismatches.Length);
            for (int index = 0; index < expectedMismatches.Length; index++)
            {
                Assert.Equal(expectedMismatches[index], actualMismatches[index]);
            }
        }

        #endregion

        #region Private members

        private static string StringFromGraph(GraphNode graph)
        {
            StringBuilder stringBuilder = new StringBuilder();
            IEnumerable<GraphNode> nodes = graph.GetNodesInDepthFirstOrder();
            foreach (GraphNode node in nodes)
            {
                string type = "Null";
                if (node.ObjectValue != null)
                {
                    type = node.ObjectType.FullName;
                }

                stringBuilder.AppendLine("".PadLeft(node.Depth*4) + node.Name + "Value = '" + node.ObjectValue + "'" + " Type=" + type);
            }
            return stringBuilder.ToString();
        }

        private static void ObjectToConsole(object value, ObjectGraphFactory factory)
        {
            GraphNode root = factory.CreateObjectGraph(value);
            Console.WriteLine(StringFromGraph(root));
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

        #endregion
    }
}