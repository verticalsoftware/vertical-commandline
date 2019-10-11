// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Infrastructure;

namespace Vertical.CommandLine.Mapping
{
    /// <summary>
    /// Functional class used for mapping.
    /// </summary>
    internal static class Mapper
    {
        /// <summary>
        /// Performs mapping of a converted value to options.
        /// </summary>
        /// <typeparam name="TOptions">Options type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="mapper">Mapper instance</param>
        /// <param name="options">Options object</param>
        /// <param name="context">Context, template or position argument</param>
        /// <param name="value">Value to map</param>
        internal static void MapValue<TOptions, TValue>(IMapper<TOptions, TValue> mapper,
            string context, TOptions options, TValue value) 
            where TOptions : class
        {
            try
            {
                mapper.MapValue(options, value);
            }
            catch (Exception ex) when (!(ex is UsageException))
            {
                throw ConfigurationExceptions.MappingFailed(context, ex);
            }
        }
    }
}