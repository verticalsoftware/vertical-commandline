// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;

namespace Vertical.CommandLine.Validation
{
    /// <summary>
    /// Validates values using an underlying delegate.
    /// </summary>
    internal sealed class DelegateValidator<TState, TValue> : Validator<TState, TValue>
    {
        private readonly Validation<TState, TValue> _function;

        /// <inheritdoc />
        internal DelegateValidator(TState state, Validation<TState, TValue> function,
            MessageFormat<TState, TValue> messageFormatter,
            string description) :
            base(state, messageFormatter, description)
        {
            _function = function ?? throw new ArgumentNullException(nameof(function));
        }

        /// <inheritdoc />
        public override bool Validate(TValue value) => _function(State, value);
    }
}