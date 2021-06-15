// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System.Collections.Generic;

namespace Vertical.CommandLine.Help
{
    /// <summary>
    /// Defines the interface of a formatted string.
    /// </summary>
    public interface IFormattedString
    {
        /// <summary>
        /// Splits the justified string to the given width.
        /// </summary>
        /// <param name="width">Width.</param>
        /// <returns><see cref="IEnumerable{T}"/></returns>
        IEnumerable<Span> SplitToWidth(int width);

        /// <summary>
        /// Gets the number of spaces in the string indent.
        /// </summary>
        int Indent { get; }

        /// <summary>
        /// Gets the index within the string where the non white-space content begins.
        /// </summary>
        int StartIndex { get; }

        /// <summary>
        /// Gets the source string.
        /// </summary>
        string? Source { get; }
    }
}