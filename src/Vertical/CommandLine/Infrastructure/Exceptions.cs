// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Linq;
using Vertical.CommandLine.Parsing;
using static Vertical.CommandLine.Infrastructure.Formatting;

namespace Vertical.CommandLine.Infrastructure
{
    /// <summary>
    /// Defines general exceptions.
    /// </summary>
    internal static class Exceptions
    {
        internal static Exception ParameterNullOrWhiteSpace(string param)
        {
            return new ArgumentException("String is null, empty, or whitespace.", param);
        }

        internal static Exception OperandMissing(Template template)
        {
            return new UsageException($"Option {Quote(template.ToString())} requires an operand value.");
        }

        internal static Exception InvalidArgumentValue<TValue>(TValue value, string message)
        {
            return new ArgumentException(message ?? $"Invalid argument value {Quote(value)}.");
        }

        internal static Exception EnumConversionFailed(Type enumType, string value)
        {
            var validConstants = string.Join(", ", Enum.GetNames(enumType));
            var message = $"{Quote(value)} is not a valid value, choices are: {validConstants}";

            return new ArgumentException(message);
        }

        internal static Exception EnumConversionFailed<T>(string value) => EnumConversionFailed(typeof(T), value);

        internal static Exception DictionaryConversionFailed<T>(IDictionary<string, T> dictionary, string value)
        {
            var validValues = string.Join(", ", dictionary.Keys.OrderBy(key => key));
            var message = $"{Quote(value)} is not a valid value, choices are: {validValues}";

            return new ArgumentException(message);
        }

        internal static Exception InvalidCommandLineArgument(string argument)
        {
            return new UsageException($"Invalid argument or option '{argument}'");
        }

        internal static string DefaultValidationMessage(object value)
        {
            return $"invalid value {Quote(value)}";
        }

        internal static Exception InvalidHelpWriter() => new InvalidOperationException(
            "Using the help feature requires a register IHelpWriter");

        internal static Exception InvalidHelpContentProvider() => new InvalidOperationException(
            "Using the help feature requires a content provider");
    }
}