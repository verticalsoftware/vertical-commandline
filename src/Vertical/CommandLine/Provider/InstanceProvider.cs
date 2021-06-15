// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Diagnostics.CodeAnalysis;

namespace Vertical.CommandLine.Provider
{
    /// <summary>
    /// Represents an object that returns an underlying instance.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    internal sealed class InstanceProvider<TOptions> : IProvider<TOptions>
        where TOptions : class
    {
        private readonly TOptions _instance;

        internal InstanceProvider(TOptions instance)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        /// <inheritdoc />
        public TOptions GetInstance() => _instance;

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => _instance.ToString()!;
    }
}