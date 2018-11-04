// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

namespace Vertical.CommandLine.Mapping
{
    /// <summary>
    /// Defines the interface of an object that maps a converted argument value
    /// to an options instance.
    /// </summary>
    public interface IMapper<in TOptions, in TValue> where TOptions : class
    {
        /// <summary>
        /// Maps a value to the given options instance.
        /// </summary>
        /// <param name="options">Options instance.</param>
        /// <param name="value">Value to map.</param>
        void MapValue(TOptions options, TValue value);
        
        /// <summary>
        /// Gets whether the mapper supports multiple values.
        /// </summary>
        bool MultiValued { get; }
    }
}