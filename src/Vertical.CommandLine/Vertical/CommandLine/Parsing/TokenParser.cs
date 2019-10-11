// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Linq;

namespace Vertical.CommandLine.Parsing
{
    /// <summary>
    /// Represents a token parser.
    /// </summary>
    internal sealed class TokenParser
    {
        private readonly ITokenMatcher[] _orderedMatchers;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="orderedMatchers">The matchers, in order of invocation that determine
        /// the valid tokens.</param>
        internal TokenParser(params ITokenMatcher[] orderedMatchers)
        {
            _orderedMatchers = orderedMatchers;
        }

        /// <summary>
        /// Parses the given value.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>One or more tokens.</returns>
        internal IEnumerable<Token> Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Enumerable.Empty<Token>();

            var tokens = _orderedMatchers
                .Select(matcher => matcher.GetTokens(value))
                .FirstOrDefault(result => result.Any());

            return tokens ?? throw new ArgumentException("One or more invalid tokens");
        }
    }
}