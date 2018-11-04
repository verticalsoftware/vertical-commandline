// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using Vertical.CommandLine.Infrastructure;

namespace Vertical.CommandLine.Conversion
{
    /// <summary>
    /// Represents an error that occurs during conversion.
    /// </summary>
    public class ConversionException : UsageException
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="context">The context (template or argument).</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="innerException">Inner exception that occurred.</param>
        internal ConversionException(string context, Type targetType, string argumentValue, Exception innerException)
            : base(FormatMessage(context, targetType, argumentValue), innerException)
        {
            ArgumentValue = argumentValue;
            TargetType = targetType;
            Context = context;
        }

        /// <summary>
        /// Gets the argument value that was the problem.
        /// </summary>
        public string ArgumentValue { get; }

        /// <summary>
        /// Gets the type being converted to.
        /// </summary>
        public Type TargetType { get; }

        /// <summary>
        /// Gets the context of the conversion. This will be the template
        /// of an option or switch or the index of a position argument.
        /// </summary>
        public string Context { get; }

        // Formats the exception message.
        private static string FormatMessage(string context, Type targetType, string argumentValue)
        {
            return $"{context}: could not convert {Formatting.Quote(argumentValue)} to target type {targetType.Name}.";
        }
    }
}