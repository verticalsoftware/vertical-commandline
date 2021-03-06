// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Vertical.CommandLine.Infrastructure;

namespace Vertical.CommandLine.Mapping
{
    /// <summary>
    /// Represents an object that maps values to a stack.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    public sealed class StackMapper<TOptions, TValue> : CollectionMapper<TOptions, TValue>
        where TOptions : class
    {
        /// <inheritdoc />
        private StackMapper(Action<TOptions, TValue> action, string propertyName) : base(action, propertyName)
        {
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        protected override string CollectionType => $"Stack<{typeof(TValue).Name}>";

        /// <summary>
        /// Creates an instance.
        /// </summary>
        internal static IMapper<TOptions, TValue> Create(Expression<Func<TOptions, Stack<TValue>>> expression)
        {
            var pushMethodInfo = typeof(Stack<TValue>).GetKnownMethodInfo(nameof(Stack<TValue>.Push),
                new[] {typeof(TValue)}, typeof(void))!;

            var action = ExpressionHelpers.CreateCollectionWriter<TOptions, Stack<TValue>, TValue>(
                expression, pushMethodInfo, out var propertyName);

            return new StackMapper<TOptions, TValue>(action, propertyName);
        }
        
        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => Formatting.FriendlyName(typeof(TOptions)) + 
                                             $".{PropertyName}.Push(<{Formatting.FriendlyName(typeof(TValue))}>)";
    }
}