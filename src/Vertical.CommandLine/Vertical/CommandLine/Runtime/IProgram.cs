// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System.Threading.Tasks;

namespace Vertical.CommandLine.Runtime
{
    /// <summary>
    /// Defines the interface of a client program.
    /// </summary>
    public interface IProgram
    {
        /// <summary>
        /// Invokes the client handler.
        /// </summary>
        /// <param name="options">Options object.</param>
        void Invoke(object options);

        /// <summary>
        /// Invokes the client handler asynchronously.
        /// </summary>
        /// <param name="options">Options object.</param>
        /// <returns>Task.</returns>
        Task InvokeAsync(object options);
    }
}