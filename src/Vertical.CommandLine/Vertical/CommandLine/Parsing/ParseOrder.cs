// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

namespace Vertical.CommandLine.Parsing
{
    /// <summary>
    /// Serves as a sorting mechanism for argument parsers.
    /// </summary>
    public enum ParserType
    {
        /// <summary>
        /// Indicates a command parser
        /// </summary>
        Command,
        
        /// <summary>
        /// Indicates a help option parser
        /// </summary>
        Help,
        
        /// <summary>
        /// Indicates an option parser
        /// </summary>
        Option,
        
        /// <summary>
        /// Indicates a position argument parser
        /// </summary>
        PositionArgument
    }
}