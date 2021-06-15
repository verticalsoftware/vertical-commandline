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
    /// Configuration object used to define an argument's conversion, validation and mapping
    /// properties.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    public class ArgumentConfiguration<TOptions, TValue> where TOptions : class
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="parserBuilder">Parser builder.</param>
        protected ArgumentConfiguration(IParserBuilder<TOptions, TValue> parserBuilder)
        {
            ParserBuilder = parserBuilder;
        }
        
        /// <summary>
        /// Gets the argument parser.
        /// </summary>
        protected IParserBuilder<TOptions, TValue> ParserBuilder { get; }
        
        /// <summary>
        /// Configures the argument.
        /// </summary>
        internal static ParserBuilder<TOptions, TValue> Configure(ParserBuilder<TOptions, TValue> parserBuilder,
            Action<ArgumentConfiguration<TOptions, TValue>> configureAction)
        {
            Check.NotNull(configureAction, nameof(configureAction));
            configureAction(new ArgumentConfiguration<TOptions, TValue>(parserBuilder));

            return parserBuilder;
        }
        
        /// <summary>
        /// Gets a configuration object used to define value mapping.
        /// </summary>
        public MappingConfiguration<TOptions, TValue> Map => new MappingConfiguration<TOptions, TValue>(
            this, ParserBuilder);
        
        /// <summary>
        /// Gets a configuration object used to define value conversion.
        /// </summary>
        public ConversionConfiguration<TOptions, TValue> Convert => new ConversionConfiguration<TOptions, TValue>(
            this, ParserBuilder);
        
        /// <summary>
        /// Gets a configuration object used to define validation.
        /// </summary>
        public ValidationConfiguration<TOptions, TValue> Validate => new ValidationConfiguration<TOptions, TValue>(
            this, ParserBuilder);
    }
}