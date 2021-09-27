// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Infrastructure;
using Vertical.CommandLine.Runtime;

namespace Vertical.CommandLine
{
    /// <summary>
    /// Represents an orchestrator for command line application execution.
    /// </summary>
    public static class CommandLineApplication
    {
        /// <summary>
        /// Executes the application.
        /// </summary>
        /// <param name="configuration">Configuration instance.</param>
        /// <param name="arguments">Arguments given on the command line.</param>
        public static void Run(ICommandLineConfiguration configuration,
            IEnumerable<string> arguments)
        {
            RuntimeCommandBuilder
                .Build(configuration, arguments, out var options)
                .Invoke(options);
        }

        /// <summary>
        /// Executes the application asynchronously.
        /// </summary>
        /// <param name="configuration">Configuration instance.</param>
        /// <param name="arguments">Arguments given on the command line.</param>
        public static Task RunAsync(ICommandLineConfiguration configuration,
            IEnumerable<string> arguments)
        {
            return RuntimeCommandBuilder
                .Build(configuration, arguments, out var options)
                .InvokeAsync(options, CancellationToken.None);
        }

        /// <summary>
        /// Executes the application asynchronously.
        /// </summary>
        /// <param name="configuration">Configuration instance.</param>
        /// <param name="arguments">Arguments given on the command line.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task RunAsync(ICommandLineConfiguration configuration,
            IEnumerable<string> arguments,
            CancellationToken cancellationToken)
        {
            return RuntimeCommandBuilder
                .Build(configuration, arguments, out var options)
                .InvokeAsync(options, cancellationToken);
        }

        /// <summary>
        /// Parses the arguments without invoking the client handler.
        /// </summary>
        /// <typeparam name="TOptions">Options type.</typeparam>
        /// <param name="configuration">Configuration</param>
        /// <param name="arguments">Arguments given on the command line</param>
        /// <returns>Populated options object</returns>
        public static TOptions ParseArguments<TOptions>(ICommandLineConfiguration configuration,
            IEnumerable<string> arguments)
            where TOptions : class
        {
            RuntimeCommandBuilder.Build(configuration, arguments, out var options);

            return (TOptions)options;
        }

        /// <summary>
        /// Displays help, bypassing any invocation of client program handlers.
        /// </summary>
        /// <param name="configuration">Configuration instance</param>
        /// <param name="command">The help context - if showing help for the root application,
        /// leave as null.</param>
        public static void ShowHelp(ICommandLineConfiguration configuration,
            string? command = null)
        {
            if (configuration.HelpTemplate == null) throw ConfigurationExceptions.NoHelpOptionDefined();

            var helpToken = configuration.HelpTemplate.Tokens.First().DistinguishedForm!;

            var args = string.IsNullOrWhiteSpace(command)
                ? new[] {helpToken}
                : new[] {command, helpToken};

            Run(configuration, args);
        }
    }
}