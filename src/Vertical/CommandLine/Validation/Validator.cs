// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Infrastructure;

namespace Vertical.CommandLine.Validation
{
    /// <summary>
    /// Exposes a combine method.
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// Appends a validator to the current instance.
        /// </summary>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="first">The instance to append.</param>
        /// <param name="second">The validator to append.</param>
        /// <returns><see cref="IValidator{TValue}"/></returns>
        internal static IValidator<TValue> Combine<TValue>(IValidator<TValue> first, IValidator<TValue> second)
        {
            // Both can't be null
            Check.NotNull(first ?? second, nameof(first));

            // If one is null return first non-null
            if ((first == null) != (second == null)) return first ?? second;

            // If first is composite then append
            if (!(first is CompositeValidator<TValue> composite))
                return new CompositeValidator<TValue>(first, second);

            composite.Append(second);

            return composite;
        }

        /// <summary>
        /// Performs contextual validation.
        /// </summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="validator">Validator instance</param>
        /// <param name="context">Context, template or position argument</param>
        /// <param name="value">Value</param>
        internal static void Validate<TValue>(IValidator<TValue> validator, string context, TValue value)
        {
            if (validator == null) return;

            bool valid;

            try
            {
                valid = validator.Validate(value);
            }
            catch (Exception ex)
            {
                // Exception thrown in logic
                throw new ConfigurationException(context, ex);
            }

            if (!valid)
            {
                throw new ValidationException(validator.GetError(value), context, value);
            }
        }
    }

    /// <summary>
    /// Serves as a base type for validators.
    /// </summary>
    /// <typeparam name="TState">State object type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    public abstract class Validator<TState, TValue> : IValidator<TValue>
    {
        private readonly MessageFormat<TState, TValue> _messageFormatter;
        private readonly string _description;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="state">Object used to compare values to.</param>
        /// <param name="messageFormatter">Function used to format messages to display when validation
        /// fails.</param>
        /// <param name="description">Debug description.</param>
        internal Validator(TState state, MessageFormat<TState, TValue> messageFormatter, string description)
        {
            State = state;
            _messageFormatter = messageFormatter ?? ((_, value) => Exceptions.DefaultValidationMessage(value));
            _description = description;
        }

        /// <summary>
        /// Gets the valid state used in comparison.
        /// </summary>
        protected TState State { get; }

        /// <inheritdoc />
        public abstract bool Validate(TValue value);

        /// <inheritdoc />
        public string GetError(TValue value) => _messageFormatter(State, value);

        /// <inheritdoc />
        public override string ToString() => _description;        
    }
}