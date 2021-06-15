// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Vertical.CommandLine.Infrastructure;
using Vertical.CommandLine.Validation;

namespace Vertical.CommandLine.Configuration
{
    /// <summary>
    /// Configuration object used to define how values are validated after conversion.
    /// </summary>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <typeparam name="TOptions">Options type.</typeparam>
    public sealed class ValidationConfiguration<TOptions, TValue> where TOptions : class
    {
        private readonly ArgumentConfiguration<TOptions, TValue> _configuration;
        private readonly IComponentSink<IValidator<TValue>> _componentSink;

        internal ValidationConfiguration(ArgumentConfiguration<TOptions, TValue> configuration,
            IComponentSink<IValidator<TValue>> componentSink)
        {
            _configuration = configuration;
            _componentSink = componentSink;
        }

        /// <summary>
        /// Uses the given predicate to check the converted value.
        /// </summary>
        /// <param name="predicate">Predicate that returns a boolean indicating if the value is valid.</param>
        /// <param name="messageFormat">A function that formats the error to display if validation fails.</param>
        /// <returns>Configuration.</returns>
        public ArgumentConfiguration<TOptions, TValue> Using(Predicate<TValue> predicate,
            Func<TValue, string>? messageFormat = null)
        {
            return Using(new DelegateValidator<object, TValue>(null!, (_, arg) => predicate(arg),
                (_, arg) => messageFormat?.Invoke(arg) ?? Common.InvalidValue,
                $"Predicate<{Formatting.FriendlyName(typeof(TValue))}>"));
        }

        /// <summary>
        /// Uses the given predicate to check the converted value.
        /// </summary>
        /// <param name="predicate">Predicate that returns a boolean indicating if the value is valid.</param>
        /// <param name="state">A state object used in the predicate.</param>
        /// <param name="messageFormat">A function that formats the error to display if validation fails.</param>
        /// <returns>Configuration.</returns>
        public ArgumentConfiguration<TOptions, TValue> Using<TState>(TState state,
            Validation<TState, TValue> predicate,
            MessageFormat<TState, TValue>? messageFormat = null)
        {
            return Using(new DelegateValidator<TState, TValue>(state, predicate,
                messageFormat ?? ((_,__) => Common.InvalidValue),
                $"Predicate<{Formatting.FriendlyName(typeof(TValue))}>"));
        }

        /// <summary>
        /// Validates that the argument value is less than the given value.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="comparer">The comparer implementation to use.</param>
        /// <param name="messageFormat">A function that formats the error to display if validation fails.</param>
        /// <returns>Configuration.</returns>
        public ArgumentConfiguration<TOptions, TValue> Less(TValue value,
            IComparer<TValue>? comparer = null,
            MessageFormat<TValue, TValue>? messageFormat = null)
        {
            return Using(new DelegateValidator<TValue, TValue>(value,
                (state, arg) => (comparer ?? Comparer<TValue>.Default).Compare(state, arg) > 0,
                messageFormat ?? ((state, _) => Common.LessThanMessage(state)),
                $"<arg> < {value}"));
        }
        
        /// <summary>
        /// Validates that the argument value is less or equal to the given value.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="comparer">The comparer implementation to use.</param>
        /// <param name="messageFormat">A function that formats the error to display if validation fails.</param>
        /// <returns>Configuration.</returns>
        public ArgumentConfiguration<TOptions, TValue> LessOrEqual(TValue value,
            IComparer<TValue>? comparer = null,
            MessageFormat<TValue, TValue>? messageFormat = null)
        {
            return Using(new DelegateValidator<TValue, TValue>(value,
                (state, arg) => (comparer ?? Comparer<TValue>.Default).Compare(state, arg) >= 0,
                messageFormat ?? ((state, _) => Common.LessOrEqualMessage(state)),
                $"<arg> <= {value}"));
        }
        
        /// <summary>
        /// Validates that the argument value is greater than the given value.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="comparer">The comparer implementation to use.</param>
        /// <param name="messageFormat">A function that formats the error to display if validation fails.</param>
        /// <returns>Configuration.</returns>
        public ArgumentConfiguration<TOptions, TValue> Greater(TValue value,
            IComparer<TValue>? comparer = null,
            MessageFormat<TValue, TValue>? messageFormat = null)
        {
            return Using(new DelegateValidator<TValue, TValue>(value,
                (state, arg) => (comparer ?? Comparer<TValue>.Default).Compare(state, arg) < 0,
                messageFormat ?? ((state, _) => Common.GreaterThanMessage(state)),
                $"<arg> > {value}"));
        }
        
        /// <summary>
        /// Validates that the argument value is greater or equal to the given value.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="comparer">The comparer implementation to use.</param>
        /// <param name="messageFormat">A function that formats the error to display if validation fails.</param>
        /// <returns>Configuration.</returns>
        public ArgumentConfiguration<TOptions, TValue> GreaterOrEqual(TValue value,
            IComparer<TValue>? comparer = null,
            MessageFormat<TValue, TValue>? messageFormat = null)
        {
            return Using(new DelegateValidator<TValue, TValue>(value,
                (state, arg) => (comparer ?? Comparer<TValue>.Default).Compare(state, arg) <= 0,
                messageFormat ?? ((state, _) => Common.GreaterEqualMessage(state)),
                $"<arg> >= {value}"));
        }

        /// <summary>
        /// Validates that the argument value is inclusively between the given minimum and maximum values.
        /// </summary>
        /// <param name="min">The minimum acceptable value.</param>
        /// <param name="max">The maximum acceptable value.</param>
        /// <param name="comparer">The comparer implementation to use.</param>
        /// <param name="messageFormat">A function that formats the error to display if validation fails.</param>
        /// <returns>Configuration.</returns>
        public ArgumentConfiguration<TOptions, TValue> Between(TValue min, TValue max,
            IComparer<TValue>? comparer = null,
            MessageFormat<TValue, TValue>? messageFormat = null)
        {
            GreaterOrEqual(min, comparer, messageFormat);
            return LessOrEqual(max, comparer, messageFormat);
        }

        /// <summary>
        /// Validates that an argument value is found in the given set of values.
        /// </summary>
        /// <param name="values">Acceptable values.</param>
        /// <param name="comparer">The comparer implementation to use.</param>
        /// <param name="messageFormat">A function that formats the error to display if validation fails.</param>
        /// <returns>Configuration.</returns>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public ArgumentConfiguration<TOptions, TValue> In(IEnumerable<TValue> values,
            IEqualityComparer<TValue>? comparer = null,
            MessageFormat<ISet<TValue>, TValue>? messageFormat = null)
        {
            return Using(new DelegateValidator<ISet<TValue>, TValue>(new HashSet<TValue>(values,
                    comparer ?? EqualityComparer<TValue>.Default),
                (state, arg) => state.Contains(arg),
                messageFormat ?? ((state, _) => Common.OneOfMessage(state)),
                $"<arg> in {string.Join(", ", values)}"));
        }

        /// <summary>
        /// Validates that an argument value is found in the given set of values.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="messageFormat">A function that formats the error to display if validation fails.</param>
        /// <returns>Configuration.</returns>
        public ArgumentConfiguration<TOptions, TValue> Matches(string pattern,
            MessageFormat<Regex, TValue>? messageFormat = null)
        {
            return Using(new DelegateValidator<Regex, TValue>(new Regex(pattern),
                (state, arg) => state.IsMatch(arg?.ToString() ?? string.Empty),
                messageFormat ?? ((state, _) => Common.PatternMessage(state)),
                $"Regex.IsMatch({pattern})"));
        }

        /// <summary>
        /// Uses the given validator instance to check the converted value.
        /// </summary>
        private ArgumentConfiguration<TOptions, TValue> Using(IValidator<TValue> validator)
        {
            _componentSink.Sink(validator);
            return _configuration;
        }
    }
}