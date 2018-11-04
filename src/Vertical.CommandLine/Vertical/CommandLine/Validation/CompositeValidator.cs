// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Linq;

namespace Vertical.CommandLine.Validation
{
    /// <summary>
    /// Represents a validator composed of multiple other validators.
    /// </summary>
    internal sealed class CompositeValidator<TValue> : IValidator<TValue>
    {
        private readonly ICollection<IValidator<TValue>> _subValidators;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="validators">Validators to include in the composition.</param>
        internal CompositeValidator(params IValidator<TValue>[] validators)
        {
            _subValidators = validators?.ToList() ?? throw new ArgumentNullException(nameof(validators));
        }

        /// <inheritdoc />
        public bool Validate(TValue value)
        {
            return _subValidators.Aggregate(true, (result, validator) => result && validator.Validate(value));
        }

        /// <inheritdoc />
        public string GetError(TValue value)
        {
            return _subValidators.First(validator => !validator.Validate(value)).GetError(value);
        }

        /// <summary>
        /// Appends a validator.
        /// </summary>
        /// <param name="validator">Validator to append.</param>
        internal void Append(IValidator<TValue> validator)
        {
            _subValidators.Add(validator);
        }
    }
}
