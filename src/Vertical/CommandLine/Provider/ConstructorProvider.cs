// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Vertical.CommandLine.Infrastructure;

namespace Vertical.CommandLine.Provider
{
    /// <summary>
    /// Represents a provider that uses the type's default constructor.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    internal sealed class ConstructorProvider<TOptions> : DelegateProvider<TOptions>
        where TOptions : class
    {
        private ConstructorProvider(Func<TOptions> function) : base(function)
        {
        }

        /// <summary>
        /// Creates an instance, or throws if it can't.
        /// </summary>
        internal static IProvider<TOptions> CreateOrThrow()
        {
            var constructorInfo = typeof(TOptions).GetConstructor(Array.Empty<Type>());

            if (constructorInfo == null)
            {
                throw ConfigurationExceptions.NoDefaultOptionsConstructor<TOptions>();
            }

            var callCtorExpr = Expression.New(constructorInfo);
            var lambda = Expression.Lambda<Func<TOptions>>(callCtorExpr);
            var function = lambda.Compile();
            
            return new ConstructorProvider<TOptions>(function);
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => $"new {Formatting.FriendlyName(typeof(TOptions))}()";
    }
}