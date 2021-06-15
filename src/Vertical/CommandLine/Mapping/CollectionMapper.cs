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
    /// Represents an object that maps values to a collection.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    public class CollectionMapper<TOptions, TValue> : DelegateMapper<TOptions, TValue>
        where TOptions : class
    {
        /// <inheritdoc />
        protected CollectionMapper(Action<TOptions, TValue> action, string propertyName)
            : base(action, multiValued: true)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Gets the property name that identifies the collection.
        /// </summary>
        [ExcludeFromCodeCoverage]
        protected string PropertyName { get; }

        /// <summary>
        /// Gets the collection type for debugging information.
        /// </summary>
        [ExcludeFromCodeCoverage]
        protected virtual string CollectionType => $"ICollection<{typeof(TValue).Name}>";

        /// <inheritdoc />
        public override void MapValue(TOptions options, TValue value)
        {
            try
            {
                MapValueCore(options, value);
            }
            catch (NullReferenceException ex)
            {
                throw ConfigurationExceptions.NullReferenceInCollectionMapping<TOptions>(
                    CollectionType, PropertyName, ex);
            }
            catch (Exception ex)
            {
                throw ConfigurationExceptions.ErrorInCollectionMapping<TOptions>(
                    CollectionType, PropertyName, ex);
            }
        }

        // Create the delegate
        internal static IMapper<TOptions, TValue> Create(Expression<Func<TOptions, ICollection<TValue>>> expression)
        {
            var addMethodInfo = typeof(ICollection<TValue>).GetKnownMethodInfo(nameof(ICollection<TValue>.Add),
                new[] {typeof(TValue)}, typeof(void));

            var action = ExpressionHelpers.CreateCollectionWriter<TOptions, ICollection<TValue>, TValue>(
                expression, addMethodInfo!, out var propertyName);

            return new CollectionMapper<TOptions, TValue>(action, propertyName);
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => Formatting.FriendlyName(typeof(TOptions)) +
                                             $".{PropertyName}.Add(<{Formatting.FriendlyName(typeof(TValue))}>)";
    }
}