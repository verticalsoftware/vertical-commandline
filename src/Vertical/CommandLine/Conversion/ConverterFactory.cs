// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Vertical.CommandLine.Infrastructure;

namespace Vertical.CommandLine.Conversion
{
    /// <summary>
    /// Factory for out-of-box converters.
    /// </summary>
    internal static class ConverterFactory
    {
        /// <summary>
        /// Creates or throws a converter.
        /// </summary>
        internal static IValueConverter<TValue> CreateOrThrow<TValue>()
        {
            var created =
                StringConverter<TValue>.TryCreate(out var converter) ||
                ParseConverter<TValue>.TryCreate(out converter) ||
                EnumConverter<TValue>.TryCreate(out converter) ||
                NullableTypeParseConverter<TValue>.TryCreate(out converter) ||
                NullableEnumConverter<TValue>.TryCreate(out converter) ||
                ConstructorConverter<TValue>.TryCreate(out converter) ||
                CastConverter<TValue>.TryCreate(out converter);

            if (created) return converter!;

            throw ConfigurationExceptions.NoDefaultConverter<TValue>();
        }
    }
}