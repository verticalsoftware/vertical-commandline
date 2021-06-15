// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;

namespace Vertical.CommandLine.Conversion
{
    /// <summary>
    /// Functional class for conversion.
    /// </summary>
    internal static class ValueConverter
    {
        /// <summary>
        /// Performs value conversion.
        /// </summary>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="converter">Converter instance</param>
        /// <param name="context">Context template or argument</param>
        /// <param name="value">Value to convert</param>
        /// <returns>The converted value</returns>
        internal static TValue Convert<TValue>(IValueConverter<TValue> converter, string context, string value)
        {
            try
            {
                return converter.Convert(value);
            }
            catch (Exception ex)
            {
                throw new ConversionException(context, typeof(TValue), value, ex);
            }
        }
    }
}