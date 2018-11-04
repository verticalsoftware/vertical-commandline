// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

namespace Vertical.CommandLine.Provider
{
    /// <summary>
    /// Defines the interface of an object that provides the options instance.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    public interface IProvider<out TOptions>
    {
        /// <summary>
        /// Gets the options instance.
        /// </summary>
        /// <returns>Options instance</returns>
        TOptions GetInstance();
    }
}