// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Linq.Expressions;
using Vertical.CommandLine.Infrastructure;
using Vertical.CommandLine.Mapping;

namespace Vertical.CommandLine.Configuration
{
    /// <summary>
    /// Configuration object used to define property mapping.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    public class MappingConfiguration<TOptions, TValue> where TOptions : class
    {
        private readonly IComponentSink<IMapper<TOptions, TValue>> _mapperSink;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        /// <param name="mapperSink">Mapper sink component.</param>
        internal MappingConfiguration(ArgumentConfiguration<TOptions, TValue> configuration,
            IComponentSink<IMapper<TOptions, TValue>> mapperSink)
        {
            Configuration = configuration;
            _mapperSink = mapperSink;
        }
        
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        protected ArgumentConfiguration<TOptions, TValue> Configuration { get; }
        
        /// <summary>
        /// Maps an argument value to the property specified by the expression.
        /// </summary>
        /// <param name="expression">The expression that identifies the property.</param>
        /// <returns>Configuration.</returns>
        public ArgumentConfiguration<TOptions, TValue> ToProperty(Expression<Func<TOptions, TValue>> expression) =>
            Using(PropertyMapper<TOptions, TValue>.Create(expression));
        
        /// <summary>
        /// Maps an argument value using the given mapper instance.
        /// </summary>
        /// <param name="mapper">The mapper instance.</param>
        /// <returns>Configuration.</returns>
        public ArgumentConfiguration<TOptions, TValue> Using(IMapper<TOptions, TValue> mapper)
        {
            Check.NotNull(mapper, nameof(mapper));
            
            _mapperSink.Sink(mapper);
            return Configuration;
        }

        /// <summary>
        /// Maps an argument value using the given delegate.
        /// </summary>
        /// <param name="action">The action that maps the value to the options instance.</param>
        /// <returns>Configuration.</returns>
        public ArgumentConfiguration<TOptions, TValue> Using(Action<TOptions, TValue> action) =>
            Using(new DelegateMapper<TOptions, TValue>(action, MultiValued));

        /// <summary>
        /// Gets whether the configuration is multi-valued.
        /// </summary>
        protected virtual bool MultiValued => false;
    }
}