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
    /// Represents a converter that uses a nullable type's Parse method.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    internal class NullableTypeParseConverter<TValue> : DelegateConverter<TValue>
    {
        /// <inheritdoc />
        private NullableTypeParseConverter(Func<string, TValue> function) : base(function)
        {
        }

        /// <summary>
        /// Tries to create a new instance.
        /// </summary>
        internal static bool TryCreate(out IValueConverter<TValue>? converter)
        {
            converter = null;
            
            var nullableType = typeof(TValue);
            var underlyingValueType = nullableType.GetNullableUnderlyingType();

            if (underlyingValueType == null) return false;

            if (!TypeHelpers.TryGetParseMethodInfo(underlyingValueType, out var parseMethodInfo))
                return false;
            
            var strParamExpr = Expression.Parameter(typeof(string));
            var callParseExpr = Expression.Call(parseMethodInfo!, strParamExpr);
            var convertParseExpr = Expression.Convert(callParseExpr, nullableType);
            var callIsNullOrWsExpr = Expression.Call(TypeHelpers.StringIsNullOrWhiteSpaceMethodInfo, strParamExpr);
            var defaultTValueExpr = Expression.Default(nullableType);
            var conditionExpr = Expression.Condition(callIsNullOrWsExpr, defaultTValueExpr, convertParseExpr);
            var lambdaExpr = Expression.Lambda<Func<string, TValue>>(conditionExpr, strParamExpr);
            var function = lambdaExpr.Compile();

            converter = new NullableTypeParseConverter<TValue>(function);
            return true;
        }
        
        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => 
            $"string.IsNullOrWhiteSpace(<arg>) ? default({Formatting.FriendlyName(typeof(TValue))}) : " + 
            $"{Formatting.FriendlyName(typeof(TValue))}.Parse(string)";
    }
}