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
    /// Represents an object that converts string to nullable enum.
    /// </summary>
    internal class NullableEnumConverter<TValue> : DelegateConverter<TValue>
    {
        /// <inheritdoc />
        private NullableEnumConverter(Func<string, TValue> function) : base(function)
        {
        }

        /// <summary>
        /// Tries to create an instance.
        /// </summary>
        internal static bool TryCreate(out IValueConverter<TValue> converter)
        {
            converter = null;

            var nullableType = typeof(TValue);

            if (!nullableType.IsNullableType())
                return false;

            var underlyingType = nullableType.GetGenericArguments()[0];

            if (!underlyingType.IsEnum)
                return false;

            var strParamExpr = Expression.Parameter(typeof(string), "str");
            var constTypeExpr = Expression.Constant(underlyingType);
            var trueExpr = Expression.Constant(true);
            var callParseExpr = Expression.Call(TypeHelpers.GetEnumParseMethodInfo(), constTypeExpr, strParamExpr, trueExpr);
            var convertExpr = Expression.Convert(callParseExpr, nullableType);
            var defaultExpr = Expression.Default(nullableType);
            var callIsNullOrWsExpr = Expression.Call(TypeHelpers.StringIsNullOrWhiteSpaceMethodInfo, strParamExpr);
            var conditionExpr = Expression.Condition(callIsNullOrWsExpr, defaultExpr, convertExpr);
            var lambdaExpr = Expression.Lambda<Func<string, TValue>>(conditionExpr, strParamExpr);
            var function = lambdaExpr.Compile();
            
            converter = new NullableEnumConverter<TValue>(value =>
            {
                try
                {
                    return function(value);
                }
                catch (ArgumentException)
                {
                    throw Exceptions.EnumConversionFailed(underlyingType, value);
                }
            });
            return true;
        }
        
        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => 
            $"string.IsNullOrWhiteSpace(<arg>) ? default({Formatting.FriendlyName(typeof(TValue))}) : " + 
            $"Enum.Parse(typeof({Formatting.FriendlyName(typeof(TValue))}), <arg>)";
    }
}