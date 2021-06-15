// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

namespace Vertical.CommandLine.Parsing
{
    /// <summary>
    /// Defines the interface of an object that matches string values to patterns
    /// and produces tokens.
    /// </summary>
    public interface ITokenMatcher
    {
        /// <summary>
        /// Gets the tokens for the given value.
        /// </summary>
        /// <param name="value">Value to parse.</param>
        /// <returns>Token array or null if no match was made.</returns>
        Token[] GetTokens(string value);
    }
}