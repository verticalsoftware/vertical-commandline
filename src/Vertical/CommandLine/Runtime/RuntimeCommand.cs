// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Threading.Tasks;
using Vertical.CommandLine.Infrastructure;
using Vertical.CommandLine.Provider;
using Vertical.CommandLine.Parsing;
using Vertical.CommandLine.Configuration;
using System.Collections.Generic;
using System.Threading;

namespace Vertical.CommandLine.Runtime
{
    /// <summary>
    /// Handler for commands.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    internal sealed class RuntimeCommand<TOptions> : IRuntimeCommand,
        IComponentSink<IProvider<IReadOnlyCollection<string>>>
        where TOptions : class
    {
        private const string DefaultContext = "application";
        private readonly Func<IArgumentParser<TOptions>[]> _parsers;
        
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="template">Template to match.</param>
        /// <param name="parsers">Collection of parses for the options and arguments.</param>
        internal RuntimeCommand(Template? template, Func<IArgumentParser<TOptions>[]> parsers)
        {
            Template = template;
            _parsers = parsers;
        }
        
        /// <summary>
        /// Gets or sets the command template.
        /// </summary>
        public Template? Template { get; }

        /// <inheritdoc />
        public object GetOptions()
        {
            var provider = OptionsProvider ?? ConstructorProvider<TOptions>.CreateOrThrow();
            object options;
            
            try
            {
                options = provider.GetInstance();
            }
            catch (Exception ex)
            {
                throw ConfigurationExceptions.OptionsProviderFailed(typeof(TOptions), ex);
            }

            return options ?? throw ConfigurationExceptions.OptionsProviderReturnedNull(typeof(TOptions));
        }
        
        /// <inheritdoc />
        public ContextResult MapArguments(ParseContext context, object options, ParserType parserType)
        {
            return Parser<TOptions>.Map((TOptions)options, _parsers(), context, parserType);
        }

        /// <inheritdoc />
        public void Invoke(object options) => GetClientHandlerOrThrow().Invoke((TOptions) options);

        /// <inheritdoc />
        public Task InvokeAsync(object options, CancellationToken cancellationToken) => GetClientHandlerOrThrow()
            .InvokeAsync((TOptions) options, cancellationToken);

        /// <summary>
        /// Gets or sets the client handler.
        /// </summary>
        public ClientHandler<TOptions>? ClientHandler { get; set; }
        
        /// <summary>
        /// Gets or sets the options provider.
        /// </summary>
        public IProvider<TOptions>? OptionsProvider { get; set; }

        /// <summary>
        /// Gets or sets the help content resource.
        /// </summary>
        public IProvider<IReadOnlyCollection<string>>? HelpContentProvider { get; private set; }

        // Ensures the client handler was defined.
        private ClientHandler<TOptions> GetClientHandlerOrThrow()
        {
            return ClientHandler ?? throw ConfigurationExceptions.NoClientHandlerDefined(
                       Template?.ToString() ?? DefaultContext);
        }

        void IComponentSink<IProvider<IReadOnlyCollection<string>>>.Sink(IProvider<IReadOnlyCollection<string>> component) =>
            HelpContentProvider = component;
    }
}