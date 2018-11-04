// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Vertical.CommandLine.Mapping;

namespace Vertical.CommandLine.Configuration
{
    /// <summary>
    /// Configuration object used to define multiple value mapping.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    public sealed class MultiValueMappingConfiguration<TOptions, TValue> :
        MappingConfiguration<TOptions, TValue>
        where TOptions : class
    {
        /// <inheritdoc />
        internal MultiValueMappingConfiguration(MultiValueArgumentConfiguration<TOptions, TValue> configuration,
            IComponentSink<IMapper<TOptions, TValue>> mapperSink) 
            : base(configuration, mapperSink)
        {
        }

        /// <summary>
        /// Maps one or more argument values to the collection specified by the expression.
        /// </summary>
        /// <param name="expression">Expression that identifies the collection.</param>
        /// <returns>Configuration.</returns>
        public MultiValueArgumentConfiguration<TOptions, TValue> ToCollection(
            Expression<Func<TOptions, ICollection<TValue>>> expression)
        {
            Using(CollectionMapper<TOptions, TValue>.Create(expression));
            return Configuration;
        }

        /// <summary>
        /// Maps one or more argument values to the stack specified by the expression.
        /// </summary>
        /// <param name="expression">Expression that identifies the collection.</param>
        /// <returns>Configuration.</returns>
        public MultiValueArgumentConfiguration<TOptions, TValue> ToStack(
            Expression<Func<TOptions, Stack<TValue>>> expression)
        {
            Using(StackMapper<TOptions, TValue>.Create(expression));
            return Configuration;
        }
        
        /// <summary>
        /// Maps one or more argument values to the queue specified by the expression.
        /// </summary>
        /// <param name="expression">Expression that identifies the collection.</param>
        /// <returns>Configuration.</returns>
        public MultiValueArgumentConfiguration<TOptions, TValue> ToQueue(
            Expression<Func<TOptions, Queue<TValue>>> expression)
        {
            Using(QueueMapper<TOptions, TValue>.Create(expression));
            return Configuration;
        }

        /// <summary>
        /// Maps one or more argument values to the queue specified by the expression.
        /// </summary>
        /// <param name="expression">Expression that identifies the collection.</param>
        /// <returns>Configuration.</returns>
        public MultiValueArgumentConfiguration<TOptions, TValue> ToSet(
            Expression<Func<TOptions, ISet<TValue>>> expression)
        {
            Using(SetMapper<TOptions, TValue>.Create(expression));
            return Configuration;
        }
        
        /// <inheritdoc />
        protected override bool MultiValued => true;

        // Returns configuration.
        private new MultiValueArgumentConfiguration<TOptions, TValue> Configuration =>
            (MultiValueArgumentConfiguration<TOptions, TValue>) base.Configuration;
    }
}