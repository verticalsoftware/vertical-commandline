// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using Vertical.CommandLine.Infrastructure;
using Vertical.CommandLine.Parsing;

namespace Vertical.CommandLine.Configuration
{
    /// <summary>
    /// Configuration object used to define a multi-valued argument.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    public sealed class MultiValueArgumentConfiguration<TOptions, TValue> :
        ArgumentConfiguration<TOptions, TValue>
        where TOptions : class
    {
        private MultiValueArgumentConfiguration(IParserBuilder<TOptions, TValue> parserBuilder) : base(parserBuilder)
        {
        }

        /// <summary>
        /// Invokes configuration.
        /// </summary>
        /// <param name="parserBuilder">Parser builder.</param>
        /// <param name="configureAction">Configuration action.</param>
        internal static ParserBuilder<TOptions, TValue> Configure(ParserBuilder<TOptions, TValue> parserBuilder,
            Action<MultiValueArgumentConfiguration<TOptions, TValue>> configureAction)
        {
            Check.NotNull(configureAction, nameof(configureAction));
            configureAction(new MultiValueArgumentConfiguration<TOptions, TValue>(parserBuilder));
            return parserBuilder;
        }
        
        /// <summary>
        /// Gets a configuration object used to map a multi-valued argument or option.
        /// </summary>
        public MultiValueMappingConfiguration<TOptions, TValue> MapMany =>
            new MultiValueMappingConfiguration<TOptions, TValue>(this, ParserBuilder);
    }
}