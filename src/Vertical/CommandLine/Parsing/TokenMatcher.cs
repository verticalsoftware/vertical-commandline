// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Vertical.CommandLine.Parsing
{
    /// <summary>
    /// Represents a matcher that produces tokens given string input.
    /// </summary>
    public sealed class TokenMatcher : ITokenMatcher
    {
        //private const string TemplateIdGroup = "value";
        private const string TemplateIdGroup = "id";
        private const string OperandGroup = "operand";
        private readonly string _pattern;
        private readonly Func<Match, Token[]> _tokenFactory;

        // Patterns used by the matchers
        private static readonly string ShortOptionPattern = $"^-(?<{TemplateIdGroup}>[0-9a-zA-Z])$";
        private static readonly string CompactShortOptionPattern = $"^-(?<{TemplateIdGroup}>[0-9a-zA-Z]+)$";
        private static readonly string LongOptionPattern = $"^--(?<{TemplateIdGroup}>[\\w-]+)$";
        private static readonly string WordPattern = $"^(?![\\W])(?<{TemplateIdGroup}>[0-9a-zA-Z-]+)$";
        private static readonly string CompositeOptionPattern = $"^--?(?<{TemplateIdGroup}>[\\w-]+)[=:](?<{OperandGroup}>.+)?$";
        private const string OptionsEndPattern = "^--$";
        private const string AnyPattern = ".+";

        private TokenMatcher(string pattern, Func<Match, Token[]> tokenFactory)
        {
            _pattern = pattern;
            _tokenFactory = tokenFactory;
        }

        /// <summary>
        /// Gets the tokens for the given value.
        /// </summary>
        /// <param name="value">Value to parse.</param>
        /// <returns>Token array or null if no match was made.</returns>
        public Token[] GetTokens(string value)
        {
            var match = Regex.Match(value, _pattern);
            return match.Success ? _tokenFactory(match) : Array.Empty<Token>();
        }

        /// <summary>
        /// Defines a matcher for double dash.
        /// </summary>
        public static TokenMatcher OptionsEnd { get; } = new TokenMatcher(
            OptionsEndPattern, _ => new []{ Token.OptionsEnd });

        /// <summary>
        /// Defines a matcher for short options.
        /// </summary>
        public static TokenMatcher ShortOption { get; } = new TokenMatcher(
            ShortOptionPattern, 
            match => new []{new Token(TokenType.ShortOption, match.Groups[TemplateIdGroup].Value) });

        /// <summary>
        /// Defines a matcher for compact short options.
        /// </summary>
        public static TokenMatcher CompactShortOption { get; } = new TokenMatcher(
            CompactShortOptionPattern,
            match => match.Groups[TemplateIdGroup].Value
                .Select(ch => new Token(TokenType.ShortOption, ch.ToString()))
                .ToArray());

        /// <summary>
        /// Defines a pattern for long options.
        /// </summary>
        public static TokenMatcher LongOption { get; } = new TokenMatcher(
            LongOptionPattern,
            match => new[] { new Token(TokenType.LongOption, match.Groups[TemplateIdGroup].Value) });

        /// <summary>
        /// Defines a pattern for composite options.
        /// </summary>
        public static TokenMatcher CompositeOption { get; } = new TokenMatcher(
            CompositeOptionPattern,
            match =>
            {
                var attachedOperand = match.Groups[OperandGroup].Value;
                var templateId = match.Groups[TemplateIdGroup].Value;

                return attachedOperand.Length > 0
                    ? new[]
                    {
                        new Token(TokenType.CompositeOption, templateId),
                        new Token(TokenType.NonTemplateValue, attachedOperand)
                    }
                    : new[]
                    {
                        new Token(TokenType.CompositeOption, templateId)
                    };
            });
        
        /// <summary>
        /// Defines a pattern for words.
        /// </summary>
        public static TokenMatcher Word { get; } = new TokenMatcher(
            WordPattern,
            match => new[] { new Token(TokenType.NonTemplateValue, match.Groups[TemplateIdGroup].Value) });

        /// <summary>
        /// Defines a pattern for any string.
        /// </summary>
        public static TokenMatcher Any { get; } = new TokenMatcher(
            AnyPattern,
            match => new [] {new Token(TokenType.NonTemplateValue, match.Value) });
    }
}