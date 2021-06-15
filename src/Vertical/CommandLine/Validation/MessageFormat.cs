// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

namespace Vertical.CommandLine.Validation
{
    /// <summary>
    /// Represents a delegate that formats validation messages.
    /// </summary>
    /// <param name="state">Validator state value.</param>
    /// <param name="value">Value that failed validation.</param>
    /// <typeparam name="TState">Validator state type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    public delegate string MessageFormat<in TState, in TValue>(TState state, TValue value);
}