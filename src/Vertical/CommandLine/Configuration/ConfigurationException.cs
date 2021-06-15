// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;

namespace Vertical.CommandLine.Configuration
{
    /// <summary>
    /// Represents an error in configuration.
    /// </summary>
    /// <remarks>
    /// This exception is thrown for one of the following scenarios:
    /// - An invalid character in a template. Typically templates only allow letters, numbers and dashes
    /// - An invalid template, e.g. short option with more than once character
    /// - An argument, option or switch does not have an assigned property mapping at runtime
    /// - A converter wasn't specified, and the API didn't find an out-of-box option
    /// - An exception was thrown in mapping, conversion or validation delegate code
    /// </remarks>
    public sealed class ConfigurationException : CommandLineException
    {
        /// <inheritdoc />
        internal ConfigurationException(string message, Exception innerException = null) 
            : base(message, innerException)
        {
        }
    }
}