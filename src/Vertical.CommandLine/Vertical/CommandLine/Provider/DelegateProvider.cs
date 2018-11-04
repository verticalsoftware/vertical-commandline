// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Diagnostics.CodeAnalysis;
using Vertical.CommandLine.Infrastructure;

namespace Vertical.CommandLine.Provider
{
    /// <summary>
    /// Represents a provider that uses an underlying delegate to provider the options.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    internal class DelegateProvider<TOptions> : IProvider<TOptions>
        where TOptions : class
    {
        private readonly Func<TOptions> _function;

        internal DelegateProvider(Func<TOptions> function)
        {
            _function = function ?? throw new ArgumentNullException(nameof(function));
        }

        /// <inheritdoc />
        public TOptions GetInstance() => _function();

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => $"Func<{Formatting.FriendlyName(typeof(TOptions))}>";
    }
}