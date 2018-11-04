// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System.Diagnostics.CodeAnalysis;

namespace Vertical.CommandLine.Conversion
{
    internal sealed class StringConverter<TValue> : IValueConverter<TValue>
    {
        private static readonly IValueConverter<TValue> Singleton = new StringConverter<TValue>();

        /// <inheritdoc />
        public TValue Convert(string value) => (TValue) (object) value;

        /// <summary>
        /// Tries to create an instance.
        /// </summary>
        internal static bool TryCreate(out IValueConverter<TValue> converter)
        {
            converter = ReferenceEquals(typeof(string), typeof(TValue)) ? Singleton : null;
            return converter != null;
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => "StringConverter";
    }
}