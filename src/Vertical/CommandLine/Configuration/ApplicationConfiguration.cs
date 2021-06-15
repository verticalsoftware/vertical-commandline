// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Vertical.CommandLine.Infrastructure;
using Vertical.CommandLine.Parsing;
using Vertical.CommandLine.Help;

namespace Vertical.CommandLine.Configuration
{
    /// <summary>
    /// Configuration object used to define the options, switches and commands of a command
    /// line application.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    public class ApplicationConfiguration<TOptions> : CommandConfiguration<TOptions>
        where TOptions : class
    {
        private readonly ICollection<ICommandLineConfiguration> _subConfigurations = 
            new List<ICommandLineConfiguration>();

        private IHelpWriter? _helpWriter;

        /// <inheritdoc />
        public override IHelpWriter HelpWriter => _helpWriter ?? ConsoleHelpWriter.Default;

        /// <inheritdoc />
        public override IEnumerable<ICommandLineConfiguration> SubConfigurations => _subConfigurations;

        /// <summary>
        /// Registers a command as an application sub-program.
        /// </summary>
        /// <param name="template">Template that identifies the command.</param>
        /// <param name="configureAction">Configuration action.</param>
        /// <returns>Configuration.</returns>
        /// <exception cref="Exception">Invalid configuration.</exception>
        public ApplicationConfiguration<TOptions> Command(string template,
            Action<CommandConfiguration<TOptions>> configureAction) => Command<TOptions>(template, configureAction);
        
        /// <summary>
        /// Registers a command as an application sub-program.
        /// </summary>
        /// <param name="template">Template that identifies the command.</param>
        /// <param name="configureAction">Configuration action.</param>
        /// <typeparam name="TCommandOptions">Command options.</typeparam>
        /// <returns>Configuration.</returns>
        /// <exception cref="Exception">Invalid configuration.</exception>
        public ApplicationConfiguration<TOptions> Command<TCommandOptions>(string template,
            Action<CommandConfiguration<TCommandOptions>> configureAction)
            where TCommandOptions : class, TOptions
        {
            Check.NotNull(configureAction, nameof(configureAction));
            
            try
            {
                var commandTemplate = Template.ForCommand(template);
                var configuration = new CommandConfiguration<TCommandOptions>(commandTemplate);
                configureAction(configuration);
                _subConfigurations.Add(configuration);
                ParserConfig.AddTemplate(commandTemplate);
                return this;
            }
            catch (Exception ex)
            {
                throw ConfigurationExceptions.InvalidCommandConfiguration(template, ex);
            }
        }

        /// <summary>
        /// Registers the template of the help option.
        /// </summary>
        /// <param name="template">Template that identifies invocation of the help option.</param>
        /// <param name="helpWriter">Help writer instance to use. If not specified, the default console
        /// writer is used.</param>
        /// <returns>Configuration.</returns>
        public ApplicationConfiguration<TOptions> HelpOption(string template, IHelpWriter? helpWriter = null)
        {
            HelpTemplate = Template.ForOptionOrSwitch(template);

            _helpWriter = helpWriter;
            ParserConfig.AddTemplate(HelpTemplate);
            ParserConfig.AddParser(new HelpOptionParser<TOptions>(HelpTemplate));
           
            return this;
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => Formatting.FriendlyName(GetType());
    }
}