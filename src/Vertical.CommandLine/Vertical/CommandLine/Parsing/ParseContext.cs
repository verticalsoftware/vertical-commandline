// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Vertical.CommandLine.Parsing
{
    /// <summary>
    /// Represents a contextual iterator tht manages argument tokens.
    /// </summary>
    public sealed class ParseContext : IEnumerable<Token>
    {
        // Used to track original argument order
        private struct TokenEntry
        {
            internal TokenEntry(int parsedIndex, Token token)
            {
                ParsedIndex = parsedIndex;
                Token = token;
            }
            
            internal int ParsedIndex { get; }
            internal Token Token { get; }

            [ExcludeFromCodeCoverage]
            public override string ToString() => $"@{ParsedIndex}: {Token}";
        }
        
        private static readonly TokenParser Parser = new TokenParser(
            TokenMatcher.OptionsEnd,
            TokenMatcher.ShortOption,
            TokenMatcher.CompactShortOption,
            TokenMatcher.LongOption,
            TokenMatcher.Any);

        private readonly IList<TokenEntry> _argumentTokenList;
        private int _index;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        internal ParseContext(IEnumerable<string> arguments) => _argumentTokenList = BuildEntries(arguments);
        
        private TokenEntry CurrentEntry => _argumentTokenList[_index];

        /// <summary>
        /// Gets the current entry.
        /// </summary>
        public Token Current => CurrentEntry.Token;

        /// <summary>
        /// Gets whether an argument token is available in the context.
        /// </summary>
        public bool Ready => _index < _argumentTokenList.Count;
       
        /// <summary>
        /// Evaluates the ready token in the context to be used as a string argument.
        /// </summary>
        /// <param name="token">When the method returns, the string token.</param>
        /// <returns>True if the ready token is a string argument.</returns>
        public bool TryTakeStringValue(out Token token) => TryConsumeTokenOrSkip(t => t.Type == TokenType.NonTemplateValue, 
            null, out token);

        /// <summary>
        /// Evaluates the ready token in the context to be used as an option argument.
        /// </summary>
        /// <param name="template">Template to match.</param>
        /// <returns>True if the ready token matches the given template.</returns>
        public bool TryTakeTemplate(Template template) => TryConsumeTokenOrSkip(template.Contains, null, out var unused);

        /// <summary>
        /// Evaluates the ready token in the context to be used as an option argument.
        /// </summary>
        /// <param name="template">Template to match.</param>
        /// <param name="index">The template index to match.</param>
        /// <returns>True if the ready token matches the given template and the token is at the given index.</returns>
        public bool TryTakeTemplate(Template template, int index) => TryConsumeTokenOrSkip(template.Contains, index,
            out var unused);

        /// <summary>
        /// Resets the context to the first available argument token.
        /// </summary>
        /// <returns>Whether an argument token is available in the context.</returns>
        public bool Reset()
        {
            _index = 0;
            return Ready;
        }

        /// <summary>
        /// Gets the number of tokens in the parse context.
        /// </summary>
        public int Count => _argumentTokenList.Count;

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => Ready ? CurrentEntry.ToString() : "(end/empty)";

        // Consumes or skips the current token.
        private bool TryConsumeTokenOrSkip(Func<Token, bool> predicate, int? index, out Token token)
        {
            if (!Ready)
            {
                token = Token.Empty;
                return false;
            }

            token = CurrentEntry.Token;

            if (!(predicate(token) && (index == null || index == CurrentEntry.ParsedIndex)))
            {
                ++_index;
                return false;
            }

            _argumentTokenList.RemoveAt(_index);
            return true;
        }

        // Builds the entries for the context
        private static IList<TokenEntry> BuildEntries(IEnumerable<string> arguments)
        {
            var array = arguments.ToArray();
            var entryList = new List<TokenEntry>(array.Length * 2);
            var count = 0;

            entryList.AddRange(array
                .SelectMany(arg => { ++count; return Parser.Parse(arg); })
                .TakeWhile(token => token.Type != TokenType.OptionsEnd)
                .Select(token => new TokenEntry(entryList.Count, token)));

            entryList.AddRange(array.Skip(count).Select(arg => new TokenEntry(entryList.Count, 
                new Token(TokenType.NonTemplateValue, arg))));

            return entryList;
        }

        /// <inheritdoc />
        public IEnumerator<Token> GetEnumerator() => _argumentTokenList.Select(entry => entry.Token).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}