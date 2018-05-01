// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.FaultInjection
{

    /// <summary>
    /// The interface defining a "fault condition", it defines WHEN the fault will be triggered on this method, and WHEN NOT.
    /// When the condition is not triggered, the faulted method will execute its original code.
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// Make decision on whether a "fault" should be triggered at this call.
        /// </summary>
        /// <param name="context">Runtime context information for this call and the faulted method.</param>
        /// <returns>If a 'fault' should be triggered, return true. Otherwise, return false</returns>
        bool Trigger(IRuntimeContext context);
    }

    /// <summary>
    /// Wrapper for all built in conditions. All built-in conditions need to be accessed from this class.
    /// </summary>
    public static class BuiltInConditions
    {
        /// <summary>
        /// Built-in condition which triggers fault all the time.
        /// </summary>
        public static ICondition TriggerAllCalls
        {
            get { return new TriggerAllCalls(); }
        }


        /// <summary>
        /// Built-in condition which triggers fault if injected method is called by a specified method.</summary>
        /// <param name="caller">Example of the string format would be:
        /// System.Console.WriteLine(string),
        /// Namespace&lt;T&gt;.OuterClass&lt;E&gt;.InnerClass&lt;F,G&gt;.MethodName&lt;H&gt;(T, E, F, H, List&lt;H&gt;)
        /// For more information about the format, please refer to 'User Guide.doc', 'Method Signature Format' section</param>
        public static ICondition TriggerIfCalledBy(string caller)
        {
            return new TriggerIfCalledBy(caller);
        }


        /// <summary>
        /// Built-in condition which triggers fault if current call stack contains a specified method.
        /// </summary>
        /// <param name="method">Example of the string format would be:
        /// System.Console.WriteLine(string),
        /// Namespace&lt;T&gt;.OuterClass&lt;E&gt;.InnerClass&lt;F,G&gt;.MethodName&lt;H&gt;(T, E, F, H, List&lt;H&gt;)
        /// For more information about the format, please refer to 'User Guide.doc', 'Method Signature Format' section</param>
        public static ICondition TriggerIfStackContains(string method)
        {
            return new TriggerIfStackContains(method);
        }


        /// <summary>
        /// Built-in condition which triggers fault for every N times injected method is called.
        /// </summary>
        /// <param name="n">The value should be a positive number, otherwise a System.Argument will be thrown</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704")]
        public static ICondition TriggerEveryNthCall(int n)
        {
            return new TriggerEveryNthCall(n);
        }


        /// <summary>
        /// Built-in condition which triggers fault if injected method is called N times.
        /// </summary>
        /// <param name="n">The value should be a positive number, otherwise a System.Argument will be thrown</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704")]
        public static ICondition TriggerOnNthCall(int n)
        {
            return new TriggerOnNthCall(n);
        }


        /// <summary>
        /// Built-in condition which triggers fault only once after this condition takes effect.
        /// </summary>
        public static ICondition TriggerOnlyOnce
        {
            get { return new TriggerOnlyOnce(); }
        }


        /// <summary>
        /// Built-in condition which never triggers fault. This condition can be used to TURN OFF a fault rule.
        /// </summary>
        public static ICondition NeverTrigger
        {
            get { return new NeverTrigger(); }
        }

        /// <summary>
        /// Built-in condition which triggers faults if it is called by the 'caller' for 'n' times.
        /// </summary>
        /// <param name="n">The value should be a positive number, otherwise a System.Argument will be thrown</param>
        /// <param name="caller">Example of the string format would be:
        /// System.Console.WriteLine(string),
        /// Namespace&lt;T&gt;.OuterClass&lt;E&gt;.InnerClass&lt;F,G&gt;.MethodName&lt;H&gt;(T, E, F, H, List&lt;H&gt;)
        /// For more information about the format, please refer to 'User Guide.doc', 'Method Signature Format' section</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704")]
        public static ICondition TriggerOnNthCallBy(int n, string caller)
        {
            return new TriggerOnNthCallBy(n, caller);
        }

    }

    [Serializable()]
    internal sealed class TriggerAllCalls : ICondition
    {
        public bool Trigger(IRuntimeContext context)
        {
            return true;
        }
    }

    [Serializable()]
    internal sealed class TriggerIfCalledBy : ICondition
    {
        public TriggerIfCalledBy(String aTargetCaller)
        {
            targetCaller = SignatureHelper.ConvertSignature(aTargetCaller);
        }

        public bool Trigger(IRuntimeContext context)
        {
            if (context.Caller == targetCaller)
            {
                return true;
            }
            return false;
        }
        private String targetCaller;
    }

    [Serializable()]
    internal sealed class TriggerIfStackContains : ICondition
    {
        public TriggerIfStackContains(String aTargetFunction)
        {
            targetFunction = SignatureHelper.ConvertSignature(aTargetFunction);
        }

        public bool Trigger(IRuntimeContext context)
        {
            for (int i = 0; i < context.CallStack.FrameCount; ++i)
            {
                if (context.CallStack[i] == null)
                {
                    return false;
                }
                else if (context.CallStack[i] == targetFunction)
                {
                    return true;
                }
            }
            return false;
        }
        private String targetFunction;
    }

    [Serializable()]
    internal sealed class TriggerEveryNthCall : ICondition
    {
        public TriggerEveryNthCall(int nth)
        {
            if (nth <= 0)
            {
                throw new ArgumentException("The parameter of TriggerEveryNthCall(int) should be a positive number");
            }
            this.n = nth;
        }

        public bool Trigger(IRuntimeContext context)
        {
            if (context.CalledTimes % n == 0)
            {
                return true;
            }
            return false;
        }
        private int n;
    }

    [Serializable()]
    internal sealed class TriggerOnNthCall : ICondition
    {
        public TriggerOnNthCall(int nth)
        {
            if (nth <= 0)
            {
                throw new ArgumentException("The parameter of TriggerOnNthCall(int) should be a postive number");
            }
            this.n = nth;
        }

        public bool Trigger(IRuntimeContext context)
        {
            if (context.CalledTimes == n)
            {
                return true;
            }
            return false;
        }
        private int n;
    }

    [Serializable()]
    internal sealed class TriggerOnlyOnce : ICondition
    {
        public bool Trigger(IRuntimeContext context)
        {
            if (triggered == false)
            {
                triggered = true;
                return true;
            }
            return false;
        }
        private bool triggered = false;
    }

    [Serializable()]
    internal sealed class NeverTrigger : ICondition
    {
        public bool Trigger(IRuntimeContext context)
        {
            return false;
        }
    }

    [Serializable()]
    internal sealed class TriggerOnNthCallBy : ICondition
    {
        public TriggerOnNthCallBy(int nth, String aTargetCaller)
        {
            calledTimes = 0;
            n = nth;
            if (nth <= 0)
            {
                throw new ArgumentException("The first parameter of TriggerOnNthCallBy(int, string) should be a postive number");
            }
            targetCaller = SignatureHelper.ConvertSignature(aTargetCaller);
        }

        public bool Trigger(IRuntimeContext context)
        {
            if (context.Caller == targetCaller)
            {
                if ((++calledTimes) == n)
                {
                    return true;
                }
            }
            return false;
        }
        private int calledTimes;
        private int n;
        private String targetCaller;
    }

}
