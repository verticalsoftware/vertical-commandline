// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;

namespace Vertical.CommandLine.Parsing
{
    /// <summary>
    /// Defines the result of context processing.
    /// </summary>
    [Flags]
    public enum ContextResult
    {
        /// <summary>
        /// Indicates the parser did not match any tokens.
        /// </summary>
        NoMatch = 0,
        
        /// <summary>
        /// Indicates the parser matched a help template.
        /// </summary>
        Help = 1,
 
        /// <summary>
        /// Indicates the parser matched an argument, option or switch.
        /// </summary>
        Argument = 2,
        
        /// <summary>
        /// Indicates the parser matched a command template.
        /// </summary>
        Command = 4
    }
}