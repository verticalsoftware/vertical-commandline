// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System.Collections.Generic;
using Vertical.CommandLine.Parsing;
using Vertical.CommandLine.Provider;

namespace Vertical.CommandLine.Runtime
{
    /// <summary>
    /// Represents the interface of a configuration handler.
    /// </summary>
    public interface IRuntimeCommand : IProgram
    {
        /// <summary>
        /// Gets the command template.
        /// </summary>
        Template? Template { get; }
        
        /// <summary>
        /// Gets the options instance.
        /// </summary>
        /// <returns>Options.</returns>
        object GetOptions();

        /// <summary>
        /// Maps the arguments in the parse context to the options object.
        /// </summary>
        /// <param name="context">Parse context that contains the application arguments.</param>
        /// <param name="options">Options object.</param>
        /// <param name="parserType">Parse type to map.</param>
        ContextResult MapArguments(ParseContext context, object options, ParserType parserType);

        /// <summary>
        /// Gets the help content resource.
        /// </summary>
        IProvider<IReadOnlyCollection<string>>? HelpContentProvider { get; }
    }
}