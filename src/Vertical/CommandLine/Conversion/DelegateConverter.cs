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
    /// Represents a converter that uses an underlying delegate to perform the conversion.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    internal class DelegateConverter<TValue> : IValueConverter<TValue>
    {
        private readonly Func<string, TValue> _function;

        internal DelegateConverter(Func<string, TValue> function)
        {
            _function = function ?? throw new ArgumentNullException(nameof(function));
        }
        

        /// <inheritdoc />
        public TValue Convert(string value) => _function(value);

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => $"Func<string, {Formatting.FriendlyName(typeof(TValue))}>";
    }
}