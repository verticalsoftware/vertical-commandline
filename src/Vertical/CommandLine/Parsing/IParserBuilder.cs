// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Conversion;
using Vertical.CommandLine.Mapping;
using Vertical.CommandLine.Validation;

namespace Vertical.CommandLine.Parsing
{
    /// <summary>
    /// Defines the interface of a parser builder.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    public interface IParserBuilder<out TOptions, TValue> : 
        IComponentSink<IValueConverter<TValue>>,
        IComponentSink<IValidator<TValue>>,
        IComponentSink<IMapper<TOptions, TValue>>
        where TOptions : class
    {
    }
}