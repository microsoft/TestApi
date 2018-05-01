// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Microsoft.Test.FaultInjection.Constants;

namespace Microsoft.Test.FaultInjection.SignatureParsing
{
    internal static class MethodSignatureTranslator
    {
        #region Public Members

        public static String GetGenericParaString(Type genericType, int genericIndex)
        {
            String strGenericParas = null;
            Type[] allGenericTypes = genericType.GetGenericArguments();


            int genericNumber = GetGenericNumber(genericType);

            if (allGenericTypes != null && allGenericTypes.Length > 0 && genericNumber > 0)
            {
                Type[] currentGenericTypes = new Type[genericNumber];

                strGenericParas = "<";
                for (int i = 0; i < genericNumber; ++i)
                {
                    currentGenericTypes[i] = allGenericTypes[genericIndex + i];
                }
                foreach (Type genericPara in currentGenericTypes)
                {
                    strGenericParas = strGenericParas.Insert(strGenericParas.Length, GetTypeString(genericPara) + ",");
                }
                if (currentGenericTypes.Length > 0)
                {
                    strGenericParas = strGenericParas.Remove(strGenericParas.Length - 1);
                }
                strGenericParas = strGenericParas.Insert(strGenericParas.Length, ">");
            }
            return strGenericParas;
        }

        public static int GetGenericNumber(Type type)
        {
            String[] temp = type.Name.Split('`');
            int genericParaNum = System.Int32.Parse(temp[1], CultureInfo.InvariantCulture);
            return genericParaNum;
        }

        /// <summary>
        /// Returns an C#-style full name for the given Type, e.g. "System.Collection.Generic.List&lt;System.Int32&gt;.
        /// </summary>
        public static String GetTypeString(Type type)
        {
            String methodString = null;
            Stack<Type> stack = new Stack<Type>();
            stack.Push(type);
            while (type.IsNested == true && !(type.IsGenericType == false && type.FullName == null)) //Eliminate <T>
            {
                type = type.DeclaringType;
                stack.Push(type);
            }

            Type outterType = stack.Pop();
            methodString = outterType.ToString().Replace('+','.');
            int genericIndex = 0;

            if (outterType.IsGenericType == true)
            {
                String[] temp = methodString.Split('[');
                methodString = temp[0];
                methodString = methodString.Remove(methodString.LastIndexOf('`'));
                methodString = methodString.Insert(methodString.Length, GetGenericParaString(outterType, genericIndex));
                genericIndex += GetGenericNumber(outterType);
            }
            while (stack.Count > 0)
            {
                Type currentType = stack.Pop();
                methodString = methodString.Insert(methodString.Length, "." + currentType.Name);

                if (currentType.IsGenericType == true && currentType.Name.Contains("`") == true)
                {
                    methodString = methodString.Remove(methodString.LastIndexOf('`'));
                    methodString = methodString.Insert(methodString.Length, GetGenericParaString(currentType, genericIndex));
                    genericIndex += GetGenericNumber(currentType);
                }
            }
            return methodString;
        }

        /// <summary>
        /// Returns an C#-style signature for the given method, i.e. the kind that can be input to Signature.ConvertSignature.
        /// </summary>
        public static string GetCSharpMethodString(MethodBase m)
        {
            // **************
            // Error checking
            // **************
            if (m == null)
            {
                throw new FaultInjectionException(ApiErrorMessages.MethodSignatureNullOrEmpty);
            }

            if (m.IsConstructor && (m.DeclaringType.IsGenericType || m.DeclaringType.IsGenericTypeDefinition))
            {
                // Fail-fast message for the user.
                throw new FaultInjectionException(Constants.ApiErrorMessages.GenericConstructorNotSupported);
            }

            // **********************
            // Building the signature
            // **********************
            StringBuilder sb = new StringBuilder();

            // Add the method's containing type. Use the generic version if needed.
            Type containingType;
            if (m.ReflectedType.IsGenericType)
            {
                containingType = m.ReflectedType.GetGenericTypeDefinition();
            }
            else
            {
                containingType = m.ReflectedType;
            }

            sb.Append(MethodSignatureTranslator.GetTypeString(containingType));
            sb.Append(".");

            // Add the method name
            if (m.IsConstructor) sb.Append(containingType.Name);
            else sb.Append(m.Name);

            // Add generic type parameters
            if (m.IsGenericMethod)
            {
                // Get the generic version, e.g. Dictionary<TKey, TValue> rather then Dictionary<string, object>
                m = ((MethodInfo)m).GetGenericMethodDefinition();
            }
            if (m.IsGenericMethodDefinition)
            {
                sb.Append("<");
                bool firstTypeParameter = true;
                foreach (Type t in m.GetGenericArguments())
                {
                    if (!firstTypeParameter) sb.Append(",");
                    firstTypeParameter = false;
                    sb.Append(t.Name);
                }
                sb.Append(">");
            }

            // Add parameters
            sb.Append("(");
            bool firstParameter = true;
            foreach (ParameterInfo p in m.GetParameters())
            {
                if (!firstParameter) sb.Append(",");
                firstParameter = false;

                if (p.IsOut) sb.Append("out ");
                if (!p.IsOut && p.ParameterType.IsByRef) sb.Append("ref ");
                sb.Append(p.ParameterType.ToString().Replace("&", ""));
            }

            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Returns a formal signature for the given method.
        /// </summary>
        public static string GetFormalMethodString(MethodBase mb)
        {
            ParameterInfo[] paras = mb.GetParameters();
            System.String callingFunc = mb.ToString();

            string signatureBeforeFunName = MethodSignatureTranslator.GetTypeString(mb.DeclaringType);

            System.String[] temps = callingFunc.Split('(');
            if (temps.Length < 2)
            {
                //error handling
            }
            temps = temps[0].Split(' ');
            if (temps.Length < 2)
            {
                //error handling
            }
            temps[0] = temps[1];

            temps[0] = temps[0].Replace('[', '<');
            temps[0] = temps[0].Replace(']', '>');
            temps[0] = temps[0].Insert(0, signatureBeforeFunName + ".");
            temps[0] = temps[0].Insert(temps[0].Length, "(");

            foreach (ParameterInfo p in paras)
            {
                String typeString = MethodSignatureTranslator.GetTypeString(p.ParameterType);

                temps[0] = temps[0].Insert(temps[0].Length, typeString);
                temps[0] = temps[0].Insert(temps[0].Length, ",");
            }
            if (paras != null && paras.Length > 0)
            {
                temps[0] = temps[0].Remove(temps[0].Length - 1);
            }
            temps[0] = temps[0].Insert(temps[0].Length, ")");
            callingFunc = temps[0];
            return callingFunc;
        }

        #endregion
    }
}