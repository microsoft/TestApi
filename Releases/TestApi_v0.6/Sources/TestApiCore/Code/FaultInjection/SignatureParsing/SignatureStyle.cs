// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Test.FaultInjection.SignatureParsing
{
    internal enum SignatureStyle
    {
        // Formal signature style used to identify a method
        // Full quailfied type name, "<>" for generics, "&" for "ref" and "out"
        // An example: NSName1.NSName2.OuterClass<T,E>.InnerClass<P,Q>.foo<R,S>(System.String[], int&)
        Formal = 0,
        // Signature style used for our COM engine to filter method
        // Full quailfied type name, no method parameters, only records number of generic parameters for classes
        // An example: NSName1.NSName2.OuterClass`2.InnerClass`2.foo
        Com = 1
    }
}