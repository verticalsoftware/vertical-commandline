// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Vertical.CommandLine.Infrastructure;

namespace Vertical.CommandLine.Conversion
{
    /// <summary>
    /// Represents a converter that uses a dictionary to associate string values
    /// with valid values.
    /// </summary>
    /// <typeparam name="TValue">Value type.</typeparam>
    internal sealed class DictionaryConverter<TValue> : IValueConverter<TValue>
    {
        private readonly IDictionary<string, TValue> _dictionary;
        
        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="values">The key/value pairs that provide the conversion mapping.</param>
        /// <param name="keyComparer">Key comparer.</param>
        internal DictionaryConverter(IEnumerable<KeyValuePair<string, TValue>> values,
            IEqualityComparer<string> keyComparer)
        {
            Check.NotNull(values, nameof(values));
            _dictionary = values.ToDictionary(kvp => kvp.Key, kvp => kvp.Value,
                keyComparer ?? throw new ArgumentNullException(nameof(keyComparer)));
        }

        /// <inheritdoc />
        public TValue Convert(string value)
        {
            if (_dictionary.TryGetValue(value, out var convertedValue))
                return convertedValue;

            throw Exceptions.DictionaryConversionFailed(_dictionary, value);
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => "Dictionary[<arg>]";
    }
}