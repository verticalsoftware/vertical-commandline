// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System.Collections.Generic;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Infrastructure;
using Vertical.CommandLine.Parsing;

namespace Vertical.CommandLine.Runtime
{
    /// <summary>
    /// Represents a functional object used to build the runtime command.
    /// </summary>
    internal static class RuntimeCommandBuilder
    {
        // Performs core runtime actions
        internal static IProgram Build(ICommandLineConfiguration configuration,
            IEnumerable<string> arguments,
            out object options)
        {
            Check.NotNull(arguments, nameof(arguments));
            Check.NotNull(configuration, nameof(configuration));

            // Create parse context
            var parseContext = new ParseContext(arguments);

            // Aggregate commands
            var aggregatedRuntime = new AggregatedRuntimeCommand(parseContext,
                configuration.GetRuntimeCommand(),
                configuration.SubConfigurations);

            // Create options instance
            options = aggregatedRuntime.GetOptions();

            // Map arguments
            var result = aggregatedRuntime.MapArguments(parseContext, options, default(ParserType));

            // Show help?
            if ((result & ContextResult.Help) == ContextResult.Help)
            {
                return new HelpProgram(configuration.HelpWriter, aggregatedRuntime.HelpContentProvider);
            }

            CheckContextStragglers(parseContext);

            return aggregatedRuntime.SelectedRuntime;
        }

        /// <summary>
        /// Checks for unmatched tokens in the parse context.
        /// </summary>
        private static void CheckContextStragglers(ParseContext parseContext)
        {
            if (!parseContext.Reset()) return;

            throw Exceptions.InvalidCommandLineArgument(parseContext.Current.DistinguishedForm);
        }
    }
}