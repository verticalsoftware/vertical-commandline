// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

namespace Vertical.CommandLine.Parsing
{
    /// <summary>
    /// Defines the interface of an object that handles a specific type
    /// of command line argument.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    public interface IArgumentParser<in TOptions> where TOptions : class
    {
        /// <summary>
        /// Gets the parse order for the parser.
        /// </summary>
        ParserType ParserType { get; }
        
        /// <summary>
        /// Gets whether the parser can handle multiple arguments.
        /// </summary>
        bool MultiValued { get; }

        /// <summary>
        /// Evaluates the parse context.
        /// </summary>
        /// <param name="options">Target options instance.</param>
        /// <param name="parseContext">Parse context.</param>
        /// <returns>The result of argument processing.</returns>
        ContextResult ProcessContext(TOptions options, ParseContext parseContext);
    }
}