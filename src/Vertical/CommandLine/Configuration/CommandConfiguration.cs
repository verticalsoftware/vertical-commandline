// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Threading.Tasks;
using Vertical.CommandLine.Runtime;
using Vertical.CommandLine.Infrastructure;
using Vertical.CommandLine.Provider;
using Vertical.CommandLine.Parsing;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Vertical.CommandLine.Help;

namespace Vertical.CommandLine.Configuration
{
    /// <summary>
    /// Configuration object used to define application arguments, options, and switches.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    public class CommandConfiguration<TOptions> : IComponentSink<IProvider<TOptions>>,
        ICommandLineConfiguration
        where TOptions : class
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        protected CommandConfiguration() : this(null)
        {
        }
        
        /// <summary>
        /// Gets the parser configuration.
        /// </summary>
        protected ParserConfiguration<TOptions> ParserConfig { get; } = new ParserConfiguration<TOptions>();

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="template">The template that identifies the command - null if this is an application
        /// configuration.</param>
        internal CommandConfiguration(Template? template)
        {
            RuntimeCommand = new RuntimeCommand<TOptions>(template, () => ParserConfig.ArgumentParsers);
        }

        /// <summary>
        /// Gets the runtime command.
        /// </summary>
        internal RuntimeCommand<TOptions> RuntimeCommand { get; }

        /// <inheritdoc />
        public virtual Template? HelpTemplate { get; protected set; }
        
        /// <summary>
        /// Adds a position argument to the configuration.
        /// </summary>
        /// <param name="configureAction">Configuration action.</param>
        /// <returns>Configuration.</returns>
        public CommandConfiguration<TOptions> PositionArgument(
            Action<MultiValueArgumentConfiguration<TOptions, string>> configureAction) =>
            PositionArgument<string>(configureAction);
        
        /// <summary>
        /// Adds a position argument to the configuration.
        /// </summary>
        /// <param name="configureAction">Configuration action.</param>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <returns>Configuration.</returns>
        public CommandConfiguration<TOptions> PositionArgument<TValue>(
            Action<MultiValueArgumentConfiguration<TOptions, TValue>> configureAction)
        {
            var index = ParserConfig.GetNextArgumentIdentity();
            
            return ConfigureArgument<TValue>(Common.FormatArgumentContext(index),
                    builder => MultiValueArgumentConfiguration<TOptions, TValue>
                .Configure(builder, configureAction)
                .PositionArgument(index));
        }

        /// <summary>
        /// Adds an option to the configuration.
        /// </summary>
        /// <param name="template">Template that identifies the option.</param>
        /// <param name="configureAction">Configuration action.</param>
        /// <returns>Configuration.</returns>
        public CommandConfiguration<TOptions> Option(string template,
            Action<MultiValueArgumentConfiguration<TOptions, string>> configureAction) =>
            Option<string>(template, configureAction);

        /// <summary>
        /// Adds an option to the configuration.
        /// </summary>
        /// <param name="template">Template that identifies the option.</param>
        /// <param name="configureAction">Configuration action.</param>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <returns>Configuration.</returns>
        public CommandConfiguration<TOptions> Option<TValue>(string template,
            Action<MultiValueArgumentConfiguration<TOptions, TValue>> configureAction)
        {
            return ConfigureArgument<TValue>(template, builder => MultiValueArgumentConfiguration<TOptions, TValue>
                .Configure(builder, configureAction)
                .Option(Template.ForOptionOrSwitch(template)));
        }
        
        /// <summary>
        /// Adds a switch style option to the configuration.
        /// </summary>
        /// <param name="template">Template that identifies the switch.</param>
        /// <param name="configureAction">Configuration action.</param>
        /// <returns>Configuration.</returns>
        public CommandConfiguration<TOptions> Switch(string template,
            Action<ArgumentConfiguration<TOptions, bool>> configureAction)
        {
            return ConfigureArgument<bool>(template, builder => ArgumentConfiguration<TOptions, bool>
                .Configure(builder, configureAction)
                .Switch(Template.ForOptionOrSwitch(template)));
        }

        /// <summary>
        /// Registers the action handler for the application.
        /// </summary>
        /// <param name="action">Synchronous handler.</param>
        /// <returns>Configuration.</returns>
        public CommandConfiguration<TOptions> OnExecute(Action<TOptions> action)
        {
            RuntimeCommand.ClientHandler = new ClientHandler<TOptions>(action ?? 
                throw new ArgumentNullException(nameof(action)));

            return this;
        }

        /// <summary>
        /// Registers an asynchronous handler for the application.
        /// </summary>
        /// <param name="asyncAction">Asynchronous handler.</param>
        /// <returns>Configuration.</returns>
        public CommandConfiguration<TOptions> OnExecuteAsync(Func<TOptions, Task> asyncAction)
        {
            RuntimeCommand.ClientHandler = new ClientHandler<TOptions>(
                asyncAction ?? throw new ArgumentNullException(nameof(asyncAction)));

            return this;
        }
        
        /// <summary>
        /// Registers an asynchronous handler for the application.
        /// </summary>
        /// <param name="asyncAction">Asynchronous handler.</param>
        /// <returns>Configuration.</returns>
        public CommandConfiguration<TOptions> OnExecuteAsync(Func<TOptions, CancellationToken, Task> asyncAction)
        {
            RuntimeCommand.ClientHandler = new ClientHandler<TOptions>(
                asyncAction ?? throw new ArgumentNullException(nameof(asyncAction)));

            return this;
        }
        
        /// <summary>
        /// Gets a configuration object used to define how the options instance is provided.
        /// </summary>
        public ProviderConfiguration<TOptions> Options => new ProviderConfiguration<TOptions>(this, this);

        /// <summary>
        /// Gets a configuration object used to define help content.
        /// </summary>
        public HelpConfiguration<TOptions> Help => new HelpConfiguration<TOptions>(this, RuntimeCommand);

        /// <inheritdoc />
        public virtual IEnumerable<ICommandLineConfiguration>? SubConfigurations => null;

        /// <inheritdoc />
        public virtual IHelpWriter? HelpWriter => null;

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => $"{Formatting.FriendlyName(GetType())}: {RuntimeCommand.Template}";

        // Invokes configuration for an argument.
        private CommandConfiguration<TOptions> ConfigureArgument<TValue>(object context,
            Func<ParserBuilder<TOptions, TValue>, IArgumentParser<TOptions>> configuration)
        {
            ParserConfig.ConfigureParser(context, configuration);
            return this;
        }

        /// <inheritdoc />
        void IComponentSink<IProvider<TOptions>>.Sink(IProvider<TOptions> component) =>
            RuntimeCommand.OptionsProvider = component;
                
        /// <inheritdoc />
        IRuntimeCommand ICommandLineConfiguration.GetRuntimeCommand() => RuntimeCommand;
    }
}