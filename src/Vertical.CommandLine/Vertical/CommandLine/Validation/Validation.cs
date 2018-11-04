// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

namespace Vertical.CommandLine.Validation
{
    /// <summary>
    /// Defines a delegate that returns the result of validating an argument using a state
    /// value.
    /// </summary>
    /// <param name="state">Validator state value.</param>
    /// <param name="value">Value to check.</param>
    /// <typeparam name="TState">State type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    public delegate bool Validation<in TState, in TValue>(TState state, TValue value);
}