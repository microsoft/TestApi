// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Test.FaultInjection;
using Xunit;

namespace Microsoft.Test.AcceptanceTests
{
    /// <summary>
    /// Tests which verify the built in conditions
    /// </summary>    
    public class SignatureTests
    {
        #region Test Helpers

        // We use an object of this class to verify that the trap succeeded
        private class TrapSuccess : Exception
        {
        }

        private static void AssertTrapSuccess(bool ret, Exception e, object o)
        {
            Assert.True(ret);
            Assert.NotNull(e);
            Assert.Equal(typeof(TrapSuccess), e.GetType());
            Assert.Null(o);
        }

        private FaultScope CreateFaultScope(MethodBase m)
        {
            FaultRule rule = new FaultRule(m, BuiltInConditions.TriggerOnEveryCall, BuiltInFaults.ThrowExceptionFault(new TrapSuccess()));
            Console.WriteLine(rule.MethodSignature);
            return new FaultScope(rule);
        }

        private MethodBase GetTargetMethodBase(Expression body)
        {
            switch (body.NodeType)
            {
                case ExpressionType.New:
                    return ((NewExpression)body).Constructor;
                case ExpressionType.Call:
                    return ((MethodCallExpression)body).Method;
                default:
                    throw new InvalidOperationException(string.Format("Test bug: invalid expression type {0}", body.NodeType));
            }
        }

        private void ExecuteMethodInfoScenario(Expression<Action> lambdaExpression)
        {
            using (FaultScope fs = CreateFaultScope(GetTargetMethodBase(lambdaExpression.Body)))
            {
                lambdaExpression.Compile()();
            }
        }

        private void ExecuteMethodInfoScenario<T>(Expression<Action<T>> lambdaExpression)
        {
            using (FaultScope fs = CreateFaultScope(GetTargetMethodBase(lambdaExpression.Body)))
            {
                lambdaExpression.Compile()(default(T));
            }
        }

        #endregion

        #region No params

        [Fact]
        public void TestMethodNoParams()
        {
            ExecuteMethodInfoScenario(() => TargetMethodNoParams());
        }

        private void TargetMethodNoParams()
        {
            Exception e;
            object o;
            bool ret = FaultDispatcher.Trap(out e, out o);
            AssertTrapSuccess(ret, e, o);
        }

        #endregion

        #region One param

        [Fact]
        public void TestMethodOneParam()
        {
            ExecuteMethodInfoScenario(() => TargetMethodOneParam(0));
        }

        private void TargetMethodOneParam(int i)
        {
            Exception e;
            object o;
            bool ret = FaultDispatcher.Trap(out e, out o);
            AssertTrapSuccess(ret, e, o);
        }
        
        #endregion

        #region Out param

        [Fact]
        public void TestMethodOutParam()
        {
            ExecuteMethodInfoScenario<int>(i => TargetMethodOutParam(out i) );
        }

        private void TargetMethodOutParam(out int i)
        {
            i = 0;
            Exception e;
            object o;
            bool ret = FaultDispatcher.Trap(out e, out o);
            AssertTrapSuccess(ret, e, o);
        }

        #endregion

        #region Ref param

        [Fact]
        public void TestMethodRefParam()
        {
            ExecuteMethodInfoScenario<int>(i => TargetMethodRefParam(ref i));
        }

        private void TargetMethodRefParam(ref int i)
        {
            Exception e;
            object o;
            bool ret = FaultDispatcher.Trap(out e, out o);
            AssertTrapSuccess(ret, e, o);
        }

        #endregion

        #region One generic param

        [Fact]
        public void TestMethodOneGenericParam()
        {
            ExecuteMethodInfoScenario(() => TargetMethodOneGenericParam<int>(0));
        }

        [Fact]
        public void TestGenericMethodIsGeneric()
        {
            // Note that 'int' and 'string' are different types. Fault injection looks at the generic types.
            using (FaultScope fs = CreateFaultScope(((Action<int>)TargetMethodOneGenericParam<int>).Method))
            {
                TargetMethodOneGenericParam<string>("");
            }
        }

        private void TargetMethodOneGenericParam<T>(T t)
        {
            Exception e;
            object o;
            bool ret = FaultDispatcher.Trap(out e, out o);
            AssertTrapSuccess(ret, e, o);
        }

        #endregion

        #region Two generic params

        [Fact]
        public void TestMethodTwoGenericParams()
        {
            ExecuteMethodInfoScenario(() => TargetMethodTwoGenericParams<int, string>(0, ""));
        }

        private void TargetMethodTwoGenericParams<T_first, T_second>(T_first t1, T_second t2)
        {
            Exception e;
            object o;
            bool ret = FaultDispatcher.Trap(out e, out o);
            AssertTrapSuccess(ret, e, o);
        }

        #endregion

        #region Array param

        [Fact]
        public void TestMethodArrayParam()
        {
            ExecuteMethodInfoScenario(() => TargetMethodArrayParam(new int[] {}));
        }

        private void TargetMethodArrayParam(int[] i)
        {
            Exception e;
            object o;
            bool ret = FaultDispatcher.Trap(out e, out o);
            AssertTrapSuccess(ret, e, o);
        }

        #endregion

        #region Square array

        [Fact]
        public void TestMethodSquareArrayParam()
        {
            int[,] i = new int[,] { {} };
            ExecuteMethodInfoScenario(() => TargetMethodSquareArrayParam(i));
        }

        private void TargetMethodSquareArrayParam(int[,] i)
        {
            Exception e;
            object o;
            bool ret = FaultDispatcher.Trap(out e, out o);
            AssertTrapSuccess(ret, e, o);
        }

        #endregion

        #region Cube array

        [Fact]
        public void TestMethodCubeArrayParam ()
        {
            int [,,] i = new int[,,] { { {} } };
            ExecuteMethodInfoScenario(() => TargetMethodCubeArrayParam(i));
        }

        private void TargetMethodCubeArrayParam(int[, ,] i)
        {
            Exception e;
            object o;
            bool ret = FaultDispatcher.Trap(out e, out o);
            AssertTrapSuccess(ret, e, o);
        }

        #endregion

        #region Jagged array

        [Fact]
        public void TestMethodJaggedArrayParam()
        {
            int[][] i = new int[][] { new int[] { } };
            ExecuteMethodInfoScenario(() => TargetMethodJaggedArrayParam(i));
        }

        private void TargetMethodJaggedArrayParam(int[][] i)
        {
            Exception e;
            object o;
            bool ret = FaultDispatcher.Trap(out e, out o);
            AssertTrapSuccess(ret, e, o);
        }

        #endregion

        #region Params

        [Fact]
        public void TestMethodParamsParam()
        {
            ExecuteMethodInfoScenario(() => TargetMethodParamsParam(1, 2, 3));
        }

        private void TargetMethodParamsParam(params int[] p)
        {
            Exception e;
            object o;
            bool ret = FaultDispatcher.Trap(out e, out o);
            AssertTrapSuccess(ret, e, o);
        }

        #endregion

        #region Static

        [Fact]
        public void TestMethodStatic()
        {
            ExecuteMethodInfoScenario(() => TargetMethodStatic());
        }

        private static void TargetMethodStatic()
        {
            Exception e;
            object o;
            bool ret = FaultDispatcher.Trap(out e, out o);
            AssertTrapSuccess(ret, e, o);
        }

        #endregion

        #region Nested non-generic class constructor/method

        [Fact]
        public void TestStaticConstructor()
        {
            // It isn't possible to do fault injection on a static constructor 
            // using the FaultRule(MethodBase, ...) constructor.
            //
            // You can get the static constructor's MethodBase like this:
            //      typeof(TargetNestedClass).TypeInitializer;
            //
            // However, referencing the type will force the static constructor 
            // to be executed. It is only ever executed once, so by the time 
            // the FaultRule/FaultScope have been made, it is too late to then
            // do a fault injection on it.
        }

        [Fact]
        public void TestConstructor()
        {
            ExecuteMethodInfoScenario(() => new TargetNestedClass(true));
        }

        [Fact]
        public void TestNestedClassMethod()
        {
            var o = new TargetNestedClass(false);
            ExecuteMethodInfoScenario(() => TargetMethodStatic());
        }

        private class TargetNestedClass
        {
            public TargetNestedClass(bool doTrap)
            {
                if (doTrap)
                {
                    Exception e;
                    object o;
                    bool ret = FaultDispatcher.Trap(out e, out o);
                    AssertTrapSuccess(ret, e, o);
                }
            }

            public void TargetMethod()
            {
                Exception e;
                object o;
                bool ret = FaultDispatcher.Trap(out e, out o);
                AssertTrapSuccess(ret, e, o);
            }
        }

        #endregion

        #region Generic class constructor/method

        [Fact]
        public void TestConstructorGeneric()
        {
            // This is currently unsupported. If it is fixed, then remove the Assert.Throws and just call the constructor instead.
            Assert.Throws<FaultInjectionException>(() => ExecuteMethodInfoScenario(() => new TargetNestedClassGeneric<int>(true)));
        }

        [Fact]
        public void TestMethodGenericClass()
        {
            var o = new TargetNestedClassGeneric<int>(false);
            ExecuteMethodInfoScenario(() => o.TargetMethod());
        }

        [Fact]
        public void TestMethodGenericClass2()
        {
            var o = new TargetNestedClassGeneric<TargetNestedClassGeneric<int>>(false);
            ExecuteMethodInfoScenario(() => o.TargetMethod());
        }

        [Fact]
        public void TestMethodGenericClass3()
        {
            ExecuteMethodInfoScenario(() => TargetMethodOneGenericParam<TargetNestedClassGeneric<int>>(null));
        }

        [Fact]
        public void TestGenericClassMethodIsGeneric()
        {
            // Note that 'int' and 'string' are different types. Fault injection looks at the generic types.
            using (FaultScope fs = CreateFaultScope(typeof(TargetNestedClassGeneric<int>).GetMethod("TargetMethod")))
            {
                new TargetNestedClassGeneric<string>(false).TargetMethod();
            }
        }

        [Fact]
        public void TestGenericClassConstructorIsGeneric()
        {
            // This is currently unsupported. If it is fixed, then remove the Assert.Throws.
            Assert.Throws<FaultInjectionException>(() =>
            {
                // Note that 'int' and 'string' are different types. Fault injection looks at the generic types.
                using (FaultScope fs = CreateFaultScope(typeof(TargetNestedClassGeneric<int>).GetConstructor(new Type[] { typeof(bool) })))
                {
                    new TargetNestedClassGeneric<string>(true);
                }

            });
        }

        private class TargetNestedClassGeneric<T>
        {
            public TargetNestedClassGeneric(bool doTrap)
            {
                if (doTrap)
                {
                    Exception e;
                    object o;
                    bool ret = FaultDispatcher.Trap(out e, out o);
                    AssertTrapSuccess(ret, e, o);
                }
            }

            public void TargetMethod()
            {
                Exception e;
                object o;
                bool ret = FaultDispatcher.Trap(out e, out o);
                AssertTrapSuccess(ret, e, o);
            }
        }

        #endregion
    }
}
