// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Vertical.CommandLine.Infrastructure;

namespace Vertical.CommandLine.Mapping
{
    /// <summary>
    /// Represents an object that maps values using a property expression.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    public sealed class PropertyMapper<TOptions, TValue> : DelegateMapper<TOptions, TValue>
        where TOptions : class
    {
        private readonly string _propertyName;

        /// <inheritdoc />
        private PropertyMapper(Action<TOptions, TValue> action, string propertyName) : base(action, false)
        {
            _propertyName = propertyName;
        }

        /// <inheritdoc />
        public override void MapValue(TOptions options, TValue value)
        {
            try
            {
                MapValueCore(options, value);
            }
            catch(Exception ex)
            {
                throw ConfigurationExceptions.ErrorInPropertyMapping<TOptions>(
                    _propertyName, ex);
            }
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        internal static IMapper<TOptions, TValue> Create(Expression<Func<TOptions, TValue>> expression)
        {
            var action = ExpressionHelpers.CreatePropertyWriter(expression, out var propertyName);

            return new PropertyMapper<TOptions, TValue>(action, propertyName);
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => $"{Formatting.FriendlyName(typeof(TOptions))}.{_propertyName} = " +
                                             $"<{Formatting.FriendlyName(typeof(TValue))}>";
    }
}