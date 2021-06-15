// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Linq;
using Vertical.CommandLine.Infrastructure;

namespace Vertical.CommandLine.Parsing
{
    /// <summary>
    /// Represents a template that describes the matching pattern for commands,
    /// options and switches.
    /// </summary>
    public sealed class Template
    {
        private const char TokenSeparator = '|';

        private static readonly TokenParser TemplateParser = new TokenParser(
            TokenMatcher.ShortOption,
            TokenMatcher.LongOption);

        private static readonly TokenParser CommandParser = new TokenParser(
            TokenMatcher.Word);

        private Template(Token[] tokens) => Tokens = tokens;

        /// <summary>
        /// Gets the template tokens.
        /// </summary>
        public Token[] Tokens { get; }

        /// <summary>
        /// Creates a template for an option or switch.
        /// </summary>
        /// <param name="template">String to convert to template.</param>
        /// <returns><see cref="Template"/></returns>
        internal static Template ForOptionOrSwitch(string template)
        {
            return new Template(BuildTokens(template, TemplateParser,
                () => ConfigurationExceptions.InvalidOptionOrSwitchTemplate(template)));
        }

        /// <summary>
        /// Creates a template for a command.
        /// </summary>
        /// <param name="template">String to convert to template.</param>
        /// <returns><see cref="Template"/></returns>
        internal static Template ForCommand(string template)
        {
            return new Template(BuildTokens(template, CommandParser,
                () => ConfigurationExceptions.InvalidCommandTemplate(template)));
        }

        /// <inheritdoc />
        public override string ToString() => string.Join(TokenSeparator.ToString(),
            Tokens.Select(token => token.DistinguishedForm));

        /// <summary>
        /// Determines if the template contains the given token.
        /// </summary>
        /// <param name="token">Token to evaluate.</param>
        /// <returns>True if the any tokens in the internal token collection match the given value.</returns>
        public bool Contains(Token token) => Tokens.Any(t => t.Equals(token));

        // Builds the token list
        private static Token[] BuildTokens(string template, TokenParser parser,
            Func<Exception> exceptionFactory)
        {
            Check.NotNullOrWhiteSpace(template, nameof(template));

            try
            {
                return template
                    .Split(TokenSeparator)
                    .SelectMany(part => parser.Parse(part.Trim()))
                    .Distinct()
                    .ToArray();
            }
            catch (ArgumentException)
            {
                throw exceptionFactory();
            }
        }
    }
}