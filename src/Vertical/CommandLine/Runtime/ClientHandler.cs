// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
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
        private readonly Action<TOptions>? _syncHandler;
        private readonly Func<TOptions, CancellationToken, Task>? _asyncHandler;

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
        internal ClientHandler(Func<TOptions, Task> handler)
        {
            Check.NotNull(handler, nameof(handler));
            _asyncHandler = (options, token) => handler(options);
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="handler">Client defined asynchronous handler with cancellation support.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handler"/> is null.</exception>
        internal ClientHandler(Func<TOptions, CancellationToken, Task> handler)
        {
            _asyncHandler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <summary>
        /// Invokes the handler synchronously.
        /// </summary>
        /// <param name="options">Options instance.</param>
        internal void Invoke(TOptions options)
        {
            if (_syncHandler == null)
            {
                _asyncHandler!(options, CancellationToken.None).GetAwaiter().GetResult();
                return;
            }

            _syncHandler(options);
        }

        /// <summary>
        /// Gets whether the handler is async.
        /// </summary>
        public bool IsAsync => _asyncHandler != null;

        /// <summary>
        /// Invokes the handler asynchronously with cancellation support.
        /// </summary>
        /// <param name="options">Options instance.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task</returns>
        internal Task InvokeAsync(TOptions options, CancellationToken cancellationToken)
        {
            if (_asyncHandler != null)
            {
                return _asyncHandler(options, cancellationToken);
            }
            
            _syncHandler!(options);
            
            return Task.CompletedTask;
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