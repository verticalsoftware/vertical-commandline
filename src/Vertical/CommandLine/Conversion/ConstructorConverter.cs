// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Linq.Expressions;
using Vertical.CommandLine.Infrastructure;

namespace Vertical.CommandLine.Conversion
{
    /// <summary>
    /// Represents a converter that uses a type's constructor.
    /// </summary>
    /// <typeparam name="TValue">Value</typeparam>
    internal sealed class ConstructorConverter<TValue> : DelegateConverter<TValue>
    {
        /// <inheritdoc />
        private ConstructorConverter(Func<string, TValue> function) : base(function)
        {
        }

        /// <summary>
        /// Tries to create an instance.
        /// </summary>
        internal static bool TryCreate(out IValueConverter<TValue> converter)
        {
            converter = null;
            var valueType = typeof(TValue);

            if (!TypeHelpers.TryGetStringConstructor(valueType, out var constructor))
                return false;

            var strParamExpr = Expression.Parameter(typeof(string));
            var newExpr = Expression.New(constructor, strParamExpr);
            var lambdaExpr = Expression.Lambda<Func<string, TValue>>(newExpr, strParamExpr);
            var function = lambdaExpr.Compile();

            converter = new ConstructorConverter<TValue>(function);
            return true;
        }
    }
}