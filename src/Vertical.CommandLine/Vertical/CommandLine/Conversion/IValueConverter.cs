// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

namespace Vertical.CommandLine.Conversion
{
    /// <summary>
    /// Defines the interface of an object that converts values from string to the target
    /// type.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IValueConverter<out TValue>
    {
        /// <summary>
        /// Converts the given argument value to the target type.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Converted value</returns>
        TValue Convert(string value);
    }
}