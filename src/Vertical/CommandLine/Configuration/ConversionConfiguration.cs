// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using Vertical.CommandLine.Conversion;

namespace Vertical.CommandLine.Configuration
{
    /// <summary>
    /// Configuration object used to define how values are converted from string arguments to the
    /// target type.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    public sealed class ConversionConfiguration<TOptions, TValue> where TOptions : class
    {
        private readonly ArgumentConfiguration<TOptions, TValue> _configuration;
        private readonly IComponentSink<IValueConverter<TValue>> _converterSink;
        
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="configuration">Argument configuration.</param>
        /// <param name="converterSink">Component sink.</param>
        internal ConversionConfiguration(ArgumentConfiguration<TOptions, TValue> configuration,
            IComponentSink<IValueConverter<TValue>> converterSink)
        {
            _configuration = configuration;
            _converterSink = converterSink;
        }

        /// <summary>
        /// Converts the value using the specified converter.
        /// </summary>
        /// <param name="converter">Converter instance.</param>
        /// <returns>Configuration.</returns>
        public ArgumentConfiguration<TOptions, TValue> Using(IValueConverter<TValue> converter)
        {
            _converterSink.Sink(converter ?? throw new ArgumentNullException(nameof(converter)));
            return _configuration;
        }

        /// <summary>
        /// Converts the value using the function.
        /// </summary>
        /// <param name="function">Function used to converter the string argument.</param>
        /// <returns>Configuration.</returns>
        public ArgumentConfiguration<TOptions, TValue> Using(Func<string, TValue> function) =>
            Using(new DelegateConverter<TValue>(function));

        /// <summary>
        /// Converts the value using a dictionary of key/value pairs.
        /// </summary>
        /// <param name="values">Values that map string arguments to option values.</param>
        /// <param name="keyComparer">Key comparer.</param>
        /// <returns>Configuration.</returns>
        public ArgumentConfiguration<TOptions, TValue> UsingValues(IEnumerable<KeyValuePair<string, TValue>> values,
            IEqualityComparer<string> keyComparer = null) =>
            Using(new DictionaryConverter<TValue>(values, keyComparer ?? EqualityComparer<string>.Default));
    }
}