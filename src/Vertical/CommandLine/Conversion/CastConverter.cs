// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Vertical.CommandLine.Infrastructure;

namespace Vertical.CommandLine.Conversion
{
    /// <summary>
    /// Represents a converter that casts values.
    /// </summary>
    /// <typeparam name="TValue">Value type.</typeparam>
    internal sealed class CastConverter<TValue> : DelegateConverter<TValue>
    {
        /// <inheritdoc />
        private CastConverter(Func<string, TValue> function) : base(function)
        {
        }

        /// <summary>
        /// Attempts to create an instance.
        /// </summary>
        internal static bool TryCreate(out IValueConverter<TValue>? converter)
        {
            converter = null;

            var conversionOperatorInfo =
                typeof(TValue).GetMethod(Common.OpExplicitMethodName, new[] { typeof(string) }) ??
                typeof(TValue).GetMethod(Common.OpImplicitMethodName, new[] { typeof(string) });

            if (conversionOperatorInfo == null) return false;

            var strParamExpr = Expression.Parameter(typeof(string));
            var callOperatorExpr = Expression.Call(conversionOperatorInfo, strParamExpr);
            var lambdaExpr = Expression.Lambda<Func<string, TValue>>(callOperatorExpr, strParamExpr);
            var function = lambdaExpr.Compile();

            converter = new CastConverter<TValue>(function);
            return true;
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => $"({Formatting.FriendlyName(typeof(TValue))}) <string>";
    }
}