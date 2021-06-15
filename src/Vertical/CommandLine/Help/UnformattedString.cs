using System.Collections.Generic;

namespace Vertical.CommandLine.Help
{
    /// <summary>
    /// Represents an unformatted string.
    /// </summary>
    internal sealed class UnformattedString : IFormattedString
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="source">Source string</param>
        internal UnformattedString(string source)
        {
            Source = source;
        }

        /// <inheritdoc />
        public IEnumerable<Span> SplitToWidth(int width)
        {
            yield return new Span(0, Source.Length);
        }

        /// <inheritdoc />
        public int Indent => 0;

        /// <inheritdoc />
        public int StartIndex => 0;

        /// <inheritdoc />
        public string Source { get; }
    }
}