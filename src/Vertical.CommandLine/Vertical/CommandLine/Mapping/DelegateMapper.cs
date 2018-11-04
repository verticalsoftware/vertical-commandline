// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Diagnostics.CodeAnalysis;
using Vertical.CommandLine.Infrastructure;

namespace Vertical.CommandLine.Mapping
{
    /// <summary>
    /// Represents an object that maps converted argument values to the options type.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    public class DelegateMapper<TOptions, TValue> : IMapper<TOptions, TValue>
        where TOptions : class
    {
        private readonly Action<TOptions, TValue> _action;

        internal DelegateMapper(Action<TOptions, TValue> action, bool multiValued)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            MultiValued = multiValued;
        }

        /// <inheritdoc />
        public virtual void MapValue(TOptions options, TValue value)
        {
            try
            {
                MapValueCore(options, value);
            }
            catch (NullReferenceException ex)
            {
                throw ConfigurationExceptions.NullReferenceInMapping<TOptions>(ex);
            }
            catch (Exception ex)
            {
                throw ConfigurationExceptions.ErrorInDelegateMapping(ex);
            }
        }

        /// <summary>
        /// Invokes the underlying delegate.
        /// </summary>
        /// <param name="options">Options</param>
        /// <param name="value">Value to map</param>
        protected void MapValueCore(TOptions options, TValue value)
        {
            _action(options, value);
        }

        /// <inheritdoc />
        public bool MultiValued { get; }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => $"Action<{Formatting.FriendlyName(typeof(TOptions))}, " +
                                             $"{Formatting.FriendlyName(typeof(TValue))}>";
    }
}