// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using Vertical.CommandLine.Provider;

namespace Vertical.CommandLine.Configuration
{
    /// <summary>
    /// Configuration object used to define how an options instance is provided.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    public class ProviderConfiguration<TOptions> where TOptions : class
    {
        private readonly IComponentSink<IProvider<TOptions>> _componentSink;
        private readonly CommandConfiguration<TOptions> _configuration;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        /// <param name="componentSink">Component sink.</param>
        internal ProviderConfiguration(CommandConfiguration<TOptions> configuration,
            IComponentSink<IProvider<TOptions>> componentSink)
        {
            _configuration = configuration;
            _componentSink = componentSink;
        }

        /// <summary>
        /// Registers the given instance to be used.
        /// </summary>
        /// <param name="options">The options instance to populate with parsed argument values.</param>
        /// <returns>Configuration.</returns>
        public CommandConfiguration<TOptions> UseInstance(TOptions options) =>
            Sink(new InstanceProvider<TOptions>(options));

        /// <summary>
        /// Registers the given factory to be used.
        /// </summary>
        /// <param name="factory">The factory function used to provide the options instance.</param>
        /// <returns>Configuration.</returns>
        public CommandConfiguration<TOptions> UseFactory(Func<TOptions> factory) =>
            Sink(new DelegateProvider<TOptions>(factory));

        /// <summary>
        /// Uses the default constructor provider for the options type.
        /// </summary>
        /// <returns>Configuration.</returns>
        /// <exception cref="Exception">No default constructor exists.</exception>
        public CommandConfiguration<TOptions> UseDefault() => Sink(ConstructorProvider<TOptions>.CreateOrThrow());
        
        // Sinks the provider
        private CommandConfiguration<TOptions> Sink(IProvider<TOptions> provider)
        {
            _componentSink.Sink(provider);
            return _configuration;
        }
    }
}