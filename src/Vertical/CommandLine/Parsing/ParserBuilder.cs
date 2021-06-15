// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Conversion;
using Vertical.CommandLine.Infrastructure;
using Vertical.CommandLine.Mapping;
using Vertical.CommandLine.Validation;

namespace Vertical.CommandLine.Parsing
{
    /// <summary>
    /// Represents an object used to incrementally build an argument parser.
    /// </summary>
    public sealed class ParserBuilder<TOptions, TValue> : IParserBuilder<TOptions, TValue>
        where TOptions : class
    {
        private IValidator<TValue>? _validator;
        private IValueConverter<TValue>? _converter;
        private IMapper<TOptions, TValue>? _mapper;
        
        /// <summary>
        /// Builds a position argument parser.
        /// </summary>
        /// <returns><see cref="IArgumentParser{TOptions}"/></returns>
        public IArgumentParser<TOptions> PositionArgument(int argumentIndex) =>
            new PositionArgumentParser<TOptions,TValue>(argumentIndex, Converter, _validator, Mapper);
        
        /// <summary>
        /// Builds an option parser.
        /// </summary>
        /// <param name="template">Template that identifies the option.</param>
        /// <returns><see cref="IArgumentParser{TOptions}"/></returns>
        public IArgumentParser<TOptions> Option(Template template) => new OptionParser<TOptions,TValue>(template,
            Converter, _validator, Mapper);
        
        /// <summary>
        /// Builds a switch parser.
        /// </summary>
        /// <param name="template">Template that identifies the switch.</param>
        /// <returns><see cref="IArgumentParser{TOptions}"/></returns>
        public IArgumentParser<TOptions> Switch(Template template) => new SwitchParser<TOptions,TValue>(template,
            Converter, _validator, Mapper);

        // Resolves the converter
        private IValueConverter<TValue> Converter => _converter ?? ConverterFactory.CreateOrThrow<TValue>();

        // Resolves the property mapper
        private IMapper<TOptions, TValue> Mapper => _mapper ?? throw ConfigurationExceptions.NoPropertyMapper();

        /// <inheritdoc />
        void IComponentSink<IValueConverter<TValue>>.Sink(IValueConverter<TValue> component) => _converter = component;

        /// <inheritdoc />
        void IComponentSink<IValidator<TValue>>.Sink(IValidator<TValue> component) =>
            _validator = Validator.Combine(_validator, component);

        /// <inheritdoc />
        void IComponentSink<IMapper<TOptions, TValue>>.Sink(IMapper<TOptions, TValue> component) =>
            _mapper = component;
    }
}