// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

namespace Vertical.CommandLine.Help
{
    /// <summary>
    /// Defines the interface of an object that creates formatted strings.
    /// </summary>
    public interface IFormatter
    {
        /// <summary>
        /// Creates a formatted string.
        /// </summary>
        /// <param name="source">Source string to format</param>
        /// <returns><see cref="IFormattedString"/></returns>
        IFormattedString CreateFormatted(string source);
    }
}