// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Linq;
using Vertical.CommandLine.Help;
using Vertical.CommandLine.Provider;

namespace Vertical.CommandLine.Configuration
{
    /// <summary>
    /// Configuration object used to define help for the command.
    /// </summary>
    /// <typeparam name="TOptions">Command options type.</typeparam>
    public sealed class HelpConfiguration<TOptions> where TOptions : class
    {
        private readonly CommandConfiguration<TOptions> _configuration;
        private readonly IComponentSink<IProvider<IReadOnlyCollection<string>>> _componentSink;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="componentSink">Component sink</param>
        internal HelpConfiguration(CommandConfiguration<TOptions> configuration,
            IComponentSink<IProvider<IReadOnlyCollection<string>>> componentSink)
        {
            _configuration = configuration;
            _componentSink = componentSink;
        }

        /// <summary>
        /// Uses the given content provider to return help content for the command.
        /// </summary>
        /// <param name="contentProvider">Content provider.</param>
        /// <returns>Configuration.</returns>
        public CommandConfiguration<TOptions> Using(IProvider<IReadOnlyCollection<string>> contentProvider)
        {
            _componentSink.Sink(contentProvider ?? throw new ArgumentNullException(nameof(contentProvider)));
            return _configuration;
        }

        /// <summary>
        /// Uses the content in the file specified by the path.
        /// </summary>
        /// <param name="path">Path to the help content file.</param>
        /// <returns>Configuration.</returns>
        public CommandConfiguration<TOptions> UseFile(string path) => Using(new FileHelpContentProvider(path));

        /// <summary>
        /// Uses the given reference as help content.
        /// </summary>
        /// <param name="content">Content to display </param>
        /// <returns></returns>
        public CommandConfiguration<TOptions> UseContent(IEnumerable<string> content) =>
            Using(new InstanceProvider<IReadOnlyCollection<string>>(content.ToArray()));
    }
}
