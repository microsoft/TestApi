// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.Test.FaultInjection;

namespace Microsoft.Test.AcceptanceTests.FaultInjection
{
    /// <summary>
    /// Data class for Fault Injection tests
    /// </summary>
    public static class FaultInjectionTestData
    {
        static FaultInjectionTestData()
        {
            FaultRules = ConstructFaultRules();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819")] //Convert to method
        public static FaultRule[] FaultRules
        {
            get;
            private set;
        }

        private static FaultRule[] ConstructFaultRules()
        {
            FaultRule[] rules = new FaultRule[16];
            rules[0] = new FaultRule("static Microsoft.Test.AcceptanceTests.FaultInjection.ConstructorTestOuterClass.InnerClass.InnerClass()");
            rules[0].Condition = BuiltInConditions.TriggerOnEveryCall;
            rules[0].Fault = BuiltInFaults.ReturnValueFault(3);   //Take no effect

            rules[1] = new FaultRule("Microsoft.Test.AcceptanceTests.FaultInjection.GenericSignatureTests.NestedGenericClass<T, S>.DoublyNestedClass.TriplyNestedGenericClass<R>.GenericTestMethod<P, Q>(System.Collections.Generic.List<System.Collections.Generic.List<System.Collections.Generic.List<System.Int32>>>,Microsoft.Test.AcceptanceTests.GenericSignatureTests.EmptyClass,System.Collections.Generic.Dictionary<P,System.String>,Q,Microsoft.Test.AcceptanceTests.GenericSignatureTests.NestedGenericClass<T, S>.DoublyNestedClass.TriplyNestedGenericClass<Microsoft.Test.AcceptanceTests.GenericSignatureTests.NestedGenericClass<T, S>.DoublyNestedClass.TriplyNestedGenericClass<E>>)");
            rules[1].Condition = BuiltInConditions.TriggerOnEveryCall;
            rules[1].Fault = BuiltInFaults.ReturnValueFault(System.Boolean.Parse("false"));   //Take no effect

            rules[2] = new FaultRule("Microsoft.Test.AcceptanceTests.FaultInjection.NestedClassTests.NestedClass.NestedClassTest()");
            rules[2].Condition = BuiltInConditions.TriggerOnEveryCall;
            rules[2].Fault = BuiltInFaults.ReturnValueFault(System.Boolean.Parse("false"));   //Take no effect

            rules[3] = new FaultRule("Microsoft.Test.AcceptanceTests.FaultInjection.NestedClassTests.NestedClass.DoublyNestedClass.DoublyNestedClassTest()");
            rules[3].Condition = BuiltInConditions.TriggerOnEveryCall;
            rules[3].Fault = BuiltInFaults.ReturnValueFault(System.Boolean.Parse("false"));   //Take no effect

            rules[4] = new FaultRule("Microsoft.Test.AcceptanceTests.FaultInjection.NonGenericSignatureTests.TestMethod(out System.Int32[][],ref System.Object,Microsoft.Test.AcceptanceTests.NonGenericSignatureTests.TestEnum[,,], Microsoft.Test.AcceptanceTests.NonGenericSignatureTests.NestedClass[][], params int[])");
            rules[4].Condition = BuiltInConditions.TriggerOnEveryCall;
            rules[4].Fault = BuiltInFaults.ReturnValueFault(System.Boolean.Parse("false"));   //Take no effect

            rules[5] = new FaultRule("Microsoft.Test.AcceptanceTests.FaultInjection.ReturnTypeErrorTests.NullValueTypeInt()");
            rules[5].Condition = BuiltInConditions.TriggerOnEveryCall;
            rules[5].Fault = BuiltInFaults.ReturnValueFault(null);   //Take no effect

            rules[6] = new FaultRule("Microsoft.Test.AcceptanceTests.FaultInjection.ReturnTypeErrorTests.NullValueTypeBool()");
            rules[6].Condition = BuiltInConditions.TriggerOnEveryCall;
            rules[6].Fault = BuiltInFaults.ReturnValueFault(null);   //Take no effect

            rules[7] = new FaultRule("Microsoft.Test.AcceptanceTests.FaultInjection.ReturnTypeErrorTests.ReturnTypeMismatchIntBool()");
            rules[7].Condition = BuiltInConditions.TriggerOnEveryCall;
            rules[7].Fault = BuiltInFaults.ReturnValueFault(true);   //Take no effect

            rules[8] = new FaultRule("Microsoft.Test.AcceptanceTests.FaultInjection.ReturnTypeErrorTests.ReturnTypeMismatchBoolInt()");
            rules[8].Condition = BuiltInConditions.TriggerOnEveryCall;
            rules[8].Fault = BuiltInFaults.ReturnValueFault(3);   //Take no effect

            rules[9] = new FaultRule("Microsoft.Test.AcceptanceTests.FaultInjection.ReturnValueTests.ReturnNullTest()");
            rules[9].Condition = BuiltInConditions.TriggerOnEveryCall;
            rules[9].Fault = BuiltInFaults.ReturnFault();   //Take no effect

            rules[10] = new FaultRule("Microsoft.Test.AcceptanceTests.FaultInjection.ReturnValueTests.ReturnIntTargetMethod(System.String,System.Int32)");
            rules[10].Condition = BuiltInConditions.TriggerOnEveryCall;
            rules[10].Fault = BuiltInFaults.ReturnValueFault(232);

            rules[11] = new FaultRule("Microsoft.Test.AcceptanceTests.FaultInjection.ReturnValueTests.ReturnBoolTargetMethod(System.String,System.Int32)");
            rules[11].Condition = BuiltInConditions.TriggerOnEveryCall;
            rules[11].Fault = BuiltInFaults.ReturnValueFault(false);

            rules[12] = new FaultRule("Microsoft.Test.AcceptanceTests.FaultInjection.ThrowExceptionTests.ThrowBuiltInExceptionTest()");
            rules[12].Condition = BuiltInConditions.TriggerOnEveryCall;
            rules[12].Fault = BuiltInFaults.ThrowExceptionFault(new ApplicationException());

            rules[13] = new FaultRule("Microsoft.Test.AcceptanceTests.FaultInjection.ThrowExceptionTests.ThrowCustomExceptionTest()");
            rules[13].Condition = BuiltInConditions.TriggerOnEveryCall;
            rules[13].Fault = BuiltInFaults.ThrowExceptionFault(new CustomizedException());

            rules[14] = new FaultRule("Microsoft.Test.AcceptanceTests.FaultInjection.ConstructorTestOuterClass.InnerClass.InnerClass()");
            rules[14].Condition = BuiltInConditions.TriggerOnEveryCall;
            rules[14].Fault = BuiltInFaults.ReturnValueFault(3);   //Take no effect

            rules[15] = new FaultRule("Microsoft.Test.AcceptanceTests.FaultInjection.BuiltInTriggerTests.TesteeTriggerOnNthCallBy()");
            rules[15].Condition = BuiltInConditions.TriggerOnNthCallBy(10, "Microsoft.Test.AcceptanceTests.FaultInjection.BuiltInTriggerTests.TesterTriggerOnNthCallBy()");
            rules[15].Fault = BuiltInFaults.ReturnValueFault(true);   //Take no effect

            return rules;
        }
    }
}
