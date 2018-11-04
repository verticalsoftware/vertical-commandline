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
    /// Represents a converter that uses a type's Parse method.
    /// </summary>
    /// <typeparam name="TValue">Value type.</typeparam>
    internal sealed class ParseConverter<TValue> : DelegateConverter<TValue>
    {
        /// <inheritdoc />
        private ParseConverter(Func<string, TValue> function) : base(function)
        {
        }
        
        /// <summary>
        /// Tries to create an instance.
        /// </summary>
        internal static bool TryCreate(out IValueConverter<TValue> converter)
        {
            converter = null;
            var valueType = typeof(TValue);

            if (!TypeHelpers.TryGetParseMethodInfo(valueType, out var parseMethodInfo))
                return false;

            var strParamExpr = Expression.Parameter(typeof(string));
            var callParseExpr = Expression.Call(parseMethodInfo, strParamExpr);
            var lambdaExpr = Expression.Lambda<Func<string, TValue>>(callParseExpr, strParamExpr);
            var function = lambdaExpr.Compile();

            converter = new ParseConverter<TValue>(function);
            return true;
        }
        
        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => $"{Formatting.FriendlyName(typeof(TValue))}.Parse(<arg>)";
    }
}