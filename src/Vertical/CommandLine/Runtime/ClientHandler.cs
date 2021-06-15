// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Vertical.CommandLine.Infrastructure;

namespace Vertical.CommandLine.Runtime
{
    /// <summary>
    /// Represents a client handler.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    internal sealed class ClientHandler<TOptions> where TOptions : class
    {
        private readonly Action<TOptions> _syncHandler;
        private readonly Func<TOptions, Task> _asyncHandler;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="handler">Client defined handler delegate.</param>
        internal ClientHandler(Action<TOptions> handler) => _syncHandler = handler ?? 
            throw new ArgumentNullException(nameof(handler));

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="handler">Client defined asynchronous handler.</param>
        internal ClientHandler(Func<TOptions, Task> handler) => _asyncHandler = handler ??
            throw new ArgumentNullException(nameof(handler));

        /// <summary>
        /// Invokes the handler synchronously.
        /// </summary>
        /// <param name="options">Options instance.</param>
        public void Invoke(TOptions options)
        {
            if (_syncHandler == null)
            {
                _asyncHandler(options).GetAwaiter().GetResult();
                return;
            }

            _syncHandler(options);
        }

        /// <summary>
        /// Gets whether the handler is async.
        /// </summary>
        public bool IsAsync => _asyncHandler != null;

        /// <summary>
        /// Invokes the handler asynchronously.
        /// </summary>
        /// <param name="options">Options instance.</param>
        /// <returns>Task</returns>
        public Task InvokeAsync(TOptions options)
        {
            if (_asyncHandler == null)
            {
                _syncHandler(options);
                return Task.CompletedTask;
            }

            return _asyncHandler(options);
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return _syncHandler != null
                ? Formatting.FriendlyName(typeof(Action<TOptions>))
                : Formatting.FriendlyName(typeof(Func<TOptions, Task>));
        }
    }
}