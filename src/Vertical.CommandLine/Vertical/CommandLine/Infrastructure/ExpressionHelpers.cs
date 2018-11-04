// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Vertical.CommandLine.Infrastructure
{
    /// <summary>
    /// Functional class with expression helpers.
    /// </summary>
    internal static class ExpressionHelpers
    {
        /// <summary>
        /// Gets PropertyInfo from an expression.
        /// </summary>
        internal static PropertyInfo GetPropertyInfo<TOptions, TValue>(Expression<Func<TOptions, TValue>> expression,
            bool checkWrite)
        {
            if (!(expression.Body is MemberExpression propertyExpr)) 
                throw ConfigurationExceptions.NotAPropertyExpression(expression);

            if (!(propertyExpr.Member is PropertyInfo propertyInfo))
                throw ConfigurationExceptions.NotAPropertyExpression(expression);

            if (checkWrite && !propertyInfo.CanWrite)
                throw ConfigurationExceptions.NotWriteableProperty(propertyInfo);

            return propertyInfo;
        }

        /// <summary>
        /// Creates a property writer delegate.
        /// </summary>
        internal static Action<TOptions, TValue> CreatePropertyWriter<TOptions, TValue>(
            Expression<Func<TOptions, TValue>> expression,
            out string propertyName)
        {
            Check.NotNull(expression, nameof(expression));

            var propertyInfo = GetPropertyInfo(expression, checkWrite: true);
            var valueParamExpr = Expression.Parameter(typeof(TValue));
            var optionParamExpr = Expression.Parameter(typeof(TOptions));
            var propertyExpr = Expression.Property(optionParamExpr, propertyInfo);
            var assignExpr = Expression.Assign(propertyExpr, valueParamExpr);
            var lambdaExpr = Expression.Lambda<Action<TOptions, TValue>>(assignExpr, optionParamExpr, valueParamExpr);
            var action = lambdaExpr.Compile();

            propertyName = propertyInfo.Name;

            return action;
        }

        /// <summary>
        /// Creates a collection writer delegate.
        /// </summary>
        internal static Action<TOptions, TValue> CreateCollectionWriter<TOptions, TCollection, TValue>(
            Expression<Func<TOptions, TCollection>> expression,
            MethodInfo addMethodInfo,
            out string propertyName)
        {
            Check.NotNull(expression, nameof(expression));

            var propertyInfo = GetPropertyInfo(expression, checkWrite: false);
            var optionsParamExpr = Expression.Parameter(typeof(TOptions));
            var valueParamExpr = Expression.Parameter(typeof(TValue));
            var propertyExpr = Expression.Property(optionsParamExpr, propertyInfo);
            var callAddExpr = Expression.Call(propertyExpr, addMethodInfo, valueParamExpr);
            var lambdaExpr = Expression.Lambda<Action<TOptions, TValue>>(callAddExpr, optionsParamExpr, valueParamExpr);
            var action = lambdaExpr.Compile();

            propertyName = propertyInfo.Name;

            return action;
        }
    }
}