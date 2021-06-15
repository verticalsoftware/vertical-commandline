// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Vertical.CommandLine.Help
{
    /// <summary>
    /// Represents a justified string.
    /// </summary>
    internal sealed class JustifiedString : IFormattedString
    {
        private const char Tab = '\t';
        private const char Space = ' ';
        private const int TabSpaceCount = 4;
        
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="source">The source string.</param>
        internal JustifiedString(string? source)
        {
            if (source == null) return;

            Source = source;
            
            for (var c = 0; c < source.Length; c ++)
            {
                switch (source[c])
                {
                    case Tab:
                        Indent += TabSpaceCount;
                        StartIndex++;
                        break;

                    case Space:
                        Indent++;
                        StartIndex++;
                        break;

                    default:
                        c = source.Length;
                        break;
                }
            }
        }

        /// <summary>
        /// Splits the justified string to the given width.
        /// </summary>
        /// <param name="width">Width.</param>
        /// <returns><see cref="IEnumerable{T}"/></returns>
        public IEnumerable<Span> SplitToWidth(int width)
        {
            if (Source == null)
                yield break;
            
            var wordBreak = -1;
            var spanStart = StartIndex;

            for (var c = StartIndex; c < Source.Length; c++)
            {
                if (c - spanStart > width && wordBreak > -1)
                {
                    yield return new Span(spanStart, wordBreak - spanStart);
                    spanStart = wordBreak + 1;
                    wordBreak = -1;
                }
                if (Source[c] == Space) wordBreak = c;
            }
            yield return new Span(spanStart, Source.Length - spanStart);
        }

        /// <summary>
        /// Gets the number of spaces in the string indent.
        /// </summary>
        public int Indent { get; }

        /// <summary>
        /// Gets the index within the string where the non white-space content begins.
        /// </summary>
        public int StartIndex { get; }

        /// <summary>
        /// Gets the source string.
        /// </summary>
        public string? Source { get; }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => Source ?? string.Empty;
    }
}
