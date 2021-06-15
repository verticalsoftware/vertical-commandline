// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Parsing;
using Vertical.CommandLine.Provider;

namespace Vertical.CommandLine.Runtime
{
    /// <summary>
    /// Represents an aggregated runtime command.
    /// </summary>
    internal sealed class AggregatedRuntimeCommand : IRuntimeCommand
    {
        private readonly ICollection<IRuntimeCommand> _runtimeCommands = new List<IRuntimeCommand>();

        // Define the precedence of argument parsing
        private static readonly ParserType[] OrderedParserTypes = 
        {
            ParserType.Help,
            ParserType.Option,
            ParserType.PositionArgument
        };

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="parseContext">Parse context</param>
        /// <param name="applicationCommand">Application command</param>
        /// <param name="subConfigurations">Command configurations</param>
        internal AggregatedRuntimeCommand(ParseContext parseContext,
            IRuntimeCommand applicationCommand,
            IEnumerable<ICommandLineConfiguration>? subConfigurations)
        {
            // Get the matched sub configuration
            var command = subConfigurations?
                .TakeWhile(_ => parseContext.Reset())
                .Select(config => config.GetRuntimeCommand())
                .FirstOrDefault(runtimeCommand => parseContext.TryTakeTemplate(runtimeCommand.Template!, 0));

            if (command != null) _runtimeCommands.Add(command);

            _runtimeCommands.Add(applicationCommand);
        }

        /// <summary>
        /// Gets the selected runtime command.
        /// </summary>
        public IRuntimeCommand SelectedRuntime => _runtimeCommands.First();

        /// <inheritdoc />
        public Template? Template => SelectedRuntime.Template;

        /// <inheritdoc />
        public IProvider<IReadOnlyCollection<string>>? HelpContentProvider => _runtimeCommands
            .FirstOrDefault(cmd => cmd.HelpContentProvider != null)?.HelpContentProvider;

        /// <inheritdoc />
        public object GetOptions() => SelectedRuntime.GetOptions();

        /// <inheritdoc />
        public ContextResult MapArguments(ParseContext context, object options, ParserType parserType)
        {
            return OrderedParserTypes
                .SelectMany(type => _runtimeCommands.Select(command => new { type, command }))
                .TakeWhile(_ => context.Reset())
                .Aggregate(ContextResult.NoMatch, (result, pair) => result | pair.command.MapArguments(context, options, pair.type));
        }

        /// <inheritdoc />
        public void Invoke(object options) => SelectedRuntime.Invoke(options);

        /// <inheritdoc />
        public Task InvokeAsync(object options) => SelectedRuntime.InvokeAsync(options);
    }
}