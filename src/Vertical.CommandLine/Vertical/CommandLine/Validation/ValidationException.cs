// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

namespace Vertical.CommandLine.Validation
{
    /// <summary>
    /// Represents an error that occurs during argument processing.
    /// </summary>
    public class ValidationException : UsageException
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="context">Context, template or position argument</param>
        /// <param name="value">Value that failed</param>
        internal ValidationException(string message, string context, object value) :
            base(message)
        {
            ArgumentValue = value;
            Context = context;
        }

        /// <summary>
        /// Gets the argument value.
        /// </summary>
        public object ArgumentValue { get; }

        /// <summary>
        /// Gets the context of the validation, template or argument.
        /// </summary>
        public string Context { get; }
    }
}