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
    public sealed class SetMapper<TOptions, TValue> : CollectionMapper<TOptions, TValue>
        where TOptions : class
    {
        /// <inheritdoc />
        private SetMapper(Action<TOptions, TValue> action, string propertyName) : base(action, propertyName)
        {
        }

        internal static IMapper<TOptions, TValue> Create(Expression<Func<TOptions, ISet<TValue>>> expression)
        {
            var addMethodInfo = typeof(ISet<TValue>).GetKnownMethodInfo(nameof(ISet<TValue>.Add),
                new[] {typeof(TValue)}, typeof(bool))!;

            var action = ExpressionHelpers.CreateCollectionWriter<TOptions, ISet<TValue>, TValue>(
                expression, addMethodInfo, out var propertyName);

            return new SetMapper<TOptions, TValue>(action, propertyName);
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        protected override string CollectionType => $"ISet<{typeof(TValue).Name}>";

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => Formatting.FriendlyName(typeof(TOptions)) +
                                             $".{PropertyName}.Add(<{Formatting.FriendlyName(typeof(TValue))}>)";
    }
}