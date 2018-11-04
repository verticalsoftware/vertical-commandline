// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;

namespace Vertical.CommandLine.Infrastructure
{
    /// <summary>
    /// Diagnostics checking.
    /// </summary>
    internal static class Check
    {
        /// <summary>
        /// Verifies a parameter condition.
        /// </summary>
        internal static void True(bool test, string param, string message)
        {
            if (test) return;
            
            throw new ArgumentException(message, param);
        }
        
        /// <summary>
        /// Verifies a reference is not null.
        /// </summary>
        internal static void NotNull(object obj, string param)
        {
            if (!ReferenceEquals(obj, null)) return;

            throw new ArgumentNullException(param);
        }
        
        /// <summary>
        /// Verifies a string is not null or whitespace.
        /// </summary>
        internal static void NotNullOrWhiteSpace(string str, string param)
        {
            if (!string.IsNullOrWhiteSpace(str)) return;

            throw Exceptions.ParameterNullOrWhiteSpace(param);
        }
    }
}