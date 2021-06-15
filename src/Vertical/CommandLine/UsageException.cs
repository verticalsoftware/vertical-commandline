// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;

namespace Vertical.CommandLine
{
    /// <summary>
    /// Represents an error caused by user input.
    /// </summary>
    public class UsageException : CommandLineException
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="message">Message describing the error</param>
        /// <param name="innerException">Inner exception</param>
        public UsageException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}