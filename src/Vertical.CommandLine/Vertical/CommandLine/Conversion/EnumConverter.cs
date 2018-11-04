// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Diagnostics.CodeAnalysis;
using Vertical.CommandLine.Infrastructure;

namespace Vertical.CommandLine.Conversion
{
    /// <summary>
    /// Represents an object that converts string values to enum.
    /// </summary>
    /// <typeparam name="TValue">Enum type.</typeparam>
    internal sealed class EnumConverter<TValue> : IValueConverter<TValue>
    {
        private EnumConverter()
        {
        }

        /// <summary>
        /// Tries to create a new instance.
        /// </summary>
        internal static bool TryCreate(out IValueConverter<TValue> converter)
        {
            converter = typeof(TValue).IsEnum ? new EnumConverter<TValue>() : null;
            return converter != null;
        }
        
        /// <inheritdoc />
        public TValue Convert(string value)
        {
            try
            {
                return (TValue)Enum.Parse(typeof(TValue), value, true);
            }
            catch (ArgumentException)
            {
                throw Exceptions.EnumConversionFailed<TValue>(value);
            }
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => $"Enum.Parse(typeof({Formatting.FriendlyName(typeof(TValue))}), <arg>)";
    }
}