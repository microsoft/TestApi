// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.FaultInjection
{
    /// <summary>
    /// The interface defining a "fault".
    /// </summary>
    public interface IFault
    {
        /// <summary>
        /// Make decision on how a "fault" should occur at this call.
        /// </summary>
        /// <param name="context">Runtime context information for this call.</param>
        /// <param name="exceptionValue">Ouput paramter for exception user want the faulted method to throw.</param>
        /// <param name="returnValue">Output paramter for return value user want the faulted method to return</param>
        /// <remarks>
        /// Please note that parameter <paramref name="exceptionValue"/> has higher priority. Parameter <paramref name="returnValue"/>
        /// will only be checked when <paramref name="exceptionValue"/> returns null.
        /// </remarks>
        void Retrieve(IRuntimeContext context, out Exception exceptionValue, out object returnValue);
    }

    /// <summary>
    /// Wrapper for all built-in faults, all built-in faults need to be accessed from this class.
    /// (e.g. a ReturnValueFault can fault a method into returning a user-specified value)
    /// </summary>
    public static class BuiltInFaults
    {   
        /// <summary>
        /// Built-in fault which always return null when triggered, call this method also when the method has a void return type.
        /// </summary>
        public static IFault ReturnNullFault
        {
            get { return new ReturnNullFault(); }
        }
        /// <summary>
        /// Built-in fault which return the specified object when triggered.
        /// </summary>
        /// <param name="returnValue">The object that user put in. Faulted method will eventually return this object when fault condition triggered.</param>
        
        public static IFault ReturnValueFault(object returnValue)
        {
            return new ReturnValueFault(returnValue);
        }
        /// <summary>
        /// Built-in fault which return an object constructed according to the specified expression when triggered.
        /// </summary>
        /// <param name="returnValueExpression">Examples of the string format would be:
        /// (int)3, (double)6.6, (bool)true, ‘Hello World’ which means "Hello World",
        /// System.Exception(‘This is a fault’).
        /// For more information about the format, please refer to 'User Guide.doc', 'Expression Format in XML' section.</param>
        
        public static IFault ReturnValueRuntimeFault(string returnValueExpression)
        {
            return new ReturnValueRuntimeFault(returnValueExpression);
        }
        /// <summary>
        /// Built-in fault which throw the specified exception object when triggered.
        /// </summary>
        /// <param name="exceptionValue">This Exception object is constructed at the process that calls Fault Injection API.
        /// It needs to be serializable (All System Excetpions are serializable already).</param>    
        public static IFault ThrowExceptionFault(Exception exceptionValue)
        {
            return new ThrowExceptionFault(exceptionValue);
        }
        /// <summary>
        /// Built-in fault which throw an exception object constructed according to the specified expression when triggered.
        /// </summary>
        /// <param name="exceptionExpression">Examples of the string format would be:
        /// System.Exception(‘This is a fault’),
        /// CustomizedNameSpace.CustomizedException(‘Error Message’, (int)3, System.Exception(‘innerException’)).
        /// For more information about the format, please refer to 'User Guide.doc', 'Expression Format in XML' section</param>
        public static IFault ThrowExceptionRuntimeFault(string exceptionExpression)
        {
            return new ThrowExceptionRuntimeFault(exceptionExpression);
        }
    }

    // Attribute class used by FaultInjector to distinguish normal fault and runtime fault
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class RuntimeFaultAttribute : Attribute { }


    [Serializable()]
    internal sealed class ReturnNullFault : IFault
    {
        public void Retrieve(IRuntimeContext rtx, out Exception exceptionValue, out object returnValue)
        {
            exceptionValue = null;
            returnValue = null;
        }
    }

    [Serializable()]
    internal sealed class ReturnValueFault : IFault
    {
        public ReturnValueFault(object returnValue)
        {
            this.returnValue = returnValue;
        }
        public void Retrieve(IRuntimeContext rtx, out Exception exceptionValue, out object returnValue)
        {
            exceptionValue = null;
            returnValue = this.returnValue;
        }
        private readonly object returnValue;
    }

    [Serializable(), RuntimeFault]
    internal sealed class ReturnValueRuntimeFault : IFault
    {
        public ReturnValueRuntimeFault(string returnValueExpression)
        {
            this.returnValueExpression = returnValueExpression;
        }
        public void Retrieve(IRuntimeContext rtx, out Exception exceptionValue, out object returnValue)
        {
            exceptionValue = null;
            returnValue = Expression.GeneralExpression(returnValueExpression);
        }
        private readonly string returnValueExpression;
    }

    [Serializable()]
    internal sealed class ThrowExceptionFault : IFault
    {
        public ThrowExceptionFault(Exception exceptionValue)
        {
            if (exceptionValue == null)
            {
                throw new FaultInjectionException(ApiErrorMessages.ExceptionNull);
            }
            this.exceptionValue = exceptionValue;
        }
        public void Retrieve(IRuntimeContext rtx, out Exception exceptionValue, out object returnValue)
        {
            returnValue = null;
            exceptionValue = this.exceptionValue;
        }
        private readonly Exception exceptionValue;
    }

    [Serializable(), RuntimeFault]
    internal sealed class ThrowExceptionRuntimeFault : IFault
    {
        public ThrowExceptionRuntimeFault(string exceptionExpression)
        {
            this.exceptionExpression = exceptionExpression;
        }
        public void Retrieve(IRuntimeContext rtx, out Exception exceptionValue, out object returnValue)
        {
            returnValue = null;
            exceptionValue = (Exception)Expression.GeneralExpression(exceptionExpression);
        }
        private readonly string exceptionExpression;
    }
}
