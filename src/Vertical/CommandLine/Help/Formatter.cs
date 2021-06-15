// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;

namespace Vertical.CommandLine.Help
{
    /// <summary>
    /// Represents a formatter.
    /// </summary>
    public sealed class Formatter : IFormatter
    {
        private readonly Func<string, IFormattedString> _factory;

        private Formatter(Func<string, IFormattedString> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Defines a formatter that justifies strings.
        /// </summary>
        public static IFormatter JustifiedFormatter { get; } = new Formatter(source => new JustifiedString(source));

        /// <summary>
        /// Defines a formatter that performs no formatting.
        /// </summary>
        public static IFormatter DefaultFormatter { get; } = new Formatter(source => new UnformattedString(source));

        /// <inheritdoc />
        public IFormattedString CreateFormatted(string source) => _factory(source);
    }
}