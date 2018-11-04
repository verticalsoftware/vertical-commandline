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
        private readonly ITokenMatcher[] _orderMatchers;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="orderMatchers">The matchers, in order of invocation that determine
        /// the valid tokens.</param>
        internal TokenParser(params ITokenMatcher[] orderMatchers)
        {
            _orderMatchers = orderMatchers;
        }

        /// <summary>
        /// Parses the given value.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>One or more tokens.</returns>
        internal IEnumerable<Token> Parse(string value)
        {
            var tokens = _orderMatchers
                .Select(matcher => matcher.GetTokens(value))
                .FirstOrDefault(result => result.Any());

            return tokens ?? throw new ArgumentException();
        }
    }
}