// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

namespace Vertical.CommandLine.Validation
{
    /// <summary>
    /// Defines the interface of an object that validates option values.
    /// </summary>
    public interface IValidator<in TValue>
    {
        /// <summary>
        /// Validates the given converted value.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        bool Validate(TValue value);

        /// <summary>
        /// Gets the error given the failed value.
        /// </summary>
        string GetError(TValue value);
    }
}