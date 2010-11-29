// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;
using Microsoft.Test.FaultInjection.SignatureParsing;

namespace Microsoft.Test.FaultInjection
{
    internal static class MethodFilterHelper
    {
        #region Public Members

        public static void WriteMethodFilter(string file, FaultRule[] rules)
        {
            using (Stream stream = File.Open(file, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    foreach (FaultRule rule in rules)
                    {
                        if (rule != null)
                        {
                            string signature = Signature.ConvertSignature(rule.MethodSignature, SignatureStyle.Com);
                            writer.WriteLine(signature);
                        }
                    }
                }
            }
        }

        #endregion  
    }
}
