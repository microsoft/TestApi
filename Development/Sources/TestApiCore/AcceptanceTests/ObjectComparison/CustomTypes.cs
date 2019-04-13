// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Test.AcceptanceTests.ObjectComparison
{
    public class BasicTypes
    {
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "short")]
        public short ShortPrimitive { get; set;}

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int")]
        public int IntPrimitive { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "long")]
        public long LongPrimitive { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte")]
        public byte BytePrimitive { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "float")]
        public float FloatPrimitive { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "double")]
        public double DoublePrimitive { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bool")]
        public bool BoolPrimitive { get; set; }

        public string StringPrimitive { get; set; }

        public char CharValue { get; set; }

        public TimeSpan TimeSpanValue { get; set; }

        public BasicTypes Clone()
        {
            BasicTypes clone = new BasicTypes()
            {
                BoolPrimitive = this.BoolPrimitive,
                BytePrimitive = this.BytePrimitive,
                CharValue = this.CharValue,
                DoublePrimitive = this.DoublePrimitive,
                FloatPrimitive = this.FloatPrimitive,
                IntPrimitive = this.IntPrimitive,
                LongPrimitive = this.LongPrimitive,
                ShortPrimitive = this.ShortPrimitive,
                StringPrimitive = this.StringPrimitive,
                TimeSpanValue = this.TimeSpanValue,
            };

            return clone;
        }
    }

    public class Element
    {
        public string Name { get; set; }
        public object Content { get; set; }
    }

    public class TypeWithPropertyThatThrows
    {
        public int ThrowProperty
        {
            get
            {
                throw new InvalidOperationException();
            }
        }
    }
}
